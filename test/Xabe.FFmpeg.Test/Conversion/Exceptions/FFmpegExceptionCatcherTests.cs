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
            Assert.IsType<ConversionException>(exception);
            Assert.Equal(output, exception.Message);
            var hardwareException = (HardwareAcceleratorNotFoundException)exception.InnerException;
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

        [Fact]
        public void CatchErrors_NoSuitableOutputFormat_ThrowFFmpegNoSuitableOutputFormatFoundException()
        {
            //Arrange
            var args = "args";
            var output = @"Unable to find a suitable output format for 'C:\Users\tomas\AppData\Local\Temp\4da4b324-3e25-42cb-b7b3-f9da041cf20c' C: \Users\tomas\AppData\Local\Temp\4da4b324 - 3e25 - 42cb - b7b3 - f9da041cf20c: Invalid argument";

            //Act
            var exception = Record.Exception(() => _sut.CatchFFmpegErrors(output, args));

            //Assert
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<FFmpegNoSuitableOutputFormatFoundException>(exception.InnerException);
            Assert.Equal(output, exception.Message);
            var conversionException = (ConversionException)exception;
            Assert.Equal(args, conversionException.InputParameters);
        }
    }
}
