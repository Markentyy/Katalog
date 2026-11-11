using CliInterface.Core.GameSystem;
using CliInterface.Infrastructure.Services;
using Xunit;

namespace Tests.Infrastructure
{
    /// <summary>
    /// Catalog service checks: local JSON fallback, case-insensitive lookup.
    /// </summary>
    public sealed class CatalogServiceTests
    {
        private static readonly string FallbackPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../data/characters.json"));

        [Fact]
        public async Task LoadsCharactersFromFallbackFile()
        {
            ICharacterCatalogService service = new GenshinCharacterCatalogService(
                "http://invalid.invalid/api", FallbackPath);

            IReadOnlyList<CharacterStats> characters = await service.GetCharactersAsync();

            Assert.NotEmpty(characters);
            Assert.All(characters, c => Assert.False(string.IsNullOrWhiteSpace(c.Name)));
        }

        [Fact]
        public async Task FindCharacterByNameCaseInsensitive()
        {
            ICharacterCatalogService service = new GenshinCharacterCatalogService(
                "http://invalid.invalid/api", FallbackPath);

            CharacterStats? character = await service.GetCharacterByNameAsync("diluc");

            Assert.NotNull(character);
            Assert.Equal("Diluc", character!.Name);
            Assert.Equal("Pyro", character.Element);
        }

        [Fact]
        public async Task UnknownCharacterReturnsNull()
        {
            ICharacterCatalogService service = new GenshinCharacterCatalogService(
                "http://invalid.invalid/api", FallbackPath);

            CharacterStats? character = await service.GetCharacterByNameAsync("Неіснуючий");

            Assert.Null(character);
        }

        [Fact]
        public void NullApiUrl_IsRejected()
        {
            Assert.Throws<ArgumentException>(() =>
                new GenshinCharacterCatalogService("", FallbackPath));
            Assert.Throws<ArgumentException>(() =>
                new GenshinCharacterCatalogService("   ", FallbackPath));
        }

        [Fact]
        public void NullFallbackPath_IsRejected()
        {
            Assert.Throws<ArgumentException>(() =>
                new GenshinCharacterCatalogService("http://example.com/api", ""));
        }
    }
}
