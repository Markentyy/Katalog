namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Pure damage calculation for the catalog combat rules.
    /// </summary>
    /// <remarks>
    /// Implements the damage formula from the design document:
    /// effective armor is reduced by the target's resist percent,
    /// flat damage is attack minus effective armor plus flat bonus,
    /// magic damage is flat damage reduced by resist.
    /// Damage can never be negative.
    /// </remarks>
    public sealed class DamageCalculator
    {
        /// <summary>
        /// Calculates the final damage dealt to a target.
        /// </summary>
        /// <param name="baseDamage">The base damage of the attack.</param>
        /// <param name="armor">The flat armor of the target.</param>
        /// <param name="resist">The resist percent of the target (0..100).</param>
        /// <param name="bonusFlat">Optional flat damage bonus (items, skills, buffs).</param>
        /// <returns>The final damage, never below zero.</returns>
        public double CalculateFinalDamage(
            double baseDamage,
            int armor,
            double resist,
            double bonusFlat = 0)
        {
            if (baseDamage < 0)
                throw new ArgumentOutOfRangeException(nameof(baseDamage), "Base damage must be >= 0.");
            if (armor < 0)
                throw new ArgumentOutOfRangeException(nameof(armor), "Armor must be >= 0.");
            if (resist is < 0 or > 100)
                throw new ArgumentOutOfRangeException(nameof(resist), "Resist must be between 0 and 100.");

            double resistFraction = Math.Clamp(resist / 100.0, 0.0, 1.0);
            int effectiveArmor = Math.Max(0, armor - (int)(armor * resistFraction));

            double flatDamage = Math.Max(0, baseDamage - effectiveArmor + bonusFlat);
            double magicDamage = flatDamage * Math.Max(0, 1.0 - resistFraction);

            return magicDamage;
        }
    }
}
