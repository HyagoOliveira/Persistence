using System;
using System.Threading.Tasks;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Interface used on objects able to be a Persistence Settings.
    /// </summary>
    interface IPersistenceSettings
    {
        /// <summary>
        /// Action fired when the save process starts.
        /// </summary>
        event Action OnSaveStart;

        /// <summary>
        /// Action fired when the save process finishes.
        /// </summary>
        event Action OnSaveEnd;

        /// <summary>
        /// Action fired when the load process starts.
        /// </summary>
        event Action OnLoadStart;

        /// <summary>
        /// Action fired when the load process finishes.
        /// </summary>
        event Action OnLoadEnd;

        /// <summary>
        /// Action fired when the save process finishes with an error.
        /// </summary>
        event Action<Exception> OnSaveError;

        /// <summary>
        /// Action fired when the load process finishes with an error.
        /// </summary>
        event Action<Exception> OnLoadError;

        /// <summary>
        /// The file system to use.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Saves the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="name">The file name.</param>
        /// <returns>A task operation of the saving process.</returns>
        Task Save<T>(T data, string name);

        /// <summary>
        /// Saves the given data using the slot index.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="slot">The slot index to use.</param>
        /// <returns>A task operation of the saving process.</returns>
        Task Save<T>(T data, int slot);

        /// <summary>
        /// Loads using the given name.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="name">The file name to load.</param>
        /// <returns>A task operation of the loading process.</returns>
        Task<T> Load<T>(string name);

        /// <summary>
        /// Loads using the given slot index.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="slot">The slot index to use.</param>
        /// <returns>A task operation of the loading process.</returns>
        Task<T> Load<T>(int slot);

        /// <summary>
        /// Loads using the last saved slot.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <returns>A task operation of the loading process.</returns>
        Task<T> LoadLastSlot<T>();
    }
}