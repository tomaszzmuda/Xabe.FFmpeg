using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Allows to prepare and start IProbe.
    /// </summary>
    public interface IProbe
    {
        /// <summary>
        /// Start probe with result from console
        /// </summary>
        /// <param name="args">Args to pass to FFprobe</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Output from the ffprobe console in the requested format. Available fields depend on the ffprobe version.</returns>
        Task<string> Start(string args, CancellationToken cancellationToken = default);
    }
}
