namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>Sources for the disposable, near-fresh consumer apps the smoke tests scaffold.</summary>
    internal static class ConsumerScaffolding
    {
        public static string Csproj(string assemblyName, string packageName, string packageVersion)
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RollForward>Major</RollForward>
    <AssemblyName>{assemblyName}</AssemblyName>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""{packageName}"" Version=""{packageVersion}"" />
  </ItemGroup>

</Project>
";
        }

        public static string NuGetConfig(string localFeed)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""xab-local"" value=""{localFeed}"" />
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" />
  </packageSources>
</configuration>
";
        }

        public const string CoreProgram =
            "using System;\n" +
            "using System.IO;\n" +
            "using System.Linq;\n" +
            "using System.Reflection;\n" +
            "using System.Security.Cryptography;\n" +
            "using System.Threading;\n" +
            "using System.Threading.Tasks;\n" +
            "using Xabe.FFmpeg;\n" +
            "\n" +
            "internal static class Program\n" +
            "{\n" +
            "    private static async Task<int> Main(string[] args)\n" +
            "    {\n" +
            "        try\n" +
            "        {\n" +
            "            string mediaPath = args[0];\n" +
            "            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(\"N\") + \".mp4\");\n" +
            "            string executableDir = Environment.GetEnvironmentVariable(\"SMOKE_FFMPEG_DIR\");\n" +
            "            if (!string.IsNullOrWhiteSpace(executableDir))\n" +
            "            {\n" +
            "                FFmpeg.SetExecutablesPath(executableDir);\n" +
            "            }\n" +
            "\n" +
            "            IMediaInfo info = await FFmpeg.GetMediaInfo(mediaPath).ConfigureAwait(false);\n" +
            "            if (info.Duration <= TimeSpan.Zero || !info.Streams.Any())\n" +
            "            {\n" +
            "                Console.Error.WriteLine(\"PROBE-FAILED\");\n" +
            "                return 2;\n" +
            "            }\n" +
            "\n" +
            "            Console.WriteLine(\"PROBE-OK duration=\" + info.Duration);\n" +
            "\n" +
            "            IConversion conversion =\n" +
            "                await FFmpeg.Conversions.FromSnippet.ToMp4(mediaPath, outputPath).ConfigureAwait(false);\n" +
            "            await conversion.Start(CancellationToken.None).ConfigureAwait(false);\n" +
            "            if (!File.Exists(outputPath) || new FileInfo(outputPath).Length == 0L)\n" +
            "            {\n" +
            "                Console.Error.WriteLine(\"CONVERT-FAILED\");\n" +
            "                return 3;\n" +
            "            }\n" +
            "\n" +
            "            Console.WriteLine(\"CONVERT-OK bytes=\" + new FileInfo(outputPath).Length);\n" +
            "\n" +
            "            Assembly assembly = typeof(FFmpeg).Assembly;\n" +
            "            using (var sha = SHA256.Create())\n" +
            "            {\n" +
            "                byte[] hash = sha.ComputeHash(File.ReadAllBytes(assembly.Location));\n" +
            "                Console.WriteLine(\"ASM-LOCATION \" + assembly.Location);\n" +
            "                Console.WriteLine(\"ASM-SHA256 \" + Convert.ToHexString(hash).ToLowerInvariant());\n" +
            "            }\n" +
            "\n" +
            "            return 0;\n" +
            "        }\n" +
            "        catch (Exception ex)\n" +
            "        {\n" +
            "            Console.Error.WriteLine(\"SMOKE-EXCEPTION \" + ex.GetType().Name + \": \" + ex.Message);\n" +
            "            return 4;\n" +
            "        }\n" +
            "    }\n" +
            "}\n";

        public const string DownloaderProgram =
            "using System;\n" +
            "using System.IO;\n" +
            "using System.Reflection;\n" +
            "using System.Security.Cryptography;\n" +
            "using System.Threading.Tasks;\n" +
            "using Xabe.FFmpeg;\n" +
            "using Xabe.FFmpeg.Downloader;\n" +
            "\n" +
            "internal static class Program\n" +
            "{\n" +
            "    private static async Task<int> Main(string[] args)\n" +
            "    {\n" +
            "        try\n" +
            "        {\n" +
            "            string downloadDir = args[0];\n" +
            "            string mediaPath = args[1];\n" +
            "            Directory.CreateDirectory(downloadDir);\n" +
            "\n" +
            "            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, downloadDir).ConfigureAwait(false);\n" +
            "\n" +
            "            string ffmpegName = \"ffmpeg\" + (OperatingSystem.IsWindows() ? \".exe\" : \"\");\n" +
            "            string ffprobeName = \"ffprobe\" + (OperatingSystem.IsWindows() ? \".exe\" : \"\");\n" +
            "            if (!File.Exists(Path.Combine(downloadDir, ffmpegName)) ||\n" +
            "                !File.Exists(Path.Combine(downloadDir, ffprobeName)))\n" +
            "            {\n" +
            "                Console.Error.WriteLine(\"DOWNLOAD-FAILED files=\" + string.Join(\"; \", Directory.GetFiles(downloadDir)));\n" +
            "                return 2;\n" +
            "            }\n" +
            "\n" +
            "            Console.WriteLine(\"DOWNLOAD-OK\");\n" +
            "\n" +
            "            FFmpeg.SetExecutablesPath(downloadDir);\n" +
            "            IMediaInfo info = await FFmpeg.GetMediaInfo(mediaPath).ConfigureAwait(false);\n" +
            "            if (info.Duration <= TimeSpan.Zero)\n" +
            "            {\n" +
            "                Console.Error.WriteLine(\"DISCOVER-FAILED\");\n" +
            "                return 3;\n" +
            "            }\n" +
            "\n" +
            "            Console.WriteLine(\"DISCOVER-OK duration=\" + info.Duration);\n" +
            "\n" +
            "            Assembly assembly = typeof(FFmpeg).Assembly;\n" +
            "            using (var sha = SHA256.Create())\n" +
            "            {\n" +
            "                byte[] hash = sha.ComputeHash(File.ReadAllBytes(assembly.Location));\n" +
            "                Console.WriteLine(\"CORE-ASM-LOCATION \" + assembly.Location);\n" +
            "                Console.WriteLine(\"CORE-ASM-SHA256 \" + Convert.ToHexString(hash).ToLowerInvariant());\n" +
            "            }\n" +
            "\n" +
            "            return 0;\n" +
            "        }\n" +
            "        catch (Exception ex)\n" +
            "        {\n" +
            "            Console.Error.WriteLine(\"SMOKE-EXCEPTION \" + ex.GetType().Name + \": \" + ex.Message);\n" +
            "            return 4;\n" +
            "        }\n" +
            "    }\n" +
            "}\n";
    }
}
