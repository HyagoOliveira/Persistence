using System.Threading.Tasks;

namespace ActionCode.Persistence
{
    public interface ICompressor
    {
        /// <summary>
        /// Compress the given value.
        /// </summary>
        /// <param name="value">A string to compress.</param>
        /// <returns>A task operation of the compressing process.</returns>
        Task<string> Compress(string value);

        /// <summary>
        /// Decompress the given value.
        /// </summary>
        /// <param name="value">A string to decompress.</param>
        /// <returns>A task operation of the decompressing process.</returns>
        Task<string> Decompress(string value);
    }
}