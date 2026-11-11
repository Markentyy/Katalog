using System.Collections.ObjectModel;
using CliInterface.Core.GameSystem;

namespace CliInterface.Presentation
{
    /// <summary>
    /// View model for the character catalog view. Holds the data that a GUI
    /// component binds to; the presenter is responsible for loading it.
    /// </summary>
    public sealed class CharacterCatalogViewModel
    {
        /// <summary>
        /// Gets the characters currently displayed in the catalog.
        /// </summary>
        public ObservableCollection<CharacterStats> Characters { get; } = new();

        /// <summary>
        /// Gets whether a load operation is in progress.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Gets the last error message, or null when no error occurred.
        /// </summary>
        public string? Error { get; private set; }

        /// <summary>
        /// Clears the catalog and the error state.
        /// </summary>
        public void Clear()
        {
            Characters.Clear();
            Error = null;
        }

        /// <summary>
        /// Replaces the catalog contents with the given characters.
        /// </summary>
        /// <param name="characters">The characters to display.</param>
        public void SetCharacters(IEnumerable<CharacterStats> characters)
        {
            Clear();
            foreach (CharacterStats character in characters)
                Characters.Add(character);
        }

        /// <summary>
        /// Marks the view model as loading.
        /// </summary>
        public void StartLoading()
        {
            IsLoading = true;
            Error = null;
        }

        /// <summary>
        /// Marks the view model as finished loading.
        /// </summary>
        public void StopLoading() => IsLoading = false;

        /// <summary>
        /// Records an error that occurred during loading.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void SetError(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
            Error = message;
            IsLoading = false;
        }
    }
}
