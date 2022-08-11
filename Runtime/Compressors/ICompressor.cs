using System.Threading.Tasks;

namespace ActionCode.Persistence
{
    public interface ICompressor
    {
        /// <summary>
        /// Compress the given value.
        /// </summary>
        /// <param name="value">A string to compress.</param>
        /// <returns>A task operation of the compress process.</returns>
        Task<string> Compress(string value);

        string Decompress(string value);
    }
}