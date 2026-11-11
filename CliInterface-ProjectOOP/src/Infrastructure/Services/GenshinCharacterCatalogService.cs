using System.Net;
using System.Text.Json;
using CliInterface.Core.GameSystem;

namespace CliInterface.Infrastructure.Services
{
    /// <summary>
    /// Catalog service that loads characters from a remote JSON API and falls back
    /// to a local JSON file when the network is unavailable.
    /// </summary>
    /// <remarks>
    /// Maps the API character shape (id, name, element, weapon, region, stats)
    /// to <see cref="CharacterStats"/>. Network failures are silently degraded
    /// to the local fallback file so the business layer never depends on the API.
    /// </remarks>
    public sealed class GenshinCharacterCatalogService : ICharacterCatalogService
    {
        private readonly string _apiUrl;
        private readonly string _fallbackPath;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenshinCharacterCatalogService"/> class.
        /// </summary>
        /// <param name="apiUrl">The URL of the remote JSON API.</param>
        /// <param name="fallbackPath">The path to the local JSON fallback file.</param>
        public GenshinCharacterCatalogService(string apiUrl, string fallbackPath)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException("API URL must not be empty.", nameof(apiUrl));
            if (string.IsNullOrWhiteSpace(fallbackPath))
                throw new ArgumentException("Fallback path must not be empty.", nameof(fallbackPath));

            _apiUrl = apiUrl;
            _fallbackPath = fallbackPath;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<CharacterStats>> GetCharactersAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                string json = await _httpClient.GetStringAsync(_apiUrl, cancellationToken);
                return ParseCharacters(json);
            }
            catch (HttpRequestException)
            {
                return LoadFromFallback(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return LoadFromFallback(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<CharacterStats?> GetCharacterByNameAsync(
            string name, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CharacterStats> characters = await GetCharactersAsync(cancellationToken);
            return characters.FirstOrDefault(c =>
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private IReadOnlyList<CharacterStats> LoadFromFallback(CancellationToken cancellationToken)
        {
            if (!File.Exists(_fallbackPath))
                return Array.Empty<CharacterStats>();

            // .NET 8 does not provide a cancellation-token overload for File.ReadAllText;
            // the local fallback is a small file, so reading synchronously is acceptable.
            string json = File.ReadAllText(_fallbackPath);
            return ParseCharacters(json);
        }

        private static List<CharacterStats> ParseCharacters(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // Supports both a bare array at the root and an object with a "characters" property.
            JsonElement items = root.ValueKind == JsonValueKind.Array
                ? root
                : root.TryGetProperty("characters", out JsonElement chars) ? chars : root;

            List<CharacterStats> result = new();
            foreach (JsonElement item in items.EnumerateArray())
            {
                string name = GetString(item, "name") ?? GetString(item, "id") ?? string.Empty;
                JsonElement stats = GetStatsElement(item);

                result.Add(new CharacterStats(
                    name: name,
                    health: GetInt(stats, "health"),
                    damage: GetInt(stats, "damage"),
                    armor: GetInt(stats, "armor"),
                    resist: GetDouble(stats, "resist"),
                    element: GetString(item, "element"),
                    weapon: GetString(item, "weapon"),
                    region: GetString(item, "region")));
            }

            return result;
        }

        private static string? GetString(JsonElement item, string propertyName) =>
            item.TryGetProperty(propertyName, out JsonElement prop) &&
            prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;

        // The API wraps numeric stats under a "stats" sub-object ({... "stats": {...}}),
        // while the local fallback JSON stores them flat at the item root. Read from
        // "stats" when present, otherwise fall back to the flat shape.
        private static JsonElement GetStatsElement(JsonElement item) =>
            item.TryGetProperty("stats", out JsonElement stats) && stats.ValueKind == JsonValueKind.Object
                ? stats
                : item;


        private static int GetInt(JsonElement item, string propertyName) =>
            item.TryGetProperty(propertyName, out JsonElement prop) &&
            prop.ValueKind == JsonValueKind.Number
                ? prop.GetInt32()
                : 0;

        private static double GetDouble(JsonElement item, string propertyName) =>
            item.TryGetProperty(propertyName, out JsonElement prop) &&
            prop.ValueKind == JsonValueKind.Number
                ? prop.GetDouble()
                : 0;
    }
}
