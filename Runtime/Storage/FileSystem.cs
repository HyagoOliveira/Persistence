#if UNITY_WEBGL && !UNITY_EDITOR
#define RUNTIME_WEBGL
#endif

using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using ActionCode.AsyncIO;
using ActionCode.Cryptography;
using System.Collections.Generic;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Default implementation of a File System.
    /// </summary>
    public sealed class FileSystem
    {
        public ISerializer Serializer => serializer;

        public const string FOLDER = "Persistence";
        public const string COMPRESSED_EXTENSION = "sv";

        public static string DataPath => Application.persistentDataPath + "/" + FOLDER;

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
            var invalidName = string.IsNullOrEmpty(name);
            if (invalidName) throw new System.Exception($"Invalid file name: '{name}'");

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
        /// Tries to delete the file using the given name.
        /// It will delete both uncompressed and compressed files.
        /// </summary>
        /// <param name="name">The data file name without extension.</param>
        /// <returns>Whether the compressed file was deleted.</returns>
        public bool TryDelete(string name)
        {
            TryDelete(name, serializer.Extension); // Raw file does not exists in build
            var wasDeleted = TryDelete(name, COMPRESSED_EXTENSION);

            TryFlushChanges();

            return wasDeleted;
        }

        /// <summary>
        /// Tries to delete only the saved files inside the persistent folder.
        /// </summary>
        /// <returns>Whether the saved files were deleted.</returns>
        public bool TryDeleteAll()
        {
            foreach (var fileName in GetFileNames())
            {
                var wasDeleted = TryDelete(fileName);
                if (!wasDeleted) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns all file names (without extension) saved on the persistent folder.
        /// </summary>
        /// <returns>An enumerable list containing all file names.</returns>
        public IEnumerable<string> GetFileNames()
        {
            var hasInvalidPath = !Directory.Exists(DataPath);
            if (hasInvalidPath) yield break;

            var compressedFilePattern = $"*.{COMPRESSED_EXTENSION}";
            var allFiles = Directory.EnumerateFiles(DataPath, compressedFilePattern);
            foreach (var filePath in allFiles)
            {
                yield return Path.GetFileNameWithoutExtension(filePath);
            }
        }

        private bool TryDelete(string name, string extension)
        {
            var path = GetPath(name, extension);
            var invalidFilie = !File.Exists(path);

            if (invalidFilie) return false;

            File.Delete(path);
            return true;
        }

        private async Task<string> LoadContent(string name, bool useCompressedFile)
        {
            var extension = useCompressedFile ? COMPRESSED_EXTENSION : serializer.Extension;
            var path = GetPath(name, extension);
            var hasNoFile = !File.Exists(path);

            if (hasNoFile) return string.Empty;

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
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(DataPath + "/");
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