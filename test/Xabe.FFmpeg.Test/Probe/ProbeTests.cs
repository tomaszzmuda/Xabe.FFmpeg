using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ProbeTests
    {
        [Fact]
        public async Task StartWithPktPtsTimeCsvResultTest()
        {
            var result = await Probe.New()
                 .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time,pict_type -of csv=print_section=0 {Resources.Mp4}");

            List<string> rows = result.Split('\n')
                                  .Select(x => x.Trim())
                                  .Where(x => x.Length > 0)
                                  .ToList();

            Assert.Equal(3, rows.Count);

            foreach (var row in rows)
            {
                List<string> cells = row.Split(',')
                                        .Select(x => x.Trim())
                                        .Where(x => x.Length > 0)
                                        .ToList();

                Assert.Contains("I", cells);
            }
        }

        [Fact]
        public async Task StartWithPtsTimeCsvResultTest()
        {
            var result = await Probe.New()
                 .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pts_time -of csv=print_section=0 {Resources.Mp4}");

            List<string> rows = result.Split('\n')
                                  .Select(x => x.Trim())
                                  .Where(x => x.Length > 0)
                                  .ToList();

            Assert.Equal(3, rows.Count);

            foreach (var row in rows)
            {
                Assert.True(double.TryParse(row, NumberStyles.Float, CultureInfo.InvariantCulture, out var time) && time >= 0);
            }
        }

        [Fact]
        public async Task StartWithStdOutputTest()
        {
            var result = await Probe.New()
                                       .Start($"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time {Resources.Mp4}");

            Assert.True(!string.IsNullOrEmpty(result));
        }
    }
}
