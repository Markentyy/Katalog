namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Abstraction over the character catalog data source.
    /// </summary>
    /// <remarks>
    /// The implementation may load JSON from a remote API or from a local file.
    /// The business layer (Core) does not know which source is used.
    /// </remarks>
    public interface ICharacterCatalogService
    {
        /// <summary>
        /// Gets all characters from the catalog.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of character stats, sorted by name.</returns>
        Task<IReadOnlyList<CharacterStats>> GetCharactersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single character by name.
        /// </summary>
        /// <param name="name">The character name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The character stats, or null when not found.</returns>
        Task<CharacterStats?> GetCharacterByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
