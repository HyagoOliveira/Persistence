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
        /// The current file system.
        /// </summary>
        public IFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Saves the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="name">The file name.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Task<bool> Save<T>(T data, string name)
        {
            CheckFileSystem();
            OnSaveStart?.Invoke();

#if !UNITY_EDITOR
                saveRawFile = false;
#endif
            try
            {
                await FileSystem.Save(data, name, saveRawFile);
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
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="slotIndex">The slot index to use.</param>
        /// <returns>A task operation of the saving process.</returns>
        public async Task<bool> Save<T>(T data, int slotIndex)
        {
            PlayerPrefs.SetInt(lastSlotKey, slotIndex);
            return await Save(data, GetSlotName(slotIndex));
        }

        /// <summary>
        /// Loads using the given name.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="name">The file name to load.</param>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<T> Load<T>(string name)
        {
            CheckFileSystem();
            OnLoadStart?.Invoke();

            try
            {
                return await FileSystem.Load<T>(name);
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
        /// Loads using the given slot index.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="slotIndex">The slot index to use.</param>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<T> Load<T>(int slotIndex) => await Load<T>(GetSlotName(slotIndex));

        /// <summary>
        /// Loads from the last saved slot.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <returns>A task operation of the loading process.</returns>
        public async Task<T> LoadLastSlot<T>()
        {
            const int invalidSlot = -1;

            var lastSlot = PlayerPrefs.GetInt(lastSlotKey, defaultValue: invalidSlot);
            var hasLastSlot = lastSlot != invalidSlot;

            return hasLastSlot ? await Load<T>(lastSlot) : default;
        }

        /// <summary>
        /// Fast loads the raw file using the given name and Serializer, without using compressor or cryptography.
        /// </summary>
        /// <typeparam name="T"><inheritdoc cref="Load{T}(string)"/></typeparam>
        /// <param name="name"><inheritdoc cref="Load{T}(string)"/></param>
        /// <param name="serializer">The Serializer type to use.</param>
        /// <returns><inheritdoc cref="Load{T}(string)"/></returns>
        public static async Task<T> LoadRawFile<T>(string name, SerializerType serializer)
        {
            var fileSystem = new FileSystem(
                serializer,
                CompressorType.None,
                CryptographerType.None,
                string.Empty
            );

            try
            {
                return await fileSystem.LoadRaw<T>(name);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return default;
        }

        private string GetSlotName(int index) => $"{slotName}-{index:D2}";

        private void CheckFileSystem()
        {
            FileSystem ??= new FileSystem(
                serializer,
                compressor,
                cryptographer,
                cryptographerKey
            );
        }
    }
}