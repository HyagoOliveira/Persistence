namespace ActionCode.Persistence
{
    /// <summary>
    /// Factory class for <see="ICompressor"/> instances.
    /// </summary>
    public static class CompressorFactory
    {
        public static ICompressor Create(CompressorType type)
        {
            return type switch
            {
                CompressorType.None => null,
                CompressorType.GZip => new GZipCompressor(),
                _ => null
            };
        }
    }
}