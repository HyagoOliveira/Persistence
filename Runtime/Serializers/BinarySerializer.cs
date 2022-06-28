#if !UNITY_WSA || !UNITY_WINRT
#define BINARY_AVAILABLE
#endif
using System;
using System.IO;
#if BINARY_AVAILABLE
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace ActionCode.Persistence
{
    public class BinarySerializer : ISerializer
    {
        public string Extension => "bin";

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
