using UnityEngine;
using System;
using System.Threading.Tasks;
using ActionCode.Cryptography;

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
        [Tooltip("The Cryptographer type to use.")]
        public CryptographerType cryptographer;
        [Tooltip("The cryptographer key to use.")]
        public string cryptographerKey = "H2h2xZe83AX90788QNqJXRiWX88xWI2b";
        [Tooltip("The Save Slot name to use.")]
        public string slotName = "SaveSlot";
        [Tooltip("The Slot name used with PlayerPrefs to save the last index.")]
        public string lastSlotKey = "LastSlot";
        [Tooltip("Whether to save the uncompressed/uncryptographed file for debugging. Only works on Editor.")]
        public bool saveRawFile = true;

        /// <summary>
        /// Action fired when the save process starts.
        /// </summary>
        public event Action OnSaveStart;

        /// <summary>
        /// Action fired when the save process finishes.
        /// </summary>
        public event Action OnSaveEnd;

        /// <summary>
        /// Action fired when the load process starts.
        /// </summary>
        public event Action OnLoadStart;

        /// <summary>
        /// Action fired when the load process finishes.
        /// </summary>
        public event Action OnLoadEnd;

        /// <summary>
        /// Action fired when the save process finishes with an error.
        /// </summary>
        public event Action<Exception> OnSaveError;

        /// <summary>
        /// Action fired when the load process finishes with an error.
        /// </summary>
        public event Action<Exception> OnLoadError;

        /// <summary>
        /// Saves the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="name">The file name.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Task<bool> Save<T>(T data, string name)
        {
            OnSaveStart?.Invoke();

#if !UNITY_EDITOR
                saveRawFile = false;
#endif
            try
            {
                await GetFileSystem().Save(data, name, saveRawFile);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnSaveError?.Invoke(e);
                return false;
            }
            finally
            {
                OnSaveEnd?.Invoke();
            }
        }

        /// <summary>
        /// Saves the given data using the slot index.
        /// </summary>
        /// <typeparam name="T">A generic data type to save.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="slotIndex">The slot index to use.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Task<bool> Save<T>(T data, int slotIndex)
        {
            PlayerPrefs.SetInt(lastSlotKey, slotIndex);
            return await Save(data, GetSlotName(slotIndex));
        }

        /// <summary>
        /// Tries to load the data using the given slot index.
        /// </summary>
        /// <typeparam name="T">A generic data type to load.</typeparam>
        /// <param name="slotIndex">The slot index to use.</param>
        /// <param name="target">The target data to load.</param>
        /// <param name="useRawFile">Whether to use the raw file to fast load the data without using compression or cryptography.</param>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<bool> TryLoad<T>(int slotIndex, T target, bool useRawFile = false) =>
            await TryLoad(GetSlotName(slotIndex), target, useRawFile);

        /// <summary>
        /// Tries to load the data using the given name.
        /// </summary>
        /// <typeparam name="T"><inheritdoc cref="TryLoad{T}(int, T, bool)"/></typeparam>
        /// <param name="name">The file name to load.</param>
        /// <param name="target"><inheritdoc cref="TryLoad{T}(int, T, bool)" path="/param[@name='target']"/></param>
        /// <param name="useRawFile"><inheritdoc cref="TryLoad{T}(int, T, bool)" path="/param[@name='useRawFile']"/></param>
        /// <returns><inheritdoc cref="TryLoad{T}(int, T, bool)"/></returns>
        public async Task<bool> TryLoad<T>(string name, T target, bool useRawFile = false)
        {
            OnLoadStart?.Invoke();

            try
            {
                var useCompressedFile = !useRawFile && Application.isEditor;
                return await GetFileSystem().TryLoad(name, target, useCompressedFile);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnLoadError?.Invoke(e);
            }
            finally
            {
                OnLoadEnd?.Invoke();
            }

            return default;
        }

        /// <summary>
        /// Tries to load the data from the last saved slot.
        /// </summary>
        /// <typeparam name="T"><inheritdoc cref="TryLoad{T}(int, T, bool)"/></typeparam>
        /// <param name="target"><inheritdoc cref="TryLoad{T}(int, T, bool)" path="/param[@name='target']"/></param>
        /// <param name="useRawFile"><inheritdoc cref="TryLoad{T}(int, T, bool)" path="/param[@name='useRawFile']"/></param>
        /// <returns><inheritdoc cref="TryLoad{T}(int, T, bool)"/></returns>
        public async Task<bool> TryLoadLastSlot<T>(T target, bool useRawFile = false)
        {
            const int invalidSlot = -1;

            var lastSlot = PlayerPrefs.GetInt(lastSlotKey, defaultValue: invalidSlot);
            var hasLastSlot = lastSlot != invalidSlot;

            return hasLastSlot && await TryLoad(lastSlot, target, useRawFile);
        }

        /// <summary>
        /// Deletes the file using the given index.
        /// </summary>
        /// <param name="slotIndex">The file slot index to delete.</param>
        public void Delete(int slotIndex) => GetFileSystem().Delete(GetSlotName(slotIndex));

        /// <summary>
        /// Deletes the file using the given name.
        /// </summary>
        /// <param name="name">The file name to delete.</param>
        public void Delete(string name) => GetFileSystem().Delete(name);

        private string GetSlotName(int index) => $"{slotName}-{index:D2}";

        private FileSystem GetFileSystem() => new(
            serializer,
            compressor,
            cryptographer,
            cryptographerKey
        );
    }
}