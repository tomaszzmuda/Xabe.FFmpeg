//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Xabe.FFmpeg.Enums;
//using Xabe.FFmpeg.Exceptions;
//using Xunit;

//namespace Xabe.FFmpeg.Test
//{
//    public class ConversionTests
//    {
//        [Theory]
//        [InlineData(RotateDegrees.Clockwise)]
//        [InlineData(RotateDegrees.Invert)]
//        public async Task TransposeTest(RotateDegrees rotateDegrees)
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .Rotate(rotateDegrees)
//                                                    .SetOutput(outputPath)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Theory]
//        [InlineData(Position.UpperRight)]
//        [InlineData(Position.BottomRight)]
//        [InlineData(Position.Left)]
//        [InlineData(Position.Right)]
//        [InlineData(Position.Up)]
//        [InlineData(Position.BottomLeft)]
//        [InlineData(Position.UpperLeft)]
//        [InlineData(Position.Center)]
//        [InlineData(Position.Bottom)]
//        public async Task WatermarkTest(Position position)
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetWatermark(Resources.PngSample, position)
//                                                    .SetOutput(outputPath)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Theory]
//        [InlineData(12, 12, 12, 1.0, Channel.Both)]
//        [InlineData(6, 6, 6, 2.0, Channel.Both)]
//        [InlineData(24, 24, 24, 0.5, Channel.Both)]
//        public async Task ChangeMediaSpeedSpeedTest(int expectedDuration, int expectedVideoDuration, int expectedAudioDuration, double speed, Channel channel)
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetSpeed(Speed.UltraFast)
//                                                    .UseMultiThread(true)
//                                                    .SetOutput(outputPath)
//                                                    .SetCodec(VideoCodec.h264, 2400)
//                                                    .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
//                                                    .ChangeSpeed(channel, speed)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
//            Assert.Equal(TimeSpan.FromSeconds(expectedAudioDuration), mediaInfo.AudioDuration);
//            Assert.Equal(TimeSpan.FromSeconds(expectedVideoDuration), mediaInfo.VideoDuration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Theory]
//        [InlineData(2.5)]
//        [InlineData(0.4)]
//        public async Task ChangeMediaSpeedSpeedTestArgumentOutOfRange(double multiplication)
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

//            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Conversion.New()
//                                                                                              .SetInput(Resources.MkvWithAudio)
//                                                                                              .SetSpeed(Speed.UltraFast)
//                                                                                              .UseMultiThread(true)
//                                                                                              .SetOutput(outputPath)
//                                                                                              .SetCodec(VideoCodec.h264, 2400)
//                                                                                              .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
//                                                                                              .ChangeSpeed(Channel.Both, multiplication)
//                                                                                              .Start());
//        }

//        [Fact]
//        public async Task AdditionalParametersTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);

//            bool conversionResult = await Conversion.New().SetInput(Resources.MkvWithAudio)
//                                                          .AddParameter($"-ss {TimeSpan.FromSeconds(1)} -t {TimeSpan.FromSeconds(1)}")
//                                                          .AddParameter("-s 1920x1080")
//                                                          .SetOutput(outputPath)
//                                                          .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(1), mediaInfo.Duration);
//            Assert.Equal(1920, mediaInfo.Width);
//        }

//        [Fact]
//        public async Task AddSubtitlesTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
//            bool conversionResult = await Conversion.New()
//                .SetInput(Resources.MkvWithAudio)
//                .AddSubtitles(Resources.SubtitleSrt, "ger")
//                .AddSubtitles(Resources.SubtitleSrt, "eng")
//                .StreamCopy(Channel.Both)
//                .SetOutput(outputPath)
//                .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(3071), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task BurnSubtitlesTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                .SetInput(Resources.MkvWithAudio)
//                .SetSubtitle(Resources.SubtitleSrt)
//                .SetOutput(outputPath)
//                .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task BurnSubtitlesWithParametersTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            string subtitle = Resources.SubtitleSrt.Replace("\\", "\\\\")
//                                                    .Replace(":", "\\:");

//            IConversion conversion = Conversion.New()
//                .SetInput(Resources.MkvWithAudio)
//                .SetSubtitle(Resources.SubtitleSrt, "UTF-8", "Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30", new VideoSize(1024, 768))
//                .SetOutput(outputPath);
//            bool conversionResult = await conversion.Start();
            
//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//            Assert.Equal($"-i \"{Resources.MkvWithAudio}\" -n -filter:v \"subtitles='{subtitle}':charenc=UTF-8:force_style='Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30':original_size=1024x768\" \"{outputPath}\"", conversion.Build());
//        }

//        [Fact]
//        public async Task ChangeOutputFramesCountTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .SetOutputFramesCount(50)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(2), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//            Assert.Equal(50, mediaInfo.Duration.TotalSeconds * mediaInfo.FrameRate);
//        }

//        [Fact]
//        public async Task ClearParametersTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            IConversion conversion = Conversion.New()
//                                               .SetInput(Resources.MkvWithAudio)
//                                               .SetCodec(VideoCodec.vp8)
//                                               .SetScale(VideoSize.Ega)
//                                               .StreamCopy(Channel.Both)
//                                               .Reverse(Channel.Both)
//                                               .SetOutput(outputPath);

//            conversion.Clear();

//            bool conversionResult = await conversion.SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task ConcatConversionStatusTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
//            IConversion conversion = Conversion.New()
//                                               .StreamCopy(Channel.Both)
//                                               .SetBitstreamFilter(Channel.Audio, BitstreamFilter.Aac_AdtstoAsc)
//                                               .SetOutput(outputPath)
//                                               .Concatenate(Resources.TsWithAudio, Resources.TsWithAudio);

//            TimeSpan currentProgress;

//            conversion.OnProgress += (sender, e) => { currentProgress = e.Duration; };
//            bool conversionResult = await conversion.Start();

//            Assert.True(conversionResult);
////            Assert.True(currentProgress > TimeSpan.Zero);
//            Assert.True((await MediaInfo.Get(outputPath)).Duration == TimeSpan.FromSeconds(26));
//        }

//        [Fact]
//        public async Task ConcatVideosTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Ts);
//            bool conversionResult = await Conversion.New()
//                                                    .StreamCopy(Channel.Both)
//                                                    .SetBitstreamFilter(Channel.Audio, BitstreamFilter.Aac_AdtstoAsc)
//                                                    .SetOutput(outputPath)
//                                                    .Concatenate(Resources.TsWithAudio, Resources.TsWithAudio)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(26), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task ConversionStatusTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
//            IConversion conversion = Conversion.New()
//                                               .SetInput(Resources.MkvWithAudio)
//                                               .SetOutput(outputPath)
//                                               .SetFormat(MediaFormat.mpegts);

//            TimeSpan currentProgress;
//            TimeSpan videoLength;

//            conversion.OnProgress += (sender, e) =>
//            {
//                currentProgress = e.Duration;
//                videoLength = e.TotalLength;
//            };
//            bool conversionResult = await conversion.Start();

//            Assert.True(conversionResult);
//            Assert.True(currentProgress > TimeSpan.Zero);
//            Assert.True(currentProgress <= videoLength);
//            Assert.True(videoLength == TimeSpan.FromSeconds(9));
//        }

//        [Fact]
//        public async Task DisableAudioChannelTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .DisableChannel(Channel.Audio)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Null(mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task DisableVideoChannelTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .DisableChannel(Channel.Video)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Null(mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task FFmpegDataReceivedTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
//            IConversion conversion = Conversion.New()
//                                               .SetInput(Resources.MkvWithAudio)
//                                               .SetOutput(outputPath)
//                                               .SetFormat(MediaFormat.mpegts);

//            var ffmpegOuput = "";

//            conversion.OnDataReceived += (sender, args) => { ffmpegOuput += $"{args.Data}{Environment.NewLine}"; };
//            bool conversionResult = await conversion.Start();

//            Assert.True(conversionResult);
//            Assert.Contains($"video:365kB audio:567kB subtitle:0kB other streams:0kB global headers:0kB", ffmpegOuput);
//        }

//        [Fact]
//        public async Task FileExistsException()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            File.Create(outputPath);
//            await Assert.ThrowsAsync<ConversionException>(async () =>
//                await Conversion.New()
//                                .SetInput(Resources.MkvWithAudio)
//                                .SetOutput(outputPath)
//                                .Start());
//        }

//        [Fact]
//        public async Task IncompatibleParametersTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            await Assert.ThrowsAsync<ConversionException>(async () =>
//            {
//                try
//                {
//                    await Conversion.New()
//                                    .SetInput(Resources.MkvWithAudio)
//                                    .SetOutput(outputPath)
//                                    .SetCodec(VideoCodec.h264, 2400)
//                                    .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
//                                    .Reverse(Channel.Both)
//                                    .StreamCopy(Channel.Both)
//                                    .Start();
//                }
//                catch(ConversionException e)
//                {
//                    Assert.Equal(
//                        $"-i \"{Resources.MkvWithAudio}\" -n -codec:v h264 -b:v 2400k -codec:a aac -b:a 384k -strict experimental -c copy -vf reverse -af areverse \"{outputPath}\"",
//                        e.InputParameters);
//                    Assert.EndsWith(
//                        $"Filtergraph \'reverse\' was defined for video output stream 0:0 but codec copy was selected.{Environment.NewLine}Filtering and streamcopy cannot be used together.",
//                        e.Message);
//                    throw;
//                }
//            });
//        }

//        [Fact]
//        public async Task LoopTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Gif);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.Mp4)
//                                                    .SetLoop(1)
//                                                    .SetOutput(outputPath)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(0), mediaInfo.Duration);
//            Assert.Equal("gif", mediaInfo.MediaFormat);
//            Assert.Null(mediaInfo.AudioFormat);
//            Assert.Equal("16:9", mediaInfo.Ratio);
//            Assert.Equal(25, mediaInfo.FrameRate);
//            Assert.Equal(1280, mediaInfo.Width);
//            Assert.Equal(720, mediaInfo.Height);
//        }

//        [Fact]
//        public async Task MultipleTaskTest()
//        {
//            string mp4Output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            string tsOutput = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Ts);

//            IConversion conversion = Conversion.New();
//            Task<bool> toMp4 = conversion
//                .SetInput(Resources.MkvWithAudio)
//                .SetOutput(mp4Output)
//                .Start();

//            conversion.SetOutput(tsOutput);
//            Assert.True(await conversion.Start());
//            Assert.True(await toMp4);
//        }

//        [Fact]
//        public async Task PassArgumentsTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            string arguments = $"-i \"{Resources.MkvWithAudio}\" \"{outputPath}\"";

//            bool conversionResult = await Conversion.New()
//                                                    .Start(arguments);

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task ReverseTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetSpeed(Speed.UltraFast)
//                                                    .UseMultiThread(true)
//                                                    .SetOutput(outputPath)
//                                                    .SetCodec(VideoCodec.h264, 2400)
//                                                    .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
//                                                    .Reverse(Channel.Both)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task ScaleTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetSpeed(Speed.UltraFast)
//                                                    .UseMultiThread(true)
//                                                    .SetOutput(outputPath)
//                                                    .SetScale(VideoSize.Sqcif)
//                                                    .SetCodec(VideoCodec.h264, 2400)
//                                                    .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//            Assert.Equal(128, mediaInfo.Width);
//            Assert.Equal(96, mediaInfo.Height);
//        }

//        [Fact]
//        public async Task SeekLengthTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            IConversion conversion = Conversion.New()
//                                               .SetInput(Resources.MkvWithAudio)
//                                               .SetSeek(TimeSpan.FromSeconds(2))
//                                               .SetOutput(outputPath);


//            TimeSpan currentProgress;
//            TimeSpan videoLength;
//            conversion.OnProgress += (sender, e) =>
//            {
//                currentProgress = e.Duration;
//                videoLength = e.TotalLength;
//            };

//            await conversion.Start();

//            Assert.True(currentProgress > TimeSpan.Zero);
//            Assert.True(currentProgress <= videoLength);
//            Assert.True(videoLength == TimeSpan.FromSeconds(7));
//        }

//        [Fact]
//        public async Task SimpleConversionTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task X265Test()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                .SetInput(Resources.MkvWithAudio)
//                .SetCodec(VideoCodec.hevc)
//                .SetOutput(outputPath)
//                .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("hevc", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public async Task SizeTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .SetSize(new VideoSize(640, 480))
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("h264", mediaInfo.MediaFormat);
//            Assert.Equal("aac", mediaInfo.AudioFormat);
//            Assert.Equal(640, mediaInfo.Width);
//            Assert.Equal(480, mediaInfo.Height);
//        }

//        [Fact]
//        public async Task SetNullSizeTest()
//        {
//            string inputPath = Resources.MkvWithAudio;
//            var inputMediaInfo = await MediaInfo.Get(inputPath);
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(inputPath)
//                                                    .SetOutput(outputPath)
//                                                    .SetSize(null)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(inputMediaInfo.Width, mediaInfo.Width);
//            Assert.Equal(inputMediaInfo.Height, mediaInfo.Height);
//        }

//        [Fact]
//        public async Task StopFFmpegProcessTest()
//        {
//            var cancellationTokenSource = new CancellationTokenSource();
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Ts);
//            IConversion conversion = Conversion.New();
//            Task<bool> result = conversion
//                .SetInput(Resources.MkvWithAudio)
//                .SetScale(VideoSize.Uhd4320)
//                .SetCodec(VideoCodec.theora, 2400)
//                .SetCodec(AudioCodec.libvorbis, AudioQuality.Ultra)
//                .SetOutput(outputPath)
//                .SetSpeed(Speed.VerySlow)
//                .UseMultiThread(false)
//                .Start(cancellationTokenSource.Token);

//            cancellationTokenSource.Cancel(false);
//            Assert.False(await result);
//        }

//        [Fact]
//        public async Task TimeoutTest()
//        {
//            var cancellationTokenSource = new CancellationTokenSource();
//            cancellationTokenSource.CancelAfter(1000);
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Ts);
//            IConversion conversion = Conversion.New();
//            Task<bool> result = conversion
//                .SetInput(Resources.MkvWithAudio)
//                .SetScale(VideoSize.Uhd4320)
//                .SetCodec(VideoCodec.theora, 2400)
//                .SetCodec(AudioCodec.libvorbis, AudioQuality.Ultra)
//                .SetOutput(outputPath)
//                .SetSpeed(Speed.VerySlow)
//                .UseMultiThread(false)
//                .Start(cancellationTokenSource.Token);

//            Assert.False(await result);
//        }

//        [Fact]
//        public async Task VideoCodecTest()
//        {
//            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
//            bool conversionResult = await Conversion.New()
//                                                    .SetInput(Resources.MkvWithAudio)
//                                                    .SetOutput(outputPath)
//                                                    .SetFormat(MediaFormat.mpegts)
//                                                    .Start();

//            Assert.True(conversionResult);
//            var mediaInfo = await MediaInfo.Get(outputPath);
//            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
//            Assert.Equal("mpeg2video", mediaInfo.MediaFormat);
//            Assert.Equal("mp2", mediaInfo.AudioFormat);
//        }

//        [Fact]
//        public void BuildParameterOrderTest()
//        {
//            var parameters = Conversion.New().AddParameter("-preset fast")
//                                             .AddParameter("-vf scale=1280x720")
//                                             .Build();

//            Assert.True(parameters.IndexOf("-preset fast", StringComparison.Ordinal) < parameters.IndexOf("-vf scale=1280x720", StringComparison.Ordinal));
//        }
//    }
//}
