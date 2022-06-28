using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Default implementation of a File System.
    /// </summary>
    public sealed class FileSystem : IFileSystem
    {
        public static string DataPath => Application.persistentDataPath;

        private const string EXTENSION = "sv";

        private readonly ISerializer serializer;
        private readonly ICompressor compressor;
        private readonly ICryptographer cryptographer;

        public FileSystem(
            SerializerType serializerType,
            CompressorType compressorType,
            CryptographerType cryptographerType,
            string cryptographerKey
        )
        {
            serializer = SerializerFactory.Create(serializerType);
            compressor = CompressorFactory.Create(compressorType);
            cryptographer = CryptographerFactory.Create(cryptographerType, cryptographerKey);
        }

        public void Save<T>(T data, string name)
        {
            var path = GetPath(name);
            var content = serializer.Serialize(data);

            if (cryptographer != null) content = cryptographer.Encrypt(content);
            if (compressor != null) content = compressor.Compress(content);

            using var writer = new StreamWriter(path);
            writer.Write(content);
        }

        public void SaveUncompressed<T>(T data, string name)
        {
            var path = GetPath(name);
            var content = serializer.Serialize(data);

            path = Path.ChangeExtension(path, serializer.Extension);

            using var writer = new StreamWriter(path);
            writer.Write(content);
        }

        public bool TryLoad<T>(string name, out T data)
        {
            data = default;
            var path = GetPath(name);
            var hasData = File.Exists(path);

            if (!hasData) return false;

            using var reader = new StreamReader(path);
            var content = reader.ReadToEnd();

            if (compressor != null) content = compressor.Decompress(content);
            if (cryptographer != null) content = cryptographer.Decrypt(content);

            data = serializer.Deserialize<T>(content);

            return hasData;
        }

        public static void OpenSaveFolder()
        {
#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", "/select, " + DataPath);
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            Process.Start("open", $"-R \"{DataPath}\"");
#endif
        }

        private static string GetPath(string name)
        {
            var path = Path.Combine(DataPath, name.Trim());
            return Path.ChangeExtension(path, EXTENSION);
        }
    }
}