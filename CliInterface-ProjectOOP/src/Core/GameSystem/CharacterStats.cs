namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Core stats of a character from the catalog (Health, Damage, Armor, Resist).
    /// </summary>
    /// <remarks>
    /// Slim read-only model used by the presentation layer (ViewModel / GUI).
    /// Holds only data and pure calculations - no console or file access.
    /// </remarks>
    public sealed class CharacterStats
    {
        /// <summary>
        /// Gets the display name of the character.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the element of the character (e.g. Pyro, Anemo).
        /// </summary>
        public string? Element { get; }

        /// <summary>
        /// Gets the weapon type of the character (e.g. Sword, Bow).
        /// </summary>
        public string? Weapon { get; }

        /// <summary>
        /// Gets the region of the character (e.g. Mondstadt, Liyue).
        /// </summary>
        public string? Region { get; }

        /// <summary>
        /// Gets the health points of the character.
        /// </summary>
        public int Health { get; }

        /// <summary>
        /// Gets the base damage of the character.
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// Gets the flat armor (damage reduction) of the character.
        /// </summary>
        public int Armor { get; }

        /// <summary>
        /// Gets the resist percent (0..100) - share of armor ignored by attacks.
        /// </summary>
        public double Resist { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStats"/> class.
        /// </summary>
        public CharacterStats(
            string name,
            int health,
            int damage,
            int armor,
            double resist,
            string? element = null,
            string? weapon = null,
            string? region = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Character name must not be empty.", nameof(name));
            if (health < 0)
                throw new ArgumentOutOfRangeException(nameof(health), "Health must be >= 0.");
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage must be >= 0.");
            if (armor < 0)
                throw new ArgumentOutOfRangeException(nameof(armor), "Armor must be >= 0.");
            if (resist is < 0 or > 100)
                throw new ArgumentOutOfRangeException(nameof(resist), "Resist must be between 0 and 100.");

            Name = name;
            Element = element;
            Weapon = weapon;
            Region = region;
            Health = health;
            Damage = damage;
            Armor = armor;
            Resist = resist;
        }

        /// <summary>
        /// Returns a short one-line description of the character stats.
        /// </summary>
        public string Summary =>
            $"{Name} ({Element ?? "-"}) — HP: {Health}, DMG: {Damage}, ARM: {Armor}, RES: {Resist}%";
    }
}
