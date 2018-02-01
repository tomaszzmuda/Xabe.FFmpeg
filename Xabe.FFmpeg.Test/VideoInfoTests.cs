using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class VideoInfoTests
    {
        [Fact]
        public async Task AudioPopertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.Mp3);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mp3, mediaInfo.FileInfo.Extension);
            Assert.Equal("audio.mp3", mediaInfo.FileInfo.Name);

            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), audioStream.Duration);

            Assert.Equal(0, mediaInfo.VideoStreams.Count());

            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(216916, mediaInfo.Size);
        }

        [Fact]
        public async Task GetMultipleStreamsTest()
        {
            IMediaInfo videoInfo = await MediaInfo.Get(Resources.MultipleStream);

            Assert.Equal(1, videoInfo.VideoStreams.Count());
            Assert.Equal(2, videoInfo.AudioStreams.Count());
            Assert.Equal(8, videoInfo.SubtitleStreams.Count());
        }

        [Fact]
        public async Task GetBitrateTest()
        {
            IMediaInfo info = await MediaInfo.Get(@"C:\tmp\input.mp4");
            IVideoStream videoStream = info.VideoStreams.First();

            Assert.Equal(862991, videoStream.Bitrate);
        }

        [Fact]
        public async Task IncorrectFormatTest()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await MediaInfo.Get(Resources.Dll));
        }

        [Fact]
        public async Task MkvPropertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mkv, mediaInfo.FileInfo.Extension);
            Assert.Equal("SampleVideo_360x240_1mb.mkv", mediaInfo.FileInfo.Name);

            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Format);
            Assert.Equal(1, audioStream.Index);
            Assert.Equal(TimeSpan.FromSeconds(9), audioStream.Duration);

            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(0, videoStream.Index);
            Assert.Equal(25, videoStream.FrameRate);
            Assert.Equal(240, videoStream.Height);
            Assert.Equal(320, videoStream.Width);
            Assert.Equal("4:3", videoStream.Ratio);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(9), videoStream.Duration);

            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(1055721, mediaInfo.Size);
        }

        [Fact]
        public async Task PropertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.Mp4WithAudio);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mp4, mediaInfo.FileInfo.Extension);
            Assert.Equal("input.mp4", mediaInfo.FileInfo.Name);

            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), audioStream.Duration);

            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(25, videoStream.FrameRate);
            Assert.Equal(720, videoStream.Height);
            Assert.Equal(1280, videoStream.Width);
            Assert.Equal("16:9", videoStream.Ratio);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), videoStream.Duration);

            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(2107842, mediaInfo.Size);
        }

        [Fact(Skip = "Results will change in the nearest future")]
        public async Task ToStringTest()
        {
            IMediaInfo videoInfo = await MediaInfo.Get(Resources.Mp4WithAudio);
            string output = videoInfo.ToString();
            string expectedOutput =
                $"Video name: input.mp4{Environment.NewLine}Video extension : .mp4{Environment.NewLine}Video duration : 00:00:13{Environment.NewLine}Video format : h264{Environment.NewLine}Audio format : aac{Environment.NewLine}Audio duration : 00:00:13{Environment.NewLine}Aspect Ratio : 16:9{Environment.NewLine}Framerate : 16:9 fps{Environment.NewLine}Resolution : 1280 x 720{Environment.NewLine}Size : 2107842 b";
            Assert.EndsWith(expectedOutput, output);
        }
    }
}
