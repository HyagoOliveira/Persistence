using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Cryptographer class for strings using AES algorithm.
    /// </summary>
    public sealed class AESCryptographer : ICryptographer
    {
        private readonly byte[] keyArray;
        private static readonly byte[] IV = new byte[16];

        /// <summary>
        /// Creates a instance of a Cryptographer using AES algorithm.
        /// </summary>
        /// <param name="key">
        /// The 256-AES key.
        /// <para>You can generate it at http://randomkeygen.com/</para>
        /// </param>
        public AESCryptographer(string key)
        {
            keyArray = Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string value)
        {
            byte[] array;
            using var aes = CreateAlgorithm();
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(value);
            }
            array = memoryStream.ToArray();
            return Convert.ToBase64String(array);
        }

        public string Decrypt(string value)
        {
            byte[] buffer = Convert.FromBase64String(value);
            using var aes = CreateAlgorithm();
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }

        private Aes CreateAlgorithm()
        {
            var algorithm = Aes.Create();
            algorithm.IV = IV;
            algorithm.Key = keyArray;
            return algorithm;
        }
    }
}