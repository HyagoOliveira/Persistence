using System.Threading.Tasks;

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
        /// <param name="saveRawData">Whether to save the given data without any compression or cryptography.</param>
        /// <returns>A task operation of the saving process.</returns>
        Task Save<T>(T data, string name, bool saveRawData);

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