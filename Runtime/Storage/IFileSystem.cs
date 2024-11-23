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
        /// <param name="saveRawData">Whether to save an additional copy of data without any compression or cryptography.</param>
        /// <returns>A task operation of the saving process.</returns>
        Task Save<T>(T data, string name, bool saveRawData);

        /// <summary>
        /// Loads the generic data using the given name. 
        /// </summary>
        /// <typeparam name="T">The generic data type.</typeparam>
        /// <param name="name">The data file name without extension.</param>
        /// <param name="useRawFile">Whether to use the uncompressed file.</param>
        /// <returns>A task operation of the loading process.</returns>
        Task<T> Load<T>(string name, bool useRawFile);
    }
}