#if PACKAGE_UNITY_SERIALIZATION
using Unity.Serialization.Json;
#endif

namespace ActionCode.Persistence
{
    public sealed class UnityJsonSerialization : ISerializer
    {
        public string Extension => "json";

#if PACKAGE_UNITY_SERIALIZATION
        public string SerializePretty<T>(T data) => JsonSerialization.ToJson(data, new JsonSerializationParameters { Minified = false });
        public string Serialize<T>(T data) => JsonSerialization.ToJson(data, new JsonSerializationParameters { Minified = true });
        public T Deserialize<T>(string value) => JsonSerialization.FromJson<T>(value);
#else
        public string SerializePretty<T>(T data) => Serialize(data);
        public string Serialize<T>(T _) => string.Empty;
        public T Deserialize<T>(string _) => default;
#endif

        public static bool IsAvailable()
        {
#if PACKAGE_UNITY_SERIALIZATION
            return true;
#else
            UnityEngine.Debug.LogError("Unity Serialization package isn't installed. Install it using Package Manager.");
            return false;
#endif
        }
    }
}