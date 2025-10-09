using UnityEngine;
using ActionCode.Cryptography;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Persist generic data using encryption and compression.
    /// </summary>
    [CreateAssetMenu(fileName = "PersistenceSettings", menuName = "ActionCode/Persistence Settings", order = 110)]
    public sealed class PersistenceSettings : ScriptableObject
    {
        [Tooltip("The Serializer type to use.")]
        public SerializerType serializer;
        [Tooltip("The Compressor type to use.")]
        public CompressorType compressor;

        [Header("Cryptography")]
        [Tooltip("The Cryptographer type to use.")]
        public CryptographerType cryptographer;
        [Tooltip("The cryptographer key to use.")]
        public string cryptographerKey = "H2h2xZe83AX90788QNqJXRiWX88xWI2b";

        /// <summary>
        /// Saves the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="name">The data name to use.</param>
        /// <param name="saveRawFile">
        /// Whether to save an additional copy of data without any compression or cryptography, in human readable format. 
        /// This is useful for debugging purposes.
        /// </param>
        /// <returns>An asynchronous operation of the saving process.</returns>
        public async Awaitable SaveAsync<T>(T data, string name, bool saveRawFile = true)
        {
            if (!Debug.isDebugBuild) saveRawFile = false;

            try
            {
                await GetFileSystem().Save(data, name, saveRawFile);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Tries to loads the data using the given name.
        /// </summary>
        /// <typeparam name="T">A generic data type to load.</typeparam>
        /// <param name="data">The data to load.</param>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="useRawFile">
        /// Whether to use the raw file to fast load the data without using any compression or cryptography.
        /// </param>
        /// <returns>An asynchronous operation of the loading process.</returns>
        public async Awaitable<bool> TryLoadAsync<T>(T data, string name, bool useRawFile = false)
        {
            var useCompressedFile = !Debug.isDebugBuild || !useRawFile;

            try
            {
                return await GetFileSystem().TryLoad(name, data, useCompressedFile);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return default;
        }

        /// <summary>
        /// Deletes the data using the given name.
        /// </summary>
        /// <param name="name"><inheritdoc cref="SaveAsync{T}(T, string, bool)" path="/param[@name='name']"/></param>
        public void Delete(string name) => GetFileSystem().Delete(name);

        /// <summary>
        /// Deletes all the saved files inside the persistent folder.
        /// </summary>
        public void DeleteAll() => GetFileSystem().DeleteAll();

        /// <summary>
        /// <inheritdoc cref="FileSystem.GetFileNames()"/>
        /// </summary>
        /// <returns><inheritdoc cref="FileSystem.GetFileNames()"/></returns>
        public System.Collections.Generic.IEnumerable<string> GetNames() => FileSystem.GetFileNames();

        /// <summary>
        /// Builds the <see cref="FileSystem"/> using the current settings.
        /// </summary>
        /// <returns></returns>
        public FileSystem GetFileSystem() => new(
            serializer,
            compressor,
            cryptographer,
            cryptographerKey
        );
    }
}