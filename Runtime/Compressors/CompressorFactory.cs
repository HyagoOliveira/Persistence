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
        /// <returns>A Compressor implementation.</returns>
        public static ICompressor Create(CompressorType type) => type switch
        {
            CompressorType.None => null,
            CompressorType.GZip => new GZipCompressor(),
            _ => null
        };
    }
}