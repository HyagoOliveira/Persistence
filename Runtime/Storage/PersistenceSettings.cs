using UnityEngine;
using System;
using System.Threading.Tasks;
using ActionCode.Cryptography;
using System.Collections.Generic;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Serialize data using encryption and compression.
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

        [Header("Slots")]
        [Tooltip("The Save Slot name to use.")]
        public string slotName = "SaveSlot";
        [Tooltip("The Slot name used with PlayerPrefs to save the last index.")]
        public string lastSlotKey = "LastSlot";
        [Tooltip("Whether to save the uncompressed/uncryptographed file for debugging. Only works on Editor.")]
        public bool saveRawFile = true;


        /// <summary>
        /// Saves the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="name">The file name.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Awaitable Save<T>(T data, string name, bool saveRawFile = true)
        {

#if !UNITY_EDITOR
            saveRawFile = false;
#endif
            try
            {
                await GetFileSystem().Save(data, name, saveRawFile);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Tries to load the data using the given slot index.
        /// </summary>
        /// <typeparam name="T">A generic data type to load.</typeparam>
        /// <param name="data">The data to load.</param>
        /// <param name="slotIndex">The slot index to use.</param>
        /// <param name="useRawFile">Whether to use the raw file to fast load the data without using compression or cryptography.</param>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<bool> TryLoad<T>(T data, int slotIndex, bool useRawFile = false) =>
            await TryLoad(data, GetSlotName(slotIndex), useRawFile);

        /// <summary>
        /// Tries to load the data using the given name.
        /// </summary>
        /// <typeparam name="T"><inheritdoc cref="TryLoad{T}(T, int, bool)"/></typeparam>
        /// <param name="data"><inheritdoc cref="TryLoad{T}(T, int, bool)" path="/param[@name='data']"/></param>
        /// <param name="name">The file name to load. Don't use any extension.</param>
        /// <param name="useRawFile"><inheritdoc cref="TryLoad{T}(T, int, bool)" path="/param[@name='useRawFile']"/></param>
        /// <returns><inheritdoc cref="TryLoad{T}(T, int, bool)"/></returns>
        public async Task<bool> TryLoad<T>(T data, string name, bool useRawFile = false)
        {
            var useCompressedFile = !useRawFile;

            try
            {
                return await GetFileSystem().TryLoad(name, data, useCompressedFile);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return default;
        }

        /// <summary>
        /// Tries to load the data from the last saved slot.
        /// </summary>
        /// <typeparam name="T"><inheritdoc cref="TryLoad{T}(T, int, bool)"/></typeparam>
        /// <param name="target"><inheritdoc cref="TryLoad{T}(T, int, bool)" path="/param[@name='data']"/></param>
        /// <param name="useRawFile"><inheritdoc cref="TryLoad{T}(T, int, bool)" path="/param[@name='useRawFile']"/></param>
        /// <returns><inheritdoc cref="TryLoad{T}(T, int, bool)"/></returns>
        public async Task<bool> TryLoadLastSlot<T>(T target, bool useRawFile = false)
        {
            const int invalidSlot = -1;

            var lastSlot = GetLastSlot(invalidSlot);
            var hasLastSlot = lastSlot != invalidSlot;

            return hasLastSlot && await TryLoad(target, lastSlot, useRawFile);
        }

        /// <summary>
        /// Tries to delete the file using the given index.
        /// </summary>
        /// <param name="slotIndex">The file slot index to delete.</param>
        /// <returns>Whether the file was deleted.</returns>
        public bool TryDelete(int slotIndex) => TryDelete(GetSlotName(slotIndex));

        /// <summary>
        /// Tries to delete the file using the given name.
        /// </summary>
        /// <param name="name">The file name to delete.</param>
        /// <returns><inheritdoc cref="TryDelete(int)"/></returns>
        public bool TryDelete(string name) => GetFileSystem().TryDelete(name);

        /// <summary>
        /// Tries to delete only the saved files inside the persistent folder.
        /// </summary>
        /// <returns>Whether the saved files were deleted.</returns>
        public bool TryDeleteAll() => GetFileSystem().TryDeleteAll();

        /// <summary>
        /// Returns all file names (without extension) saved on the persistent folder.
        /// </summary>
        /// <returns>An enumerable list containing all file names.</returns>
        public IEnumerable<string> GetNames() => GetFileSystem().GetFileNames();

        /// <summary>
        /// Returns the last slot used or the given default value.
        /// </summary>
        /// <param name="defaultValue">The default value if the last slot is not found.</param>
        /// <returns>Always an integer.</returns>
        public int GetLastSlot(int defaultValue = 0) => PlayerPrefs.GetInt(lastSlotKey, defaultValue);

        public FileSystem GetFileSystem() => new(
            serializer,
            compressor,
            cryptographer,
            cryptographerKey
        );

        public string GetSlotName(int index) => $"{slotName}-{index:D2}";
    }
}