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