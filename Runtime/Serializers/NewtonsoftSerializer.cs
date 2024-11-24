#if PACKAGE_NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace ActionCode.Persistence
{
    public sealed class NewtonsoftSerializer : ISerializer
    {
        public string Extension => "json";

        public NewtonsoftSerializer()
        {
#if !PACKAGE_NEWTONSOFT
            throw new System.Exception("Newtonsoft Json Module isn't installed. Install it using Package Manager.");
#endif
        }

#if PACKAGE_NEWTONSOFT
        public string SerializePretty<T>(T data) => JsonConvert.SerializeObject(data, Formatting.Indented);
        public string Serialize<T>(T data) => JsonConvert.SerializeObject(data);
        public void Deserialize<T>(string value, ref T target) => JsonConvert.PopulateObject(value, target);
#else
        public string SerializePretty<T>(T data) => Serialize(data);
        public string Serialize<T>(T _) => string.Empty;
        public void Deserialize<T>(string _, ref T __) { }
#endif
    }
}