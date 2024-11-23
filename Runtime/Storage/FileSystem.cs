#if UNITY_WEBGL && !UNITY_EDITOR
#define RUNTIME_WEBGL
#endif

using UnityEngine;
using System;
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

        private const string compressedExtension = "sv";

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
            CheckWhetherSerializable<T>("save");
            var path = GetPath(name, compressedExtension);

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

#if RUNTIME_WEBGL
            // Flushes the changes into the Browser IndexedDB.
            SyncDB();
#endif
        }

        public async Task<T> Load<T>(string name, bool useRawFile)
        {
            var extension = useRawFile ? serializer.Extension : compressedExtension;
            var path = GetPath(name, extension);
            return await LoadUsingPath<T>(path);
        }

        private async Task<T> LoadUsingPath<T>(string path)
        {
            CheckWhetherSerializable<T>("deserialize");
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

        private static string GetPath(string name, string extension)
        {
            var path = Path.Combine(DataPath, name.Trim());
            return Path.ChangeExtension(path, extension);
        }

        private static void CheckWhetherSerializable<T>(string action)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException(
                    $"Tried to {action} non-serializable type {typeof(T).Name}. " +
                    $"Add the [Serializable] attribute into your class."
                );
        }

#if RUNTIME_WEBGL
        // The function is present on /Plugins/WebGL/IndexedDB_Flusher.jslib file
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void SyncDB(); 
#endif
    }
}