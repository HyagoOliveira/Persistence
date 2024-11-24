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
        public void Deserialize<T>(string value, ref T target) => JsonUtility.FromJsonOverwrite(value, target);
#else
        public string SerializePretty<T>(T _) => default;
        public string Serialize<T>(T _) => default;
        public void Deserialize<T>(string _, ref T __) { }
#endif
    }
}
