# ConsoleKit

A colorful C# console toolbox for bootcamp learners. Simple static methods, one helper file, no packages.

- Modern tables, arrays and matrices, with emoji-aware alignment and long-cell limits
- Color themes, inline color tags, badges, rainbow text and emoji aliases
- Panels, headings, lists, steps, trees, key/value details and bar charts
- Numbered menus, text selection, multiple selections and validated input
- Progress bars, three spinner styles, real-work loading, typing effects and file output
- Plain-text, ASCII and animation settings for simpler terminals

## Run the examples

The repository uses the .NET 10 SDK:

```sh
dotnet run --project ConsoleKit
```

The opening screen shows student results; use the menu to explore every feature group.

```sh
dotnet run --project ConsoleKit -- --demo
# Show every noninteractive example:
dotnet run --project ConsoleKit -- --all
```

## Use in your project

Copy [ConsoleKit.cs](ConsoleKit/ConsoleKit.cs) into a .NET 8+ console project with implicit usings enabled. The examples use C# 12 collection expressions. No namespace import is required.

```csharp
ConsoleKit.MarkupLine(":rocket: [cyan]Welcome to C#![/]");
ConsoleKit.Success("Your first method works!");
ConsoleKit.PrintArray(new[] { 12, 18, 20 });
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
int choice = ConsoleKit.Menu("What next?", "Practice", "Exit");
```

## Full documentation

Read the **[complete feature guide](docs/GUIDE.md)** for every method, setting, enum, parameter default, example and terminal limitation. Start with its [first program](docs/GUIDE.md#first-program), or browse the [feature index](docs/GUIDE.md#feature-index).

## Verify

```sh
dotnet build ConsoleKit.slnx
dotnet run --project ConsoleKit.Tests
```

The regression runner uses no testing packages. It checks table geometry, Unicode, input validation, selection, markup, emoji fallbacks, charts, file output, and loading failures and cancellation.
