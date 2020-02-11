using System;
using System.Collections.Generic;
using System.Text;
using Xabe.FFmpeg.Exceptions;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class FFmpegExceptionCatcherTests
    {
        private readonly FFmpegExceptionCatcher _sut;

        public FFmpegExceptionCatcherTests()
        {
            _sut = new FFmpegExceptionCatcher();
        }

        [Fact]
        public void CatchErrors_UnrecognizedHwacceel_ThrowHardwareAcceleratorNotFoundException()
        {
            //Arrange
            var args = "args";
            var output = "Unrecognized hwaccel: a16f0cb5c0354b6197e9f3bc3108c017. Supported hwaccels: cuda dxva2 qsv d3d11va qsv cuvid";
            
            //Act
            var exception = Record.Exception(() => _sut.CatchFFmpegErrors(output, args));

            //Assert
            Assert.IsType<HardwareAcceleratorNotFoundException>(exception);
            Assert.Equal(output, exception.Message);
            var hardwareException = (HardwareAcceleratorNotFoundException)exception;
            Assert.Equal(args, hardwareException.InputParameters);
        }

        [Fact]
        public void CatchErrors_NoFFmpegError_NoExceptionIsThrown()
        {
            //Arrange
            var args = "args";
            var output = "FFmpeg result without exception";

            //Act
            var exception = Record.Exception(() => _sut.CatchFFmpegErrors(output, args));

            //Assert
            Assert.Null(exception);
        }
    }
}
