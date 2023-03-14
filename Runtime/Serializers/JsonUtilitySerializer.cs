using UnityEngine;

namespace ActionCode.Persistence
{
    public class JsonUtilitySerializer : ISerializer
    {
        public string Extension => "json";

        public string SerializePretty<T>(T data) => JsonUtility.ToJson(data, prettyPrint: true);
        public string Serialize<T>(T data) => JsonUtility.ToJson(data);
        public T Deserialize<T>(string value) => JsonUtility.FromJson<T>(value);

        public static bool IsAvailable()
        {
#if MODULE_JSON_SERIALIZE
            return true;
#else
            Debug.LogError("Json Module isn't installed. Install it using Package Manager.");
            return false;
#endif
        }
    }
}
