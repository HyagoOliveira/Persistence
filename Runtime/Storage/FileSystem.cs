using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public async Task Save<T>(T data, string name, bool saveRawData)
        {
            var path = GetPath(name);
            var content = serializer.Serialize(data);

            if (saveRawData)
            {
                var rawPath = Path.ChangeExtension(path, serializer.Extension);
                await SynchronyStreamAdapter.Write(rawPath, content);
            }

            if (cryptographer != null) content = await cryptographer.Encrypt(content);
            if (compressor != null) content = await compressor.Compress(content);

            await SynchronyStreamAdapter.Write(path, content);
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
            var path = DataPath.Replace(@"/", @"\") + @"\";
            Process.Start("explorer.exe", "/select, " + path);
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