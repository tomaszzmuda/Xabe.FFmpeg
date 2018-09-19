using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Allows to prepare and start IProbe.
    /// </summary>
    public interface IProbe
    {
        /// <summary>
        /// Start probe with JSON result
        /// </summary>
        /// <typeparam name="T">Type to return (deserialize from output)</typeparam>
        /// <param name="args">Args to pass to FFprobe</param>
        /// <returns>Deserialized output</returns>
        Task<T> Start<T>(string args);

        /// <summary>
        /// Start probe with raw result
        /// </summary>
        /// <param name="args">Args to pass to FFprobe</param>
        /// <returns>Raw output</returns>
        Task<string> Start(string args);
    }
}
