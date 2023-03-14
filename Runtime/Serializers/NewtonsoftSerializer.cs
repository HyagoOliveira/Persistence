#if PACKAGE_NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace ActionCode.Persistence
{
    public sealed class NewtonsoftSerializer : ISerializer
    {
        public string Extension => "json";

#if PACKAGE_NEWTONSOFT
        public string Serialize<T>(T data) => JsonConvert.SerializeObject(data);
        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);
#else
        public string Serialize<T>(T _) => string.Empty;
        public T Deserialize<T>(string _) => default;
#endif

        public static bool IsAvailable()
        {
#if PACKAGE_NEWTONSOFT
            return true;
#else
            Debug.LogError("Newtonsoft Json package isn't installed. Install it using Package Manager.");
            return false;
#endif
        }
    }
}