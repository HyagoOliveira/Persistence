#if UNITY_WEBGL && !UNITY_EDITOR
#define RUNTIME_WEBGL
#endif

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
    public sealed class FileSystem
    {
        public const string COMPRESSED_EXTENSION = "sv";

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

        /// <summary>
        /// Saves the given data using the name. 
        /// </summary>
        /// <typeparam name="T">The data generic type.</typeparam>
        /// <param name="data">The data instance.</param>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="saveRawData">Whether to save an additional copy of data without any compression or cryptography.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Task Save<T>(T data, string name, bool saveRawData)
        {
            var path = GetPath(name, COMPRESSED_EXTENSION);

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
            TryFlushChanges();
        }

        /// <summary>
        /// Tries to load the generic data using the given name.
        /// </summary>
        /// <typeparam name="T">The generic data type to load.</typeparam>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="target">The target data to load.</param>
        /// <param name="useCompressedFile">Whether to use the compressed/encrypted file.</param>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<bool> TryLoad<T>(string name, T target, bool useCompressedFile)
        {
            var content = await LoadContent(name, useCompressedFile);
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent) serializer.Deserialize(content, ref target);

            return hasContent;
        }

        /// <summary>
        /// Deletes the file using the given name.
        /// It will delete both uncompressed and compressed files.
        /// </summary>
        /// <param name="name">The data file name without extension.</param>
        public void Delete(string name)
        {
            var compressedPath = GetPath(name, COMPRESSED_EXTENSION);
            var uncompressedPath = GetPath(name, serializer.Extension);

            TryDelete(compressedPath);
            TryDelete(uncompressedPath);

            TryFlushChanges();
        }

        private void TryDelete(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        private async Task<string> LoadContent(string name, bool useCompressedFile)
        {
            var extension = useCompressedFile ? COMPRESSED_EXTENSION : serializer.Extension;
            var path = GetPath(name, extension);
            var hasNoFile = !File.Exists(path);

            if (hasNoFile) throw new FileNotFoundException($"File {path} does not exists");

            var content = await stream.Read(path);

            if (useCompressedFile)
            {
                content = await compressor.Decompress(content);
                content = await cryptographer.Decrypt(content);
            }

            return content;
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

        private void TryFlushChanges()
        {
#if RUNTIME_WEBGL
            // Flushes the changes into the Browser IndexedDB.
            SyncDB();
#endif
        }

#if RUNTIME_WEBGL
        // The function is present on /Plugins/WebGL/IndexedDB_Flusher.jslib file
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void SyncDB(); 
#endif
    }
}