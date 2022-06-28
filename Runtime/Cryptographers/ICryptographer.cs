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
        /// <returns>A non human readable string.</returns>
        string Encrypt(string value);

        /// <summary>
        /// Decrypts the given value.
        /// </summary>
        /// <param name="value">A string to decrypt.</param>
        /// <returns>A human readable string.</returns>
        string Decrypt(string value);
    }
}