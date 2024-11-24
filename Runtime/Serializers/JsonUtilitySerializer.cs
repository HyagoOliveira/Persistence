using UnityEngine;

namespace ActionCode.Persistence
{
    public class JsonUtilitySerializer : ISerializer
    {
        public string Extension => "json";

        public JsonUtilitySerializer()
        {
#if !MODULE_JSON_SERIALIZE
            throw new System.Exception("Unity Json Module isn't installed. Install it using Package Manager.");
#endif
        }

#if MODULE_JSON_SERIALIZE
        public string SerializePretty<T>(T data) => JsonUtility.ToJson(data, prettyPrint: true);
        public string Serialize<T>(T data) => JsonUtility.ToJson(data);
        public T Deserialize<T>(string value) => JsonUtility.FromJson<T>(value);
        public void Deserialize<T>(string value, ref T objectToOverride) =>
            JsonUtility.FromJsonOverwrite(value, objectToOverride);
#else
        public string SerializePretty<T>(T data) => default;
        public string Serialize<T>(T data) => default;
        public T Deserialize<T>(string value) => default;
        public void Deserialize<T>(string value, ref T objectToOverride) { }
#endif
    }
}
