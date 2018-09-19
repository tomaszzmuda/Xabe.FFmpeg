using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class Probe : IProbe
    {
        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IProbe object</returns>
        public static IProbe New()
        {
            return new Probe();
        }

        /// <inheritdoc />
        public async Task<T> Start<T>(string args)
        {
            var wrapper = new FFprobeWrapper();
            return await wrapper.Start<T>(args);
        }

        /// <inheritdoc />
        public async Task<string> Start(string args)
        {
            var wrapper = new FFprobeWrapper();
            return await wrapper.Start(args);
        }
    }
}
