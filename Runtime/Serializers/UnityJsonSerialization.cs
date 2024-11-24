#if PACKAGE_UNITY_SERIALIZATION
using Unity.Serialization.Json;
#endif

namespace ActionCode.Persistence
{
    public sealed class UnityJsonSerialization : ISerializer
    {
        public string Extension => "json";

        public UnityJsonSerialization()
        {
#if !PACKAGE_UNITY_SERIALIZATION
            throw new System.Exception("Unity Serialization package isn't installed. Install it using Package Manager.");
#endif
        }

#if PACKAGE_UNITY_SERIALIZATION
        public string SerializePretty<T>(T data) => JsonSerialization.ToJson(data, new JsonSerializationParameters { Minified = false });
        public string Serialize<T>(T data) => JsonSerialization.ToJson(data, new JsonSerializationParameters { Minified = true });
        public T Deserialize<T>(string value) => JsonSerialization.FromJson<T>(value);
        public void Deserialize<T>(string value, ref T objectToOverride) =>
            JsonSerialization.FromJsonOverride(value, ref objectToOverride);
#else
        public string SerializePretty<T>(T data) => Serialize(data);
        public string Serialize<T>(T _) => string.Empty;
        public T Deserialize<T>(string _) => default;
        public void Deserialize<T>(string value, ref T objectToOverride) { }
#endif
    }
}