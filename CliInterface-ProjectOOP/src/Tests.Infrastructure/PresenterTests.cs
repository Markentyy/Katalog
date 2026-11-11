using CliInterface.Core.GameSystem;
using CliInterface.Presentation;
using Xunit;

namespace Tests.Infrastructure
{
    /// <summary>
    /// Character catalog presenter checks: loading, lookup and formatting.
    /// </summary>
    public sealed class PresenterTests
    {
        private sealed class FakeCatalogService : ICharacterCatalogService
        {
            private readonly List<CharacterStats> _characters;

            public FakeCatalogService(params CharacterStats[] characters) =>
                _characters = characters.ToList();

            public Task<IReadOnlyList<CharacterStats>> GetCharactersAsync(
                CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyList<CharacterStats>>(_characters);

            public Task<CharacterStats?> GetCharacterByNameAsync(
                string name, CancellationToken cancellationToken = default) =>
                Task.FromResult(_characters.FirstOrDefault(c =>
                    string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task LoadCharacters_ReturnsSortedByName()
        {
            CharacterCatalogPresenter presenter = new(new FakeCatalogService(
                new CharacterStats("Zed", 100, 10, 5, 0),
                new CharacterStats("Amy", 100, 10, 5, 0)));

            IReadOnlyList<CharacterStats> characters = await presenter.LoadCharactersAsync();

            Assert.Equal(2, characters.Count);
            Assert.Equal("Amy", characters[0].Name);
            Assert.Equal("Zed", characters[1].Name);
        }

        [Fact]
        public async Task LoadCharacter_ReturnsMatchingCharacter()
        {
            CharacterCatalogPresenter presenter = new(new FakeCatalogService(
                new CharacterStats("Diluc", 1100, 240, 80, 10)));

            CharacterStats? character = await presenter.LoadCharacterAsync("diluc");

            Assert.NotNull(character);
            Assert.Equal("Diluc", character!.Name);
        }

        [Fact]
        public async Task LoadCharacter_UnknownReturnsNull()
        {
            CharacterCatalogPresenter presenter = new(new FakeCatalogService(
                new CharacterStats("Diluc", 1100, 240, 80, 10)));

            CharacterStats? character = await presenter.LoadCharacterAsync("Nobody");

            Assert.Null(character);
        }

        [Fact]
        public void FormatCharacter_ReturnsSummary()
        {
            CharacterCatalogPresenter presenter = new(new FakeCatalogService(Array.Empty<CharacterStats>()));
            CharacterStats stats = new("Jean", 1050, 180, 70, 15);

            string formatted = presenter.FormatCharacter(stats);

            Assert.Equal(stats.Summary, formatted);
        }

        [Fact]
        public void ViewModel_AddsCharacters()
        {
            CharacterCatalogViewModel vm = new();
            vm.SetCharacters(new[]
            {
                new CharacterStats("A", 100, 10, 5, 0),
                new CharacterStats("B", 100, 10, 5, 0),
            });

            Assert.Equal(2, vm.Characters.Count);
            Assert.False(vm.IsLoading);
            Assert.Null(vm.Error);
        }

        [Fact]
        public void ViewModel_TracksLoadingState()
        {
            CharacterCatalogViewModel vm = new();
            vm.StartLoading();
            Assert.True(vm.IsLoading);
            vm.StopLoading();
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public void ViewModel_RecordsError()
        {
            CharacterCatalogViewModel vm = new();
            vm.SetError("Network unavailable.");

            Assert.Equal("Network unavailable.", vm.Error);
            Assert.False(vm.IsLoading);
        }
    }
}
