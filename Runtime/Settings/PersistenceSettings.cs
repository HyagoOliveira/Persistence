using UnityEngine;
using System;
using System.Threading.Tasks;
using ActionCode.Cryptography;

namespace ActionCode.Persistence
{
    [CreateAssetMenu(fileName = "PersistenceSettings", menuName = "ActionCode/Persistence Settings", order = 110)]
    public sealed class PersistenceSettings : ScriptableObject, IPersistenceSettings
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

        public event Action OnSaveStart;
        public event Action OnSaveEnd;

        public event Action OnLoadStart;
        public event Action OnLoadEnd;

        public event Action<Exception> OnSaveError;
        public event Action<Exception> OnLoadError;

        public IFileSystem FileSystem { get; private set; }

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

        public async Task<bool> Save<T>(T data, int slot)
        {
            PlayerPrefs.SetInt(lastSlotKey, slot);
            return await Save(data, GetSlotName(slot));
        }

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

        public async Task<T> Load<T>(int slot) => await Load<T>(GetSlotName(slot));

        public async Task<T> LoadLastSlot<T>()
        {
            const int invalidSlot = -1;

            var lastSlot = PlayerPrefs.GetInt(lastSlotKey, defaultValue: invalidSlot);
            var hasLastSlot = lastSlot != invalidSlot;

            return hasLastSlot ? await Load<T>(lastSlot) : default;
        }

        private string GetSlotName(int index) => $"{slotName}-{index:D2}";

        private void CheckFileSystem()
        {
            if (FileSystem == null)
            {
                FileSystem = new FileSystem(
                    serializer,
                    compressor,
                    cryptographer,
                    cryptographerKey
                );
            }
        }
    }
}