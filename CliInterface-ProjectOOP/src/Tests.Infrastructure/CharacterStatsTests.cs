using CliInterface.Core.GameSystem;
using Xunit;

namespace Tests.Infrastructure
{
    /// <summary>
    /// Catalog character stats model validation.
    /// </summary>
    public sealed class CharacterStatsTests
    {
        [Fact]
        public void Stats_CaptureAllProperties()
        {
            CharacterStats stats = new("Diluc", 1100, 240, 80, 10, "Pyro", "Claymore", "Mondstadt");
            Assert.Equal("Diluc", stats.Name);
            Assert.Equal("Pyro", stats.Element);
            Assert.Equal("Claymore", stats.Weapon);
            Assert.Equal("Mondstadt", stats.Region);
            Assert.Equal(1100, stats.Health);
            Assert.Equal(240, stats.Damage);
            Assert.Equal(80, stats.Armor);
            Assert.Equal(10, stats.Resist);
        }

        [Fact]
        public void Summary_ContainsNameAndStats()
        {
            CharacterStats stats = new("Jean", 1050, 180, 70, 15);
            string summary = stats.Summary;
            Assert.Contains("Jean", summary);
            Assert.Contains("1050", summary);
            Assert.Contains("180", summary);
            Assert.Contains("70", summary);
            Assert.Contains("15%", summary);
        }

        [Fact]
        public void InvalidArguments_AreRejected()
        {
            Assert.Throws<ArgumentException>(() => new CharacterStats("  ", 10, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterStats("A", -1, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterStats("A", 10, -1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterStats("A", 10, 0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterStats("A", 10, 0, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterStats("A", 10, 0, 0, 101));
        }
    }
}
