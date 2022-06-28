namespace ActionCode.Persistence
{
    public interface ISerializer
    {
        string Extension { get; }

        string Serialize<T>(T data);
        T Deserialize<T>(string value);
    }
}
