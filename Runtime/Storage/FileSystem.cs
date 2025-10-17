#if UNITY_WEBGL && !UNITY_EDITOR
#define RUNTIME_WEBGL
#endif

using System.IO;
using System.Collections.Generic;
using ActionCode.AsyncIO;
using ActionCode.Cryptography;
using UnityEngine;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Default implementation of a File System.
    /// </summary>
    public sealed class FileSystem
    {
        /// <summary>
        /// The serializer used by the File System.
        /// </summary>
        public ISerializer Serializer => serializer;

        /// <summary>
        /// The folder name used by the File System.
        /// </summary>
        public const string FOLDER = "Persistence";

        /// <summary>
        /// The compressed file extension used by the File System.
        /// </summary>
        public const string COMPRESSED_EXTENSION = "sv";

        /// <summary>
        /// The persistent data path used by the File System.
        /// </summary>
        public static string DataPath => Path.Combine(Application.persistentDataPath, FOLDER);

        private readonly IStream stream;
        private readonly ISerializer serializer;
        private readonly ICompressor compressor;
        private readonly ICryptographer cryptographer;

        /// <summary>
        /// Creates a File System using the given params.
        /// </summary>
        /// <param name="stream">The Stream implementation used.</param>
        /// <param name="serializer">The Serializer implementation used.</param>
        /// <param name="compressor">The Compressor implementation used.</param>
        /// <param name="cryptographer">The Cryptographer implementation used.</param>
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
        /// Creates a File System using the given params.
        /// </summary>
        /// <param name="serializerType">The serializer type to use.</param>
        /// <param name="compressorType">The compressor type to use.</param>
        /// <param name="cryptographerType">The cryptographer type to use.</param>
        /// <param name="cryptographerKey">The key used by the cryptographer to use.</param>
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

        /// <summary>
        /// Saves the given data using the name. 
        /// </summary>
        /// <typeparam name="T">The generic data type to save.</typeparam>
        /// <param name="data">The data instance.</param>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="savePrettyData">
        /// Whether to save an additional copy of data without any compression or cryptography.
        /// </param>
        /// <returns>An asynchronous save operation.</returns>
        public async Awaitable SaveAsync<T>(T data, string name, bool savePrettyData)
        {
            var invalidName = string.IsNullOrEmpty(name);
            if (invalidName) throw new System.Exception($"Invalid file name: '{name}'");

            CheckDataPath();
            var path = GetPath(name, COMPRESSED_EXTENSION);

            if (savePrettyData)
            {
                var prettyContent = serializer.SerializePretty(data);
                var rawPath = Path.ChangeExtension(path, serializer.Extension);

                await stream.WriteAsync(rawPath, prettyContent);
            }

            var content = serializer.Serialize(data);

            if (cryptographer != null) content = await cryptographer.EncryptAsync(content);
            if (compressor != null) content = await compressor.CompressAsync(content);

            await stream.WriteAsync(path, content);

            TryFlushBrowserDatabase();
        }

        /// <summary>
        /// Tries to load the generic data using the given name.
        /// </summary>
        /// <typeparam name="T">The generic data type to load.</typeparam>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="target">The target data to load.</param>
        /// <param name="useCompressedFile">Whether to use the compressed/encrypted file.</param>
        /// <returns>An asynchronous load operation.</returns>
        public async Awaitable<bool> TryLoadAsync<T>(string name, T target, bool useCompressedFile)
        {
            var content = await LoadContentAsync(name, useCompressedFile);
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent) serializer.Deserialize(content, ref target);

            return hasContent;
        }

        /// <summary>
        /// Tries to load the generic data from the given path.
        /// </summary>
        /// <typeparam name="T">The generic data type to load.</typeparam>
        /// <param name="path">The path where the data is.</param>
        /// <param name="target">The target data to load.</param>
        /// <returns><inheritdoc cref="TryLoadAsync{T}(string, T, bool)"/></returns>
        public async Awaitable<bool> TryLoadAsync<T>(string path, T target)
        {
            var content = await LoadContentAsync(path);
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent) serializer.Deserialize(content, ref target);

            return hasContent;
        }

        /// <summary>
        /// Loads the data content using the given name.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="useCompressedFile"><inheritdoc cref="TryLoadAsync{T}(string, T, bool)" path="/param[@name='useCompressedFile']"/></param>
        /// <returns><inheritdoc cref="TryLoadAsync{T}(string, T, bool)"/></returns>
        public async Awaitable<string> LoadContentAsync(string name, bool useCompressedFile)
        {
            var extension = useCompressedFile ? COMPRESSED_EXTENSION : serializer.Extension;
            var path = GetPath(name, extension);
            return await LoadContentAsync(path);
        }

        /// <summary>
        /// Loads the data content using the given path.
        /// </summary>
        /// <param name="path"><inheritdoc cref="TryLoadAsync{T}(string, T)" path="/param[@name='path']"/></param>
        /// <returns><inheritdoc cref="TryLoadAsync{T}(string, T, bool)"/></returns>
        public async Awaitable<string> LoadContentAsync(string path)
        {
            var hasNoFile = !File.Exists(path);
            if (hasNoFile) return string.Empty;

            var extension = Path.GetExtension(path).Replace(".", "");
            var content = await stream.ReadAsync(path);
            var isCompressed = extension == COMPRESSED_EXTENSION;

            if (isCompressed)
            {
                content = await compressor.DecompressAsync(content);
                content = await cryptographer.DecryptAsync(content);
            }

            return content;
        }

        /// <summary>
        /// Loads the compressed/cryptographed data stream using the given name.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <returns><inheritdoc cref="TryLoadAsync{T}(string, T, bool)"/></returns>
        public Stream LoadStream(string name)
        {
            var path = GetPath(name, COMPRESSED_EXTENSION);
            var hasFile = File.Exists(path);
            return hasFile ? new StreamReader(path).BaseStream : null;
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
            Delete(name, serializer.Extension); // Pretty file does not exists in build
            Delete(name, COMPRESSED_EXTENSION);
            TryFlushBrowserDatabase();
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

        /// <summary>
        /// Deletes the given file using the name and extension.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="extension">The file extension.</param>
        public static void Delete(string name, string extension)
        {
            var path = GetPath(name, extension);
            var isValidFilie = File.Exists(path);
            if (isValidFilie) File.Delete(path);
        }

        /// <summary>
        /// Opens the persistent data folder in the file explorer.
        /// </summary>
        public static void OpenSaveFolder()
        {
            CheckDataPath();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(DataPath + "/");
#endif
        }

        /// <summary>
        /// Opens the file using the given name and extension.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="extension"><inheritdoc cref="Delete(string, string)" path="/param[@name='extension']"/></param>
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

        /// <summary>
        /// Checks if the data path exists. If not, creates it.
        /// </summary>
        public static void CheckDataPath()
        {
            var hasInvalidDataPath = !Directory.Exists(DataPath);
            if (hasInvalidDataPath) Directory.CreateDirectory(DataPath);
        }

        /// <summary>
        /// Gets the full path using the given name and extension.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="extension"><inheritdoc cref="Delete(string, string)" path="/param[@name='extension']"/></param>
        /// <returns>The path using the extension.</returns>
        public static string GetPath(string name, string extension)
        {
            var path = Path.Combine(DataPath, name.Trim());
            return Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// Tries to persist the Browser database changes into the disk. 
        /// Only works for WebGL builds.
        /// </summary>
        public static void TryFlushBrowserDatabase()
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