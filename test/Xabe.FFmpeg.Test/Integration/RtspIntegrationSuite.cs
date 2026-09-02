using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test.Integration
{
    /// <summary>
    ///     Groups all RTSP integration tests behind one shared <see cref="RtspServerFixture"/> and keeps them from
    ///     running in parallel with each other (or with any other collection).
    ///     xUnit only discovers collection definitions in the entry test assembly, hence this lives next to the tests.
    /// </summary>
    [CollectionDefinition(CollectionName, DisableParallelization = true)]
    public sealed class RtspIntegrationSuite : ICollectionFixture<RtspServerFixture>
    {
        public const string CollectionName = "XabeFfmpeg.Rtsp";
    }
}
