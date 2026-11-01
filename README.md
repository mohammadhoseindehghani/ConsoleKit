# ConsoleKit

A colorful C# console toolbox for bootcamp learners. Simple static methods, one helper file, no packages.

**Learning C#? Start with the [learner's handbook](docs/LEARNERS.md).** It teaches every feature separately, with setup steps, copy-and-paste examples, expected results and a complete student-scores app.

- Indexed or inline arrays (`[1, 2, 3]`), quoted string arrays, modern tables and matrices
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

`Program.cs` is a commented cookbook covering every public method, setting and enum choice. The normal run shows all visual examples, then offers live input exercises in an interactive terminal.

```sh
dotnet run --project ConsoleKit -- --demo
# Run every example, including input helpers with supplied answers:
dotnet run --project ConsoleKit -- --all
```

## Use in your project

Copy [ConsoleKit.cs](ConsoleKit/ConsoleKit.cs) into a .NET 8+ console project with implicit usings enabled. The examples use C# 12 collection expressions. No namespace import is required.

```csharp
ConsoleKit.MarkupLine(":rocket: [cyan]Welcome to C#![/]");
ConsoleKit.Success("Your first method works!");
ConsoleKit.PrintArray(new[] { 12, 18, 20 });
ConsoleKit.PrintArrayInline(new[] { 1, 2, 3, 4 }); // [1, 2, 3, 4]
ConsoleKit.PrintArrayInline(new[] { "ali", "sina", "kamran" }); // ["ali", "sina", "kamran"]
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
int choice = ConsoleKit.Menu("What next?", "Practice", "Exit");
```

## Full documentation

Choose the guide that matches what you need:

- **[Learner's handbook](docs/LEARNERS.md):** 54 individual lessons covering every method and setting. Each lesson explains when to use it, shows an example and describes what happens. Includes all table styles, spinner styles, themes, emoji aliases, a complete practice app and common mistakes.
- **[Technical reference](docs/GUIDE.md):** exact method signatures, parameter defaults, validation rules and terminal limitations.

Start with [setting up your project](docs/LEARNERS.md#set-up-your-project-once), use the [feature finder](docs/LEARNERS.md#find-a-feature), or try the [complete student-scores project](docs/LEARNERS.md#practice-project-student-scores).

## Verify

```sh
dotnet build ConsoleKit.slnx
dotnet run --project ConsoleKit.Tests
```

The regression runner uses no testing packages. It checks table geometry, Unicode, input validation, selection, markup, emoji fallbacks, charts, file output, and loading failures and cancellation.
