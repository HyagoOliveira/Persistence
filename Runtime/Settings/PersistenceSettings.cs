using System;
using UnityEngine;

namespace ActionCode.Persistence
{
    [CreateAssetMenu(fileName = "PersistenceSettings", menuName = "Persistence/Settings", order = 110)]
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

        public bool Save<T>(T data, string name)
        {
            CheckFileSystem();
            OnSaveStart?.Invoke();

            try
            {
                FileSystem.Save(data, name);
#if UNITY_EDITOR
                if (saveRawFile) FileSystem.SaveUncompressed(data, name);
#endif
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

        public bool Save<T>(T data, int slot)
        {
            PlayerPrefs.SetInt(lastSlotKey, slot);
            return Save(data, GetSlotName(slot));
        }

        public bool TryLoad<T>(out T data, string name)
        {
            CheckFileSystem();
            OnLoadStart?.Invoke();

            try
            {
                var hasData = FileSystem.TryLoad(name, out data);
                return hasData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = default;
                OnLoadError?.Invoke(e);
                return false;
            }
            finally
            {
                OnLoadEnd?.Invoke();
            }
        }

        public bool TryLoad<T>(out T data, int slot) => TryLoad(out data, GetSlotName(slot));

        public bool TryLoadLastSlot<T>(out T data)
        {
            const int invalidSlot = -1;

            var lastSlot = PlayerPrefs.GetInt(lastSlotKey, defaultValue: invalidSlot);
            var hasLastSlot = lastSlot != invalidSlot;

            data = default;
            return hasLastSlot && TryLoad(out data, lastSlot);
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