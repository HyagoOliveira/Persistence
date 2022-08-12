using System.Threading.Tasks;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Interface used on objects able to encrypt and decrypt strings.
    /// </summary>
    public interface ICryptographer
    {
        /// <summary>
        /// Encrypts the given value.
        /// </summary>
        /// <param name="value">A string to encrypt.</param>
        /// <returns>A task operation of the encrypting process containing a non human readable string.</returns>
        Task<string> Encrypt(string value);

        /// <summary>
        /// Decrypts the given value.
        /// </summary>
        /// <param name="value">A string to decrypt.</param>
        /// <returns>A task operation of the decrypting process containing a human readable string.</returns>
        Task<string> Decrypt(string value);
    }
}