﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams.SubtitleStream;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class VideoSnippetsTests(StorageFixture storageFixture, RtspServerFixture rtspServer) : IClassFixture<StorageFixture>, IClassFixture<RtspServerFixture>
    {
        private readonly StorageFixture _storageFixture = storageFixture;
        private readonly RtspServerFixture _rtspServer = rtspServer;

        public static IEnumerable<object[]> JoinFiles =>
        [
            [Resources.MkvWithAudio, Resources.Mp4WithAudio, 23, 1280, 720, "16:9"],
            [Resources.MkvWithAudio, Resources.MkvWithAudio, 19, 320, 240, "4:3"],
            [Resources.MkvWithAudio, Resources.Mp4, 23, 1280, 720, "16:9"]
        ];

        [Theory]
        [MemberData(nameof(JoinFiles))]
        public async Task Concatenate_Test(string firstFile, string secondFile, int duration, int width, int height, string ratio)
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Concatenate(output, firstFile, secondFile)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(duration, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(width, videoStream.Width);
            Assert.Equal(height, videoStream.Height);
            Assert.Contains($"-aspect {ratio}", result.Arguments);
        }

        [Fact]
        public async Task ChangeSizeTest()
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            var input = Resources.MkvWithAudio;
            _ = await (await FFmpeg.Conversions.FromSnippet.ChangeSize(input, output, 640, 360))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(640, videoStream.Width);
            Assert.Equal(360, videoStream.Height);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(Resources.Mp4WithAudio));
            _ = await (await FFmpeg.Conversions.FromSnippet.ExtractVideo(Resources.Mp4WithAudio, output))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Empty(mediaInfo.AudioStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal("h264", videoStream.Codec);
        }

        [Fact]
        public async Task SnapshotInvalidArgumentTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Png);
            await Assert.ThrowsAsync<ArgumentException>(async () => await (await FFmpeg.Conversions.FromSnippet.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(999)))
                                                                                    .Start());
        }

        [Theory]
        [InlineData(FileExtensions.Png, 1825653)]
        [InlineData(FileExtensions.Jpg, 84461)]
        public async Task SnapshotTest(string extension, long expectedLength)
        {
            var output = _storageFixture.GetTempFileName(extension);
            _ = await (await FFmpeg.Conversions.FromSnippet.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(0)))
                                             .Start();

            Assert.True(File.Exists(output));
            // It does not has to be the same
            Assert.Equal(expectedLength / 10, (await File.ReadAllBytesAsync(output)).LongLength / 10);
        }

        [Fact]
        public async Task SplitVideoTest()
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Split(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8)))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal(TimeSpan.FromSeconds(8), audioStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), videoStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), mediaInfo.Duration);
        }

        [Fact]
        public async Task WatermarkTest()
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.SetWatermark(Resources.Mp4WithAudio, output, Resources.PngSample, Position.Center))
                                             .Start();

            Assert.Contains("overlay=", result.Arguments);
            Assert.Contains(Resources.Mp4WithAudio, result.Arguments);
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Equal("h264", videoStream.Codec);
        }

        // Docker container needs to be run with some m3u8 stream
        //[Fact]
        //public async Task SaveM3U8Stream_Https_EverythingWorks()
        //{
        //    var output = Path.ChangeExtension(Path.GetTempFileName(), "mkv");
        //    var uri = new Uri("https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8");

        //    var exception = await Record.ExceptionAsync(async () => await (await FFmpeg.Conversions.FromSnippet.SaveM3U8Stream(uri, output, TimeSpan.FromSeconds(1)))
        //                                                            .Start());

        //    Assert.Null(exception);
        //}

        //[Fact]
        //public async Task SaveM3U8Stream_Http_EverythingWorks()
        //{
        //    var output = Path.ChangeExtension(Path.GetTempFileName(), "mkv");
        //    var uri = new Uri("http://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8");

        //    var exception = await Record.ExceptionAsync(async () => await (await FFmpeg.Conversions.FromSnippet.SaveM3U8Stream(uri, output, TimeSpan.FromSeconds(1)))
        //                                                            .Start());

        //    Assert.Null(exception);
        //}

        [Fact]
        public async Task SaveM3U8Stream_NotExisting_ExceptionIsThrown()
        {
            var output = Path.ChangeExtension(Path.GetTempFileName(), "mkv");
            var uri = new Uri("http://www.bitdash-a.akamaihd.net/notexisting.m3u8");

            var exception = await Record.ExceptionAsync(async () => await (await FFmpeg.Conversions.FromSnippet.SaveM3U8Stream(uri, output, TimeSpan.FromSeconds(1)))
                                                                    .Start());

            Assert.NotNull(exception);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithSubtitles_SkipSubtitles()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithSubtitles, output)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Empty(mediaInfo.SubtitleStreams);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithSubtitles_SkipSubtitlesWithParameter()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithSubtitles, output, false)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Empty(mediaInfo.SubtitleStreams);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithSubtitles_KeepSubtitles()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithSubtitles, output, true)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Equal(2, mediaInfo.SubtitleStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Theory]
        [InlineData(VideoCodec.hevc, AudioCodec.aac, SubtitleCodec.mov_text)]
        [InlineData(VideoCodec.h264, AudioCodec.aac, SubtitleCodec.mov_text)]
        public async Task BasicTranscode_InputFileWithSubtitles_KeepSubtitles(VideoCodec videoCodec, AudioCodec audioCodec, SubtitleCodec subtitleCodec)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Transcode(Resources.MkvWithSubtitles, output, videoCodec, audioCodec, subtitleCodec, true)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Equal(2, mediaInfo.SubtitleStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal(videoCodec.ToString(), videoStream.Codec);
            Assert.Equal(audioCodec.ToString(), audioStream.Codec);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Theory]
        [InlineData(VideoCodec.hevc, AudioCodec.aac, SubtitleCodec.copy)]
        [InlineData(VideoCodec.h264, AudioCodec.aac, SubtitleCodec.copy)]
        public async Task BasicTranscode_InputFileWithSubtitles_SkipSubtitles(VideoCodec videoCodec, AudioCodec audioCodec, SubtitleCodec subtitleCodec)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Transcode(Resources.MkvWithSubtitles, output, videoCodec, audioCodec, subtitleCodec)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Empty(mediaInfo.SubtitleStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal(videoCodec.ToString(), videoStream.Codec);
            Assert.Equal(audioCodec.ToString(), audioStream.Codec);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Theory]
        [InlineData(VideoCodec.hevc, AudioCodec.aac, SubtitleCodec.copy)]
        [InlineData(VideoCodec.h264, AudioCodec.aac, SubtitleCodec.copy)]
        public async Task BasicTranscode_InputFileWithSubtitles_SkipSubtitlesWithParameter(VideoCodec videoCodec, AudioCodec audioCodec, SubtitleCodec subtitleCodec)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Transcode(Resources.MkvWithSubtitles, output, videoCodec, audioCodec, subtitleCodec, false)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Empty(mediaInfo.SubtitleStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal(videoCodec.ToString(), videoStream.Codec);
            Assert.Equal(audioCodec.ToString(), audioStream.Codec);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_SloMoVideo_CorrectFramerate()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.SloMoMp4, output)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(3, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Empty(mediaInfo.SubtitleStreams);
            // It does not has to be the same
            Assert.Equal(116, (int)videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithMultipleStreams_CorrectResult()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MultipleStream, output)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(46, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Equal(2, mediaInfo.AudioStreams.Count());
            Assert.Empty(mediaInfo.SubtitleStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(24, videoStream.Framerate);
        }

        [Fact]
        public async Task Rtsp_GotTwoStreams_SaveEverything()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            await _rtspServer.Publish(Resources.BunnyMp4, "bunny");
            await Task.Delay(2000);

            var mediaInfo = await FFmpeg.GetMediaInfo("rtsp://127.0.0.1:8554/bunny");

            await FFmpeg.Conversions.New().AddStream(mediaInfo.Streams).SetInputTime(TimeSpan.FromSeconds(3)).SetOutput(output).Start();

            IMediaInfo result = await FFmpeg.GetMediaInfo(output);
            Assert.True(result.Duration > TimeSpan.FromSeconds(0));
            Assert.Single(result.VideoStreams);
            Assert.Single(result.AudioStreams);
            Assert.Empty(result.SubtitleStreams);
            Assert.Equal("h264", result.VideoStreams.First().Codec);
            Assert.Equal(23, (int)result.VideoStreams.First().Framerate);
            Assert.Equal(640, result.VideoStreams.First().Width);
            Assert.Equal(360, result.VideoStreams.First().Height);
            Assert.Equal("aac", result.AudioStreams.First().Codec);
        }
    }
}
