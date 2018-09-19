using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ProbeTests
    {
        [Fact]
        public async Task StartTest()
        {
            string result = await Probe.New()
                 .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time -of csv=print_section=0 {Resources.Mp4}");

            IEnumerable<string> values = result.Split('\n')
                               .Where(x => !string.IsNullOrEmpty(x));

            Assert.Equal(3, values.Count());
        }

        [Fact]
        public async Task StartWithJsonResultTest()
        {

            ProbeMock result = await Probe.New()
                                       .Start<ProbeMock>($"-v quiet -show_streams \"{Resources.Mp4}\"");

            Assert.NotNull(result);
            Assert.NotNull(result.Streams);
        }

        private class ProbeMock
        {
            [JsonProperty(PropertyName = "streams")]
            public StreamMock[] Streams { get; set; }
        }

        private class StreamMock
        {
            public string codec_name { get; set; }
        }
    }
}
