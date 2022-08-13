using ActionCode.AsyncIO;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Static factory class for <see cref="ICompressor"/> instances.
    /// </summary>
    public static class CompressorFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ICompressor"/> based on the given type.
        /// </summary>
        /// <param name="type">The Compressor type to use.</param>
        /// <returns>An Compressor implementation</returns>
        public static ICompressor Create(CompressorType type) =>
            Create(type, StreamFactory.Create());

        /// <summary>
        /// <inheritdoc cref="Create(CompressorType)"/>
        /// </summary>
        /// <param name="type"><inheritdoc cref="Create(CompressorType)"/></param>
        /// <param name="stream">The stream instance to use.</param>
        /// <returns><inheritdoc cref="Create(CompressorType)"/></returns>
        public static ICompressor Create(CompressorType type, IStream stream)
        {
            return type switch
            {
                CompressorType.None => null,
                CompressorType.GZip => new GZipCompressor(stream),
                _ => null
            };
        }
    }
}