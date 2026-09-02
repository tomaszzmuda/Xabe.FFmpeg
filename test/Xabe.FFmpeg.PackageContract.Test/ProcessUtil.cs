using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    internal static class AssertUtil
    {
        public static void AreEqual<T>(T expected, T actual, string context)
        {
            Assert.True(
                EqualityComparer<T>.Default.Equals(expected, actual),
                context + " - expected [" + expected + "] but was [" + actual + "]");
        }
    }

    internal static class ProcessUtil
    {
        public static (int ExitCode, string Output) Run(
            string fileName,
            string[] arguments,
            TimeSpan timeout,
            string what,
            Action<ProcessStartInfo> configure = null)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (string argument in arguments)
            {
                psi.ArgumentList.Add(argument);
            }

            psi.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
            psi.Environment["DOTNET_NOLOGO"] = "1";
            configure?.Invoke(psi);

            var process = new Process { StartInfo = psi };
            var sb = new StringBuilder();
            object gate = new object();
            process.OutputDataReceived += DataHandler(gate, sb);
            process.ErrorDataReceived += DataHandler(gate, sb);

            int exitCode = -1;
            try
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException("'" + what + "' did not start");
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit((int)timeout.TotalMilliseconds))
                {
                    ThrowTimedOut(what, timeout, sb, gate);
                }

                process.WaitForExit(); // flush redirected streams
                exitCode = process.ExitCode;
            }
            finally
            {
                TryKill(process);
                process.Dispose();
            }

            return (exitCode, Locked(sb, gate));

            static void ThrowTimedOut(string what, TimeSpan timeout, StringBuilder sink, object gate)
            {
                throw new TimeoutException("'" + what + "' exceeded " + timeout + ":\n" + Tail(Locked(sink, gate)));
            }
        }

        private static DataReceivedEventHandler DataHandler(object gate, StringBuilder sb)
        {
            return (_, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                lock (gate)
                {
                    sb.AppendLine(e.Data);
                }
            };
        }

        private static string Locked(StringBuilder sb, object gate)
        {
            lock (gate)
            {
                return sb.ToString();
            }
        }

        private static void TryKill(Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
                // shutdown races are irrelevant here
            }
        }

        public static string Tail(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "<no output>";
            }

            string[] lines = text.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            int start = Math.Max(0, lines.Length - 40);
            return string.Join(Environment.NewLine, lines.Skip(start));
        }
    }

    internal static class ZipHelper
    {
        public static byte[] ReadEntry(string archivePath, string entryName)
        {
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            ZipArchiveEntry entry = archive.GetEntry(entryName);
            if (entry == null)
            {
                throw new InvalidOperationException(
                    archivePath + ": missing entry '" + entryName + "'. Entries:\n" + Names(archive));
            }

            using Stream stream = entry.Open();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public static bool HasEntry(string archivePath, string entryName)
        {
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            return archive.GetEntry(entryName) != null;
        }

        public static string[] Entries(string archivePath)
        {
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            return archive.Entries.Select(e => e.FullName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        }

        public static string Names(ZipArchive archive)
        {
            return string.Join(", ", archive.Entries.Select(e => e.FullName).OrderBy(n => n, StringComparer.Ordinal));
        }

        public static long EntrySize(string archivePath, string entryName)
        {
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            return archive.GetEntry(entryName)?.Length ?? -1;
        }
    }

    internal static class XmlHelper
    {
        public static XmlDocument Load(byte[] bytes)
        {
            // NuGet emits either nuspec XSD generation depending on the metadata in use; dropping
            // the default namespace keeps the plain XPath selectors in the contract tests readable.
            string text = Encoding.UTF8.GetString(bytes)
                .Replace(" xmlns=\"http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd\"", "")
                .Replace(" xmlns=\"http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd\"", "");
            var doc = new XmlDocument { PreserveWhitespace = true };
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
            doc.Load(ms);
            return doc;
        }

        public static string GetString(XmlNode parent, string xpath)
        {
            XmlNode node = parent?.SelectSingleNode(xpath);
            return node?.InnerText?.Trim();
        }

        public static string GetAttribute(XmlNode parent, string elementXPath, string attribute)
        {
            XmlNode node = parent?.SelectSingleNode(elementXPath);
            return node?.Attributes?[attribute]?.Value;
        }

    }

    internal static class HashUtil
    {
        public static string Sha256Hex(byte[] bytes)
        {
            return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        }
    }
}
