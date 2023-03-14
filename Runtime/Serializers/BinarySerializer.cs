#if !UNITY_WSA || !UNITY_WINRT
#define BINARY_AVAILABLE
#endif

#if BINARY_AVAILABLE
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace ActionCode.Persistence
{
    public class BinarySerializer : ISerializer
    {
        public string Extension => "bin";

        public string SerializePretty<T>(T data) => Serialize(data);

#if BINARY_AVAILABLE
        public string Serialize<T>(T data)
        {
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, data);
            return Convert.ToBase64String(stream.ToArray());
        }

        public T Deserialize<T>(string value)
        {
            var formatter = new BinaryFormatter();
            var bytes = Convert.FromBase64String(value);
            using var ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
#else
        public string Serialize<T>(T _) => string.Empty;
        public T Deserialize<T>(string _) => default;
#endif

        public static bool IsAvailable()
        {
#if BINARY_AVAILABLE
            return true;
#else
            UnityEngine.Debug.LogError("Binary Serializer isn't supported on Windows Store or UWP.");
            return false;
#endif
        }
    }
}
