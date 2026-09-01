using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test.Docs
{
    /// <summary>
    /// Guards the README examples: the C# snippets in README.md are byte-compared against
    /// the source fragments below (the fragments are compiled as part of this assembly and,
    /// for the quick start, executed), so the documentation can not drift from the API.
    /// </summary>
    public class ReadmeSnippetGuardTests
    {
        public const string QuickStartSource = """
using System;
using Xabe.FFmpeg;

string input = "movie.mkv";
string output = "movie.mp4";

IConversion conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(input, output);
IConversionResult result = await conversion.Start();

Console.WriteLine(result.Arguments);
""";

        public const string StreamBuilderSource = """
using System.Linq;
using Xabe.FFmpeg;

IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(input);
IVideoStream videoStream = mediaInfo.VideoStreams
    .First()
    .SetCodec(VideoCodec.h264)
    .SetSize(VideoSize.Hd480);

IConversion conversion = FFmpeg.Conversions.New()
    .AddStream(videoStream)
    .SetOutput(output)
    .SetOverwriteOutput(true);

await conversion.Start();
""";

        public const string RawParametersSource = """
using Xabe.FFmpeg;

IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(input);

IConversion conversion = FFmpeg.Conversions.New()
    .AddStream(mediaInfo.Streams)
    .AddParameter("-re", ParameterPosition.PreInput)
    .AddParameter("-ss 00:00:01 -t 00:00:05")
    .SetOutput(output)
    .SetOverwriteOutput(true);

string arguments = conversion.Build(); // inspect before running
await conversion.Start();
""";

        [Fact]
        public void Readme_QuickStartMatchesSourceFragment()
        {
            AssertContainsCodeBlock(QuickStartSource);
        }

        [Fact]
        public void Readme_StreamBuilderMatchesSourceFragment()
        {
            AssertContainsCodeBlock(StreamBuilderSource);
        }

        [Fact]
        public void Readme_RawParametersMatchesSourceFragment()
        {
            AssertContainsCodeBlock(RawParametersSource);
        }

        [Fact(DisplayName = "Quick start performs a real conversion and emits the advertised arguments")]
        public async Task QuickStart_RunsAgainstSampleMedia_AndEmitsExpectedArguments()
        {
            string input = Resources.MkvWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            try
            {
                // Mirrors the guarded quick start source fragment.
                IConversion conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(input, output);
                IConversionResult result = await conversion.Start();

                Assert.True(File.Exists(output), "expected output file to be created");

                // The output must be probeable media with the expected codecs.
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
                Assert.NotNull(mediaInfo);
                Assert.True(mediaInfo.Duration > TimeSpan.Zero);
                Assert.Single(mediaInfo.VideoStreams);
                Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
                Assert.True(mediaInfo.AudioStreams.Any());
                Assert.Equal("aac", mediaInfo.AudioStreams.First().Codec);

                string args = result.Arguments;
                Assert.Contains($"-i \"{input}\"", args, StringComparison.Ordinal);
                Assert.Contains($"\"{output}\"", args, StringComparison.Ordinal);
                Assert.Contains("-c:v h264", args, StringComparison.Ordinal);
                Assert.Contains("-c:a aac", args, StringComparison.Ordinal);
                Assert.Contains("-map 0:0", args, StringComparison.Ordinal);
                Assert.Contains("-map 0:1", args, StringComparison.Ordinal);
            }
            finally
            {
                if (File.Exists(output))
                {
                    File.Delete(output);
                }
            }
        }

        [Fact(DisplayName = "Stream builder example performs a real conversion")]
        public async Task StreamBuilder_RunsAgainstSampleMedia()
        {
            string input = Resources.MkvWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            try
            {
                // Mirrors the guarded stream builder source fragment.
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(input);
                IVideoStream videoStream = mediaInfo.VideoStreams
                    .First()
                    .SetCodec(VideoCodec.h264)
                    .SetSize(VideoSize.Hd480);

                IConversion conversion = FFmpeg.Conversions.New()
                    .AddStream(videoStream)
                    .SetOutput(output)
                    .SetOverwriteOutput(true);

                IConversionResult result = await conversion.Start();

                Assert.True(File.Exists(output), "expected output file to be created");
                Assert.Contains("-s 852x480", result.Arguments, StringComparison.Ordinal);
            }
            finally
            {
                if (File.Exists(output))
                {
                    File.Delete(output);
                }
            }
        }

        [Fact(DisplayName = "Raw parameters honor ParameterPosition in the built arguments")]
        public async Task RawParameters_BuildPlacesArgumentsAtDeclaredPositions()
        {
            string input = Resources.MkvWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            // Mirrors the guarded raw parameters source fragment, stopping at Build().
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(input);

            IConversion conversion = FFmpeg.Conversions.New()
                .AddStream(mediaInfo.Streams)
                .AddParameter("-re", ParameterPosition.PreInput)
                .AddParameter("-ss 00:00:01 -t 00:00:05")
                .SetOutput(output)
                .SetOverwriteOutput(true);

            string arguments = conversion.Build();

            int preIndex = arguments.IndexOf("-re", StringComparison.Ordinal);
            int inputFlagIndex = arguments.IndexOf("-i ", StringComparison.Ordinal);
            int postIndex = arguments.IndexOf("-ss 00:00:01", StringComparison.Ordinal);

            Assert.True(preIndex >= 0 && inputFlagIndex >= 0 && postIndex >= 0, $"expected all markers in built arguments: {arguments}");
            Assert.True(preIndex < inputFlagIndex, $"expected PreInput parameter before the input: {arguments}");
            Assert.True(postIndex > inputFlagIndex, $"expected PostInput parameter after the input: {arguments}");
        }

        private static void AssertContainsCodeBlock(string source)
        {
            string readme = NormalizeLineEndings(File.ReadAllText(ReadmePath()));
            string fence = "```csharp\n" + NormalizeLineEndings(source).TrimEnd('\n') + "\n```";
            Assert.Contains(fence, readme);
        }

        private static string NormalizeLineEndings(string text)
        {
            return text.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private static string ReadmePath()
        {
            DirectoryInfo current = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            while (current is not null)
            {
                string candidate = Path.Combine(current.FullName, "README.md");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }

            throw new FileNotFoundException("README.md not found above the test output directory");
        }
    }
}
