using System;
using System.IO;
using Xabe.FFmpeg;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class JsonDocumentTests
    {
        [Fact]
        public void ParsePreservesMemberOrderAndShapes()
        {
            var doc = JsonDocument.Parse("{\"b\":2,\"a\":\"x\",\"c\":[true,null],\"d\":-1.5e3}", "unit");

            Assert.Equal(JsonValueType.Object, doc.Type);
            Assert.Equal(4, doc.Members.Count);
            Assert.Equal("b", doc.Members[0].Name);
            Assert.Equal("a", doc.Members[1].Name);
            Assert.Equal("c", doc.Members[2].Name);

            Assert.Equal(JsonValueType.Number, doc.Member("b").Type);
            Assert.Equal(JsonValueType.String, doc.Member("a").Type);
            var array = doc.Member("c");
            Assert.Equal(JsonValueType.Array, array.Type);
            Assert.Equal(JsonValueType.Boolean, array.Elements[0].Type);
            Assert.Equal(JsonValueType.Null, array.Elements[1].Type);
            Assert.Equal("-1.5e3", doc.Member("d").Scalar);
        }

        [Fact]
        public void ParseHandlesEscapesWhitespaceAndQuotedNumbers()
        {
            var doc = JsonDocument.Parse("  {\n  \"name\" : \"a\\nb\\tc\\\"d\\\\e\\/f\\u00e9\\ud83d\\ude00\",\n  \"rate\" : \"48000\",\n  \"idx\" : 1\n}", "unit");

            Assert.Equal("a\nb\tc\"d\\e/f\u00e9\ud83d\ude00", doc.Member("name").Scalar);
            Assert.Equal(JsonValueType.String, doc.Member("rate").Type);
            Assert.Equal(48000, doc.GetInt("rate"));
            Assert.Equal(1, doc.GetInt("idx"));
        }

        [Theory]
        [InlineData("42", "streams")]
        [InlineData("[1,2]", "streams")]
        [InlineData("null", "streams")]
        [InlineData("\"root\"", "streams")]
        [InlineData("truex", "streams")]
        [InlineData("-", "streams")]
        [InlineData("", "streams")]
        [InlineData("nul", "streams")]
        [InlineData("{\"streams\":42}", "streams")]
        [InlineData("{\"streams\":[42]}", "streams")]
        [InlineData("{\"streams\":[\"x\"]}", "streams")]
        [InlineData("{\"streams\":[{\"codec_name\":42}]}", "streams")]
        [InlineData("{\"streams\":[{\"tags\":\"hello\"}]}", "streams")]
        [InlineData("{\"streams\":[{\"disposition\":[]}]}", "streams")]
        [InlineData("{\"streams\":[{\"width\":null}]}", "streams")]
        [InlineData("{\"streams\":[{\"duration\":null}]}", "streams")]
        [InlineData("{\"streams\":[{\"index\":\"-1.5\"}]}", "streams")]
        [InlineData("{\"streams\":[{\"duration\":\"fast\"}]}", "streams")]
        [InlineData("{\"streams\":[{\"nb_frames\":7}]}", "streams")]
        [InlineData("{\"format\":\"x\"}", "format")]
        [InlineData("{\"format\":{\"bit_rate\":null}}", "format")]
        
        [InlineData("{\"format\":{\"duration\":1.0,\"bit_rate\":\"loud\"}}", "format")]
        public void MalformedStructureSurfacesAsInvalidDataWithoutEchoingPayload(string payload, string kind)
        {
            Func<string, object> project = kind == "streams"
                ? json => JsonDocument.Map(json, "unit-label", ProbeModel.GetStreams)
                : json => JsonDocument.Map(json, "unit-label", FormatModel.Root.FromJson);

            var ex = Assert.Throws<InvalidDataException>(() => project(payload));
            var inner = Assert.IsType<JsonFormatException>(ex.InnerException);
            Assert.Contains("unit-label", ex.Message);
            Assert.DoesNotContain("SECRETX", ex.Message);
            Assert.NotEmpty(inner.Message);
        }

        [Fact]
        public void MalformedSyntaxReportsPositionNotContent()
        {
            const string payload = "{\"streams\":[{\"codec_name\":\"SEC\\u004FRETX\"}";
            var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(payload, "unit-label", ProbeModel.GetStreams));
            Assert.Contains("unit-label", ex.Message);
            Assert.DoesNotContain("SECRETX", ex.Message);
        }

        [Fact]
        public void DepthBeyondCapIsRejected()
        {
            using (var writer = new StringWriter())
            {
                writer.Write("{\"streams\":[");
                for (var i = 0; i < 80; i++)
                {
                    writer.Write("{\"d\":");
                }

                writer.Write("1");
                for (var i = 0; i < 80; i++)
                {
                    writer.Write("}");
                }

                writer.Write("]}");
                var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(writer.ToString(), "unit", ProbeModel.GetStreams));
                Assert.Contains("unit", ex.Message);
            }
        }

        [Fact]
        public void TrailingGarbageIsRejected()
        {
            const string payload = "{\"streams\":[]} trailing";
            var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(payload, "unit", ProbeModel.GetStreams));
            Assert.Contains("unit", ex.Message);
        }

        [Fact]
        public void RawControlCharactersInsideStringsAreRejected()
        {
            const string payload = "{\"streams\":[{\"codec_name\":\"a\nb\"}]}";
            var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(payload, "unit", ProbeModel.GetStreams));
            Assert.Contains("unit", ex.Message);
        }

        [Fact]
        public void StreamsMapsRealisticFlvShape()
        {
            const string payload = "{\"streams\":[{\"index\":0,\"codec_name\":\"aac\",\"codec_type\":\"audio\",\"sample_rate\":\"48000\",\"channel_layout\":\"mono\",\"bits_per_sample\":16,\"initial_padding\":0,\"sample_fmt\":\"fltp\",\"padd\":0,\"extradata_size\":2,\"disposition\":{\"default\":0,\"forced\":0,\"hearing_impaired\":0,\"visual_impaired\":0,\"carried_by_video\":0},\"tags\":{\"language\":\"und\",\"encoder\":\"Lavf58.29.100\",\"handler_name\":\"SoundHandler\"}},{\"index\":1,\"codec_name\":\"flv1\",\"profile\":\"Main\",\"codec_type\":\"video\",\"width\":1280,\"height\":720,\"coded_width\":1280,\"coded_height\":720,\"has_b_frames\":0,\"pix_fmt\":\"yuv420p\",\"level\":30,\"color_range\":\"tv\",\"r_frame_rate\":\"25/1\",\"avg_frame_rate\":\"25/1\",\"time_base\":\"1/1250000\",\"disposition\":{\"default\":0,\"forced\":0},\"tags\":{\"language\":\"und\",\"handler_name\":\"VideoHandler\"}}],\"format\":{\"filename\":\"in.flv\",\"nb_streams\":2}}";

            var streams = JsonDocument.Map(payload, "unit", ProbeModel.GetStreams);

            Assert.Equal(2, streams.Length);
            Assert.Equal("aac", streams[0].CodecName);
            Assert.Equal(48000, streams[0].SampleRate);
            Assert.Equal(0, streams[0].Index);
            Assert.Equal("und", streams[0].Tags.Language);
            Assert.Null(streams[0].Tags.Title);
            Assert.Null(streams[0].Tags.Rotate);
            Assert.Equal(0, streams[0].Disposition.Default);
            Assert.Equal("yuv420p", streams[1].PixFmt);
            Assert.Equal(1280, streams[1].Width);
            Assert.Equal("25/1", streams[1].RFrameRate);
        }

        [Fact]
        public void FormatMapsQuotedAndBareNumbers()
        {
            var quoted = JsonDocument.Map("{\"format\":{\"filename\":\"in.ts\",\"nb_streams\":1,\"start_time\":\"1502.272000\",\"duration\":\"0.040000\",\"size\":\"105082\",\"bit_rate\":\"209923\",\"probe_score\":33}}", "unit", FormatModel.Root.FromJson);
            Assert.Equal("105082", quoted.format.size);
            Assert.Equal(209923, quoted.format.bit_Rate);
            Assert.Equal(0.04d, quoted.format.duration, 3);

            var bare = JsonDocument.Map("{\"format\":{\"size\":\"100\",\"bit_rate\":5000,\"duration\":2.5}}", "unit", FormatModel.Root.FromJson);
            Assert.Equal(5000, bare.format.bit_Rate);
            Assert.Equal(2.5d, bare.format.duration, 3);
        }

        [Fact]
        public void GettersTolerateAbsenceAndExplicitNullForNullableTargets()
        {
            var doc = JsonDocument.Parse("{\"a\":null,\"b\":\"x\"}", "unit");
            Assert.Null(doc.GetString("a"));
            Assert.Null(doc.GetInt("a"));
            Assert.Null(doc.Member("missing"));
            Assert.Equal("x", doc.GetString("b"));
        }

        [Fact]
        public void NullRootIsNotObjectShape()
        {
            var doc = JsonDocument.Parse("null", "unit");
            Assert.Equal(JsonValueType.Null, doc.Type);
            var ex = Assert.Throws<JsonFormatException>(() => ProbeModel.GetStreams(doc));
            Assert.Contains("object", ex.Message);
        }
    }
}
