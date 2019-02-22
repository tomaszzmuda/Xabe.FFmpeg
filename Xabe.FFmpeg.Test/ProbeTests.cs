using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ProbeTests
    {
        [Fact]
        public async Task StartWithCsvResultTest()
        {
            string result = await Probe.New()
                 .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time -of csv=print_section=0 {Resources.Mp4}");

            IEnumerable<string> values = result.Split('\n')
                               .Where(x => !string.IsNullOrEmpty(x));

            Assert.Equal(3, values.Count());
        }

        [Fact]
        public async Task StartWithStdOutputTest()
        {
            string result = await Probe.New()
                                       .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time {Resources.Mp4}");

            Assert.True(!string.IsNullOrEmpty(result));
        }
    }
}
