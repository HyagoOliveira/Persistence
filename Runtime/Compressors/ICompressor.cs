using UnityEngine;

namespace ActionCode.Persistence
{
    public interface ICompressor
    {
        /// <summary>
        /// Compress the given value.
        /// </summary>
        /// <param name="value">A string to compress.</param>
        /// <returns>An asynchronous operation of the compressing process.</returns>
        Awaitable<string> Compress(string value);

        /// <summary>
        /// Decompress the given value.
        /// </summary>
        /// <param name="value">A string to decompress.</param>
        /// <returns>An asynchronous operation of the decompressing process.</returns>
        Awaitable<string> Decompress(string value);
    }
}