namespace ActionCode.Persistence
{
    public interface ISerializer
    {
        string Extension { get; }

        string Serialize<T>(T data);
        string SerializePretty<T>(T data);

        void Deserialize<T>(string value, ref T target);
    }
}
