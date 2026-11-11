using System.Text.Json;
using CliInterface.Core.GameSystem;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

string baseDir = AppContext.BaseDirectory;
string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));
string dataPath = Path.Combine(solutionRoot, "data", "characters.json");
if (!File.Exists(dataPath))
    throw new FileNotFoundException($"Could not find data/characters.json at {dataPath}.");

string json = File.ReadAllText(dataPath);
List<CharacterStats> characters = ParseCharacters(json);

app.MapGet("/characters", () =>
{
    return Results.Ok(characters.Select(c => new
    {
        id = c.Name.ToLowerInvariant().Replace(" ", "-"),
        name = c.Name,
        element = c.Element,
        weapon = c.Weapon,
        region = c.Region,
        stats = new { health = c.Health, damage = c.Damage, armor = c.Armor, resist = c.Resist }
    }));
});

app.MapGet("/characters/{name}", (string name) =>
{
    CharacterStats? character = characters.FirstOrDefault(c =>
        string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

    return character is null
        ? Results.NotFound(new { error = $"Character '{name}' not found." })
        : Results.Ok(new
        {
            id = character.Name.ToLowerInvariant().Replace(" ", "-"),
            name = character.Name,
            element = character.Element,
            weapon = character.Weapon,
            region = character.Region,
            stats = new { health = character.Health, damage = character.Damage, armor = character.Armor, resist = character.Resist }
        });
});

app.Run();

static List<CharacterStats> ParseCharacters(string json)
{
    using JsonDocument doc = JsonDocument.Parse(json);
    JsonElement root = doc.RootElement;

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

    static string? GetString(JsonElement item, string propertyName) =>
        item.TryGetProperty(propertyName, out JsonElement prop) &&
        prop.ValueKind == JsonValueKind.String
            ? prop.GetString()
            : null;

    static int GetInt(JsonElement item, string propertyName) =>
        item.TryGetProperty(propertyName, out JsonElement prop) &&
        prop.ValueKind == JsonValueKind.Number
            ? prop.GetInt32()
            : 0;

    static double GetDouble(JsonElement item, string propertyName) =>
        item.TryGetProperty(propertyName, out JsonElement prop) &&
        prop.ValueKind == JsonValueKind.Number
            ? prop.GetDouble()
            : 0;

    // The API wraps numeric stats under a "stats" sub-object ({... "stats": {...}}),
    // while the local fallback JSON stores them flat at the item root. Read from
    // "stats" when present, otherwise fall back to the flat shape.
    static JsonElement GetStatsElement(JsonElement item) =>
        item.TryGetProperty("stats", out JsonElement stats) && stats.ValueKind == JsonValueKind.Object
            ? stats
            : item;
}
