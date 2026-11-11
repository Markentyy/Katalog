using CliInterface.Core.GameSystem;
using Xunit;

namespace Tests.Infrastructure
{
    /// <summary>
    /// Damage calculation checks matching the catalog combat rules from the design document.
    /// </summary>
    public sealed class DamageCalculatorTests
    {
        private readonly DamageCalculator _calculator = new();

        [Fact]
        public void FullArmorAbsorbsAllDamage()
        {
            double damage = _calculator.CalculateFinalDamage(100, 100, 0);
            Assert.Equal(0, damage);
        }

        [Fact]
        public void ResistIgnoresPartOfArmor()
        {
            // 50% resist ignores half of the armor: effective armor = 100 × (1 − 0.5) = 50.
            // Flat damage = 100 − 50 = 50; magic damage = 50 × (1 − 0.5) = 25.
            double damage = _calculator.CalculateFinalDamage(100, 100, 50);
            Assert.Equal(25, damage);
        }

        [Fact]
        public void FlatBonusAddsDamage()
        {
            double baseDamage = _calculator.CalculateFinalDamage(100, 50, 0);
            double bonusDamage = _calculator.CalculateFinalDamage(100, 50, 0, 30);
            Assert.Equal(baseDamage + 30, bonusDamage);
        }

        [Fact]
        public void DamageCannotBeNegative()
        {
            double damage = _calculator.CalculateFinalDamage(10, 1000, 0);
            Assert.Equal(0, damage);
        }

        [Fact]
        public void ResistAppliedAfterArmorReduction()
        {
            // Armor 50 × (1 − 0.5) = 25 effective armor; flat = 100 − 25 = 75;
            // resist then reduces by 50% → 37.5.
            double damage = _calculator.CalculateFinalDamage(100, 50, 50);
            Assert.Equal(37.5, damage);
        }

        [Fact]
        public void NegativeArguments_AreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.CalculateFinalDamage(-1, 10, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.CalculateFinalDamage(10, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.CalculateFinalDamage(10, 10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.CalculateFinalDamage(10, 10, 101));
        }
    }
}
