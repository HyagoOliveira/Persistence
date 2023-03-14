using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using ActionCode.AsyncIO;
using ActionCode.Cryptography;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Default implementation of a File System.
    /// </summary>
    public sealed class FileSystem : IFileSystem
    {
        public static string DataPath => Application.persistentDataPath;

        private readonly IStream stream;
        private readonly ISerializer serializer;
        private readonly ICompressor compressor;
        private readonly ICryptographer cryptographer;

        public FileSystem(
            SerializerType serializerType,
            CompressorType compressorType,
            CryptographerType cryptographerType,
            string cryptographerKey
        ) :
            this(
                StreamFactory.Create(),
                SerializerFactory.Create(serializerType),
                CompressorFactory.Create(compressorType),
                CryptographerFactory.Create(cryptographerType, cryptographerKey)
            )
        { }

        public FileSystem(
            IStream stream,
            ISerializer serializer,
            ICompressor compressor,
            ICryptographer cryptographer
        )
        {
            this.stream = stream;
            this.serializer = serializer;
            this.compressor = compressor;
            this.cryptographer = cryptographer;
        }

        public async Task Save<T>(T data, string name, bool saveRawData)
        {
            var path = GetPath(name);

            if (saveRawData)
            {
                var prettyContent = serializer.SerializePretty(data);
                var rawPath = Path.ChangeExtension(path, serializer.Extension);

                await stream.Write(rawPath, prettyContent);
            }

            var content = serializer.Serialize(data);

            if (cryptographer != null) content = await cryptographer.Encrypt(content);
            if (compressor != null) content = await compressor.Compress(content);

            await stream.Write(path, content);
        }

        public async Task<T> Load<T>(string name)
        {
            var path = GetPath(name);
            var hasNoFile = !File.Exists(path);
            if (hasNoFile) return default;

            var content = await stream.Read(path);

            if (compressor != null) content = await compressor.Decompress(content);
            if (cryptographer != null) content = await cryptographer.Decrypt(content);

            return serializer.Deserialize<T>(content);
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
            const string extension = "sv";
            var path = Path.Combine(DataPath, name.Trim());
            return Path.ChangeExtension(path, extension);
        }
    }
}