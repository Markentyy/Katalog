using CliFacade = CliInterface.Cli.Cli;
using CliInterface.Cli;
using CliInterface.Cli.Engine;
using CliInterface.Cli.Rendering;
using CliInterface.Cli.Strategies;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using CliInterface.Data;
using CliInterface.Infrastructure.Services;

// Console interface entry point:
//   dotnet run [-- --data <path>]   data file path, data/world.json by default
// The catalog autoloads from the data file when it exists, so user
// created characters survive restarts without extra commands.
// Characters from the catalog API (localhost:5000) are loaded on startup
// and merged with any locally saved characters.
string dataPath = "data/world.json";
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--data" when i + 1 < args.Length:
            dataPath = args[++i];
            break;
        default:
            Console.Error.WriteLine($"Unknown argument '{args[i]}'. Use --data <path>.");
            return 2;
    }
}

IConsole console = new SystemConsole();
CharacterWorld world = new();
IWorldStore store = new JsonWorldStore(dataPath);
if (File.Exists(dataPath))
{
    try
    {
        store.Load(world);
        console.WriteLine($"Autoloaded {world.Characters.Count} characters from {dataPath}.");
    }
    catch (DataException error)
    {
        console.WriteLine($"Warning: {error.Message}");
    }
}

// Load characters from the catalog API (http://localhost:5000/characters).
// Falls back to data/characters.json when the API is unavailable.
try
{
    var apiService = new GenshinCharacterCatalogService(
        "http://localhost:5000/characters",
        "data/characters.json");

    var apiCharacters = await apiService.GetCharactersAsync();

    foreach (var stats in apiCharacters)
    {
        var character = new Character(
            name: stats.Name,
            maxHealth: stats.Health,
            baseArmor: stats.Armor,
            baseAttack: stats.Damage,
            graphicsUrl: null);

        world.Characters.Add(character);
    }

    console.WriteLine($"Loaded {apiCharacters.Count} characters from catalog API.");
}
catch (Exception ex)
{
    console.WriteLine($"Warning: could not load characters from API: {ex.Message}");
}

CliFacade cli = new(console);
cli.UseStrategy(new InfoStrategy(world, new CharacterTableRenderer(), new CharacterCardRenderer()));
cli.UseStrategy(new TransformStrategy(world, store));
cli.UseStrategy(new InfraStrategy(world, store));

Repl.Run(console, cli,
    "Character catalog. Type 'help' for commands, 'exit' to leave.");
return 0;
