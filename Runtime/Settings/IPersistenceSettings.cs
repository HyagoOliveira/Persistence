using System;

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
        /// <returns>An instance of the saved data.</returns>
        bool Save<T>(T data, string name);

        /// <summary>
        /// Saves the given data using the slot index.
        /// </summary>
        /// <typeparam name="T">The generic type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="slot">The slot index to use.</param>
        /// <returns>An instance of the saved data.</returns>
        bool Save<T>(T data, int slot);

        /// <summary>
        /// Tries to load the given data using the name.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="data">An instance of the loaded data if available.</param>
        /// <param name="name">The file name to load.</param>
        /// <returns>Whether the data was successfully loaded.</returns>
        bool TryLoad<T>(out T data, string name);

        /// <summary>
        /// Tries to load the given data using the slot index.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="data">An instance of the loaded data if available.</param>
        /// <param name="slot">The slot index to use.</param>
        /// <returns>Whether the data was successfully loaded.</returns>
        bool TryLoad<T>(out T data, int slot);

        /// <summary>
        /// Tries to load the given data using the last saved slot.
        /// </summary>
        /// <typeparam name="T">The generic type of the loaded data.</typeparam>
        /// <param name="data">An instance of the loaded data if available.</param>
        /// <returns>Whether the data was successfully loaded.</returns>
        bool TryLoadLastSlot<T>(out T data);
    }
}