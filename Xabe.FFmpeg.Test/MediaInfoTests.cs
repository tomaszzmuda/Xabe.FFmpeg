using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class MediaInfoTests
    {
        [Fact]
        public async Task AudioPopertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mp3, mediaInfo.FileInfo.Extension);
            Assert.Equal("audio.mp3", mediaInfo.FileInfo.Name);

            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), audioStream.Duration);

            Assert.Empty(mediaInfo.VideoStreams);

            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(216916, mediaInfo.Size);
        }

        [Fact]
        public async Task GetMultipleStreamsTest()
        {
            IMediaInfo videoInfo = await MediaInfo.Get(Resources.MultipleStream).ConfigureAwait(false);

            Assert.Single(videoInfo.VideoStreams);
            Assert.Equal(2, videoInfo.AudioStreams.Count());
            Assert.Equal(8, videoInfo.SubtitleStreams.Count());
        }

        [Fact]
        public async Task GetVideoBitrateTest()
        {
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First();

            Assert.Equal(860233, videoStream.Bitrate);
        }

        [Fact]
        public async Task IncorrectFormatTest()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await MediaInfo.Get(Resources.Dll).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task Mp4PropertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.BunnyMp4).ConfigureAwait(false);

            Assert.True(mediaInfo.Streams.Any());
        }

        [Fact]
        public async Task MkvPropertiesTest()
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mkv, mediaInfo.FileInfo.Extension);
            Assert.Equal("SampleVideo_360x240_1mb.mkv", mediaInfo.FileInfo.Name);

            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Format);
            Assert.Equal(1, audioStream.Index);
            Assert.Equal(TimeSpan.FromSeconds(9), audioStream.Duration);

            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(0, videoStream.Index);
            Assert.Equal(25, videoStream.Framerate);
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
            IMediaInfo mediaInfo = await MediaInfo.Get(Resources.Mp4WithAudio).ConfigureAwait(false);

            Assert.True(File.Exists(mediaInfo.FileInfo.FullName));
            Assert.Equal(FileExtensions.Mp4, mediaInfo.FileInfo.Extension);
            Assert.Equal("input.mp4", mediaInfo.FileInfo.Name);

            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), audioStream.Duration);

            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(25, videoStream.Framerate);
            Assert.Equal(720, videoStream.Height);
            Assert.Equal(1280, videoStream.Width);
            Assert.Equal("16:9", videoStream.Ratio);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(13), videoStream.Duration);

            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(2107842, mediaInfo.Size);
        }

        [Theory]
        [InlineData("檔")]
        [InlineData("אספירין")]
        [InlineData("एस्पिरि")]
        [InlineData("阿司匹林")]
        [InlineData("アセチルサリチル酸")]
        public async Task GetMediaInfo_NonUTF8CharactersInPath(string path)
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), path));
            var destFile = Path.Combine(dir.FullName, "temp.mp4");
            File.Copy(Resources.Mp4WithAudio, destFile, true);

            IMediaInfo mediaInfo = await MediaInfo.Get(destFile).ConfigureAwait(false);

            Assert.NotNull(mediaInfo);
            Assert.Equal(FileExtensions.Mp4, mediaInfo.FileInfo.Extension);
        }
    }
}
