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
        public void Deserialize<T>(string value, ref T target) => JsonSerialization.FromJsonOverride(value, ref target);
#else
        public string SerializePretty<T>(T data) => Serialize(data);
        public string Serialize<T>(T _) => string.Empty;
        public void Deserialize<T>(string _, ref T __) { }
#endif
    }
}