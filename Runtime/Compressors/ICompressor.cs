namespace ActionCode.Persistence
{
    public interface ICompressor
    {
        string Compress(string value);
        string Decompress(string value);
    }
}