using CliInterface.Core.GameSystem;

namespace CliInterface.Presentation
{
    /// <summary>
    /// Presenter for the character catalog. Independent of any concrete GUI
    /// framework (CLI, WinForms, Avalonia, etc.) - it only provides data and
    /// formatting, while the presentation (view) owns rendering.
    /// </summary>
    public sealed class CharacterCatalogPresenter
    {
        private readonly ICharacterCatalogService _catalogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterCatalogPresenter"/> class.
        /// </summary>
        /// <param name="catalogService">The catalog data service.</param>
        public CharacterCatalogPresenter(ICharacterCatalogService catalogService)
        {
            ArgumentNullException.ThrowIfNull(catalogService);
            _catalogService = catalogService;
        }

        /// <summary>
        /// Loads all characters sorted by name.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The sorted list of character stats.</returns>
        public async Task<IReadOnlyList<CharacterStats>> LoadCharactersAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CharacterStats> characters = await _catalogService.GetCharactersAsync(cancellationToken);
            return characters.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// Loads a single character by name.
        /// </summary>
        /// <param name="name">The character name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The character stats, or null when not found.</returns>
        public async Task<CharacterStats?> LoadCharacterAsync(
            string name, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            return await _catalogService.GetCharacterByNameAsync(name, cancellationToken);
        }

        /// <summary>
        /// Formats a character for display.
        /// </summary>
        /// <param name="character">The character to format.</param>
        /// <returns>A one-line description of the character.</returns>
        public string FormatCharacter(CharacterStats character) => character.Summary;
    }
}
