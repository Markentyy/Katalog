# Katalog

CLI character catalogue built on a facade pattern (`useStrategy`, `execCommand`, `display`) with command strategies, renderers (`IRenderer` + `IRenderable`), and a layered architecture (.NET 8 / C# 12).

## Tech stack

| Layer | Responsibility |
|-------|----------------|
| `Core` | Domain: `Character`, `CharacterStats`, `Equipment`, `Ability`, `Registry<T>`, `DamageCalculator` |
| `Data` | `IWorldStore` + `JsonWorldStore` (atomic JSON via `.tmp`) |
| `Cli` | Facade, REPL, parser, renderers, command strategies |
| `App` | Entry point: argument parsing, dependency wiring |
| `Infrastructure` | `GenshinCharacterCatalogService` — remote API + local fallback |
| `Presentation` | `CharacterCatalogPresenter` + `CharacterCatalogViewModel` (GUI-agnostic) |
| `Api` | Local demo Minimal API on `:5000` serving `data/characters.json` |

## Quick start

1. **Install .NET 8 SDK** (or newer with RollForward enabled).
2. **Clone and build**
   ```bash
   git clone https://github.com/Markentyy/Katalog.git
   cd Katalog
   dotnet build CliInterface-ProjectOOP/CliInterface.slnx
   ```
3. **(Optional) Start the demo API on port 5000** — in a separate terminal:
   ```bash
   dotnet run --project CliInterface-ProjectOOP/src/Api/Api.csproj --urls "http://0.0.0.0:5000"
   ```
   Public Genshin APIs (`https://genshin.jmp.blue/`, `https://api.genshin.dev/`) are currently offline (502/521 Cloudflare), so a local demo Minimal API (`src/Api/`) serving `data/characters.json` was added as a reliable replacement. The CLI loads characters from `:5000` on startup; if unavailable, it falls back to `data/characters.json` and keeps working.
4. **Run the CLI**
   ```bash
   dotnet run --project CliInterface-ProjectOOP/src/App/App.csproj --no-build
   ```
   Data is saved to `CliInterface-ProjectOOP/data/world.json`.
5. **Run with a custom data file**
   ```bash
   dotnet run --project CliInterface-ProjectOOP/src/App/App.csproj --no-build -- --data /tmp/demo.json
   ```
6. **Run the demo scenario** (feeds `demo/demo.txt` as input):
   ```bash
   dotnet run --project CliInterface-ProjectOOP/src/App/App.csproj --no-build -- --data /tmp/demo.json < CliInterface-ProjectOOP/demo/demo.txt
   ```
7. **Run tests**
   ```bash
   dotnet test CliInterface-ProjectOOP/CliInterface.slnx
   ```

## Commands

| Command | Example | Description |
|---------|---------|-------------|
| `ls` | `ls` | Table of all characters with HP bar |
| `info <id\|name>` | `info Mage` | Character card: stats, graphics link, inventory, abilities |
| `create <char\|item\|ability>` | `create char` | Interactive creation dialog; empty id → auto `char-N` |
| `add --char_id <id\|name> --id <id\|name>` | `add --char_id Mage --id Sword` | Equip item / learn ability; auto-saves on success |
| `save` | `save` | Force-save catalogue to data file |
| `load` | `load` | Reload data file (resets current state) |
| `help` | `help` | List commands grouped by strategy |
| `exit` | `exit` | Quit the interpreter |

## Project layout

```
src/
  Core/          Domain model
  Data/          JSON world store (IWorldStore)
  Cli/           Facade, REPL, parser, renderers, strategies
  App/           Entry point
  Infrastructure/ Remote API client + local fallback
  Presentation/  GUI-agnostic catalogue view model
  Api/           Local demo Minimal API (:5000)
  Tests.*        xUnit tests per layer (Core, Data, Cli, Infrastructure)
```

Design diagrams (PlantUML sources + rendered PNGs) live in `CliInterface-ProjectOOP/docs/img/`, full design doc in `CliInterface-ProjectOOP/docs/design.md`.

## License

MIT — see [`CliInterface-ProjectOOP/LICENSE`](CliInterface-ProjectOOP/LICENSE).

## Author

Mark Voskoboinikov ([Markentyy](https://github.com/Markentyy)), group КН-31.
