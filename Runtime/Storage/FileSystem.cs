#if UNITY_WEBGL && !UNITY_EDITOR
#define RUNTIME_WEBGL
#endif

using System.IO;
using System.Collections.Generic;
using ActionCode.Cryptography;
using UnityEngine;

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

        public static string DataPath => Path.Combine(Application.persistentDataPath, FOLDER);

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
                SerializerFactory.Create(serializerType),
                CompressorFactory.Create(compressorType),
                CryptographerFactory.Create(cryptographerType, cryptographerKey)
            )
        { }

        public FileSystem(
            ISerializer serializer,
            ICompressor compressor,
            ICryptographer cryptographer
        )
        {
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
        /// <returns>An asynchronous operation of the saving process.</returns>
        public async Awaitable Save<T>(T data, string name, bool saveRawData)
        {
            var invalidName = string.IsNullOrEmpty(name);
            if (invalidName) throw new System.Exception($"Invalid file name: '{name}'");

            var path = GetPath(name, COMPRESSED_EXTENSION);

            if (saveRawData)
            {
                var prettyContent = serializer.SerializePretty(data);
                var rawPath = Path.ChangeExtension(path, serializer.Extension);

                await WriteAsync(rawPath, prettyContent);
            }

            var content = serializer.Serialize(data);

            if (cryptographer != null) content = await cryptographer.EncryptAsync(content);
            if (compressor != null) content = await compressor.CompressAsync(content);

            await WriteAsync(path, content);

            TryFlushChanges();
        }

        /// <summary>
        /// Tries to load the generic data using the given name.
        /// </summary>
        /// <typeparam name="T">The generic data type to load.</typeparam>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="target">The target data to load.</param>
        /// <param name="useCompressedFile">Whether to use the compressed/encrypted file.</param>
        /// <returns>A asynchronous operation of the loading process.</returns>
        public async Awaitable<bool> TryLoad<T>(string name, T target, bool useCompressedFile)
        {
            var content = await LoadContent(name, useCompressedFile);
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent) serializer.Deserialize(content, ref target);

            return hasContent;
        }

        /// <summary>
        /// Deletes the file using the given name.
        /// </summary>
        /// <remarks>
        /// It will delete both uncompressed and compressed files.
        /// </remarks>
        /// <param name="name">The data file name without extension.</param>
        public void Delete(string name)
        {
            Delete(name, serializer.Extension); // Raw file does not exists in build
            Delete(name, COMPRESSED_EXTENSION);

            TryFlushChanges();
        }

        /// <summary>
        /// Deletes all the saved files inside the persistent folder.
        /// </summary>
        public void DeleteAll()
        {
            foreach (var fileName in GetFileNames())
            {
                Delete(fileName);
            }
        }

        /// <summary>
        /// Returns all file names (without extension) saved on the persistent folder.
        /// </summary>
        /// <returns>An enumerable list containing all file names.</returns>
        public static IEnumerable<string> GetFileNames()
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

        private static void Delete(string name, string extension)
        {
            var path = GetPath(name, extension);
            var isValidFilie = File.Exists(path);
            if (isValidFilie) File.Delete(path);
        }

        private async Awaitable<string> LoadContent(string name, bool useCompressedFile)
        {
            var extension = useCompressedFile ? COMPRESSED_EXTENSION : serializer.Extension;
            var path = GetPath(name, extension);
            var hasNoFile = !File.Exists(path);

            if (hasNoFile) return string.Empty;

            var content = await ReadAsync(path);

            Debug.Log("Content " + content);

            if (useCompressedFile)
            {
                content = await compressor.DecompressAsync(content);
                content = await cryptographer.DecryptAsync(content);
            }

            return content;
        }

        public static void OpenSaveFolder()
        {
            CheckDataPath();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(DataPath + "/");
#endif
        }

        public static void Open(string name, string extension)
        {
            CheckDataPath();

            var path = GetPath(name, extension);
            var hasNoFile = !File.Exists(path);
            if (hasNoFile)
            {
                Debug.LogError($"File '{path}' does not exist.");
                return;
            }

            System.Diagnostics.Process.Start(path);
        }

        public static async Awaitable WriteAsync(string path, string content)
        {
            await using var writer = new StreamWriter(path);
            await writer.WriteAsync(content);
        }

        public static async Awaitable WriteAsync(Stream stream, byte[] bytes) =>
            await stream.WriteAsync(bytes, 0, bytes.Length);

        public static async Awaitable<string> ReadAsync(string path)
        {
            using var reader = new StreamReader(path);
            return await reader.ReadToEndAsync();
        }

        public static async Awaitable<string> ReadAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private static void CheckDataPath()
        {
            var hasInvalidDataPath = !Directory.Exists(DataPath);
            if (hasInvalidDataPath) Directory.CreateDirectory(DataPath);
        }

        private static string GetPath(string name, string extension)
        {
            var path = Path.Combine(DataPath, name.Trim());
            return Path.ChangeExtension(path, extension);
        }

        private static void TryFlushChanges()
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