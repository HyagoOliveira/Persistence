namespace ActionCode.Persistence
{
    /// <summary>
    /// Interface used on objects able to be a File System.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Saves the given data using the name. 
        /// </summary>
        /// <typeparam name="T">The data generic type.</typeparam>
        /// <param name="data">The data instance.</param>
        /// <param name="name">The data file name without extension.</param>
        void Save<T>(T data, string name);

        /// <summary>
        /// Saves the given data using the name without compression.
        /// </summary>
        /// <typeparam name="T">The data generic type.</typeparam>
        /// <param name="data">The data instance.</param>
        /// <param name="name">The data file name without extension.</param>
        void SaveUncompressed<T>(T data, string name);

        /// <summary>
        /// Tries to load the data using the given name.
        /// </summary>
        /// <typeparam name="T">The data generic type.</typeparam>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="data">The data instance if available.</param>
        /// <returns>Whether the data exists.</returns>
        bool TryLoad<T>(string name, out T data);
    }
}