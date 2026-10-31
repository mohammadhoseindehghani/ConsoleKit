# ConsoleKit feature guide

ConsoleKit is a single-file console toolbox designed for students learning arrays, loops and methods. Start with simple calls; the later examples introduce objects and asynchronous work. It has no external dependencies.

## Feature index

- [First program](#first-program)
- [Settings and themes](#settings-and-themes)
- [Text and colors](#text-and-colors)
- [Inline color markup](#inline-color-markup)
- [Emoji](#emoji)
- [Titles, rules and centered text](#titles-rules-and-centered-text)
- [Panels, badges and details](#panels-badges-and-details)
- [Arrays, matrices and tables](#arrays-matrices-and-tables)
- [Lists, steps and trees](#lists-steps-and-trees)
- [Bar charts](#bar-charts)
- [Input and menus](#input-and-menus)
- [Loading and animations](#loading-and-animations)
- [Files and screen control](#files-and-screen-control)
- [Text measurement and terminal support](#text-measurement-and-terminal-support)
- [Complete practice project](#complete-practice-project)
- [Public API reference](#public-api-reference)

## First program

This repository targets .NET 10. To use the helper elsewhere, copy `ConsoleKit/ConsoleKit.cs` into a .NET 8 or newer console project. Enable implicit usings and nullable references, as the standard project template does. Examples use C# 12 collection expressions such as `[1, 2, 3]`; `new[] { 1, 2, 3 }` is also valid for ordinary arrays.

`ConsoleKit` is a class in the global namespace. Do not add `using ConsoleKit;`.

Put this in your own `Program.cs`:

```csharp
ConsoleKit.Title("My first console UI");
ConsoleKit.MarkupLine(":rocket: [cyan]Hello, bootcamp![/]");
ConsoleKit.Success("Ready to practice.");

int[] scores = { 12, 18, 20 };
ConsoleKit.PrintArray(scores, "My scores");
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);

int choice = ConsoleKit.Menu("Next step", "Practice arrays", "Exit");
if (choice == 1)
    ConsoleKit.Info("Try printing an array of your own.");
```

Run the repository's examples:

```sh
dotnet run --project ConsoleKit
# Results screen, then exit:
dotnet run --project ConsoleKit -- --demo
# Every noninteractive example, then exit:
dotnet run --project ConsoleKit -- --all
```

Menus are skipped by the demo when input or output is redirected. The helper's input methods themselves can read piped input.

## Settings and themes

Settings are static: change them once, and subsequent calls use them. Set them at startup, rather than changing them while a loading action is running.

| Setting | Default | Purpose |
| --- | --- | --- |
| `UseColors` | `true` | Apply console foreground/background colors. Redirected output is always uncolored. |
| `UseUnicode` | `true` | Use Unicode borders, bullets, bars and spinner frames. Set false for generated ASCII shapes and emoji alias fallbacks. |
| `UseEmoji` | `true` | Use emoji for named aliases and message icons; false uses text fallbacks. |
| `UseAnimations` | `true` | Enable in-place progress, spinners and typing delays in interactive terminals. |
| `TableStyle` | `TableStyle.Modern` | Appearance of all tables and matrices. |
| `MaxColumnWidth` | `32` | Maximum terminal cells per table column; must be at least 4. |
| `AccentColor` | `ConsoleColor.Cyan` | Accent used for numeric table cells, charts, details, menus, loading and progress. |
| `MutedColor` | `ConsoleColor.DarkGray` | Subtle color for table borders, details and supporting UI. |
| `SpinnerStyle` | `SpinnerStyle.Dots` | Spinner animation frames. |

A typical setup:

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);
ConsoleKit.TableStyle = TableStyle.Modern;
ConsoleKit.MaxColumnWidth = 28;
ConsoleKit.UseEmoji = true;
ConsoleKit.UseUnicode = true;
ConsoleKit.UseColors = true;
ConsoleKit.UseAnimations = true;
ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;
```

### ApplyTheme

`ApplyTheme(ConsoleTheme theme)` changes `AccentColor` and resets `MutedColor` to DarkGray. Success, error and warning retain their green/red/yellow meaning. Explicitly colored text, markup, badges, panels and rainbow text keep their own colors.

| `ConsoleTheme` value | Accent |
| --- | --- |
| `Ocean` | Cyan |
| `Forest` | Green |
| `Sunset` | Magenta |
| `Mono` | White |

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Forest);
ConsoleKit.Info("A green accent.");
ConsoleKit.AccentColor = ConsoleColor.Yellow; // Your own accent
ConsoleKit.MutedColor = ConsoleColor.Gray;   // Increase subtle-text contrast
```

`Mono` sets a neutral accent; to remove all coloring, use `UseColors = false`.

### Simple-terminal setup

```csharp
ConsoleKit.UseColors = false;
ConsoleKit.UseUnicode = false;
ConsoleKit.UseEmoji = false;
ConsoleKit.UseAnimations = false;
ConsoleKit.TableStyle = TableStyle.Ascii;
```

These settings affect generated UI. They do not translate arbitrary Unicode or literal emoji in your own strings into ASCII.

## Text and colors

### Write and WriteLine

`Write(object? text, ConsoleColor? foreground = null, ConsoleColor? background = null)` prints without adding a newline. `WriteLine(object? text = null, ConsoleColor? foreground = null, ConsoleColor? background = null)` adds one.

```csharp
ConsoleKit.Write("Score: ", ConsoleColor.Gray);
ConsoleKit.WriteLine(20, ConsoleColor.Cyan);
ConsoleKit.WriteLine("Highlighted", ConsoleColor.Black, ConsoleColor.Yellow);
ConsoleKit.WriteLine(); // Blank line
```

Numbers and other objects use their `ToString()` representation. Null text prints nothing. Existing console colors are restored after every call, even when writing fails. A null color means keep the current console color.

`ConsoleColor` provides 16 colors: Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray, DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow and White. Their exact shades depend on the terminal's palette.

### Success, Error, Warning and Info

Each accepts a string and prints a complete line with an icon and semantic color:

```csharp
ConsoleKit.Success("Saved successfully."); // Green
ConsoleKit.Error("File not found.");       // Red
ConsoleKit.Warning("Check your answer.");  // Yellow
ConsoleKit.Info("Arrays start at zero.");  // AccentColor
```

Icons use emoji when `UseEmoji` and `UseUnicode` are true. Otherwise they are `[OK]`, `[X]`, `[!]` and `[i]`.

### Rainbow

`Rainbow(string text)` prints each text element in a repeating red/yellow/green/cyan/blue/magenta palette and ends the line. It keeps emoji clusters and combining characters together.

```csharp
ConsoleKit.Rainbow("Welcome to the bootcamp!");
```

This uses the standard console palette; it is not a true-color gradient. With colors disabled, the same text prints normally.

## Inline color markup

### Markup and MarkupLine

Use `[color]text[/]` to color parts of one line. Tags accept the `ConsoleColor` names above, ignoring case. Tags can nest; closing a tag restores the outer tag's color.

`Markup(string text)` does not add a newline. `MarkupLine(string text)` does.

```csharp
ConsoleKit.Markup("[cyan]Name:[/] ");
ConsoleKit.MarkupLine("[white]Sara[/]");
ConsoleKit.MarkupLine("[green]Saved [yellow]3 files[/] successfully.[/]");
ConsoleKit.MarkupLine(":rocket: [magenta]Ready to launch![/]");
```

Emoji aliases are expanded in markup. Colors disappear cleanly in redirected output, but the text remains. Supported tags are foreground colors and `[/]`; there are no bold, RGB, background or link tags.

Malformed markup throws `FormatException` **before any of that call's output is written**. Unknown colors, missing `]`, unmatched `[/]`, and unclosed color tags are errors.

### EscapeMarkup

`EscapeMarkup(string text)` doubles brackets so they print literally. Use it before inserting user input into markup.

```csharp
string answer = "scores[0]";
ConsoleKit.MarkupLine("[cyan]You entered:[/] " + ConsoleKit.EscapeMarkup(answer));
ConsoleKit.MarkupLine("Use [[green]] to start a color tag.");
```

Escaping only handles brackets; recognized emoji aliases can still expand. `Write` and `WriteLine` never interpret markup or emoji aliases, so use those for fully literal output.

## Emoji

UTF-8 output is selected automatically when ConsoleKit first runs in an interactive terminal. Use a font and terminal that support the emoji you want; color emoji rendering depends on the terminal.

### Direct emoji

Ordinary strings can contain emoji:

```csharp
ConsoleKit.WriteLine("🚀 Starting...");
ConsoleKit.Table(["Task", "Status"], ["Read arrays", "✅"]);
```

A portable source-code alternative is a Unicode escape:

```csharp
ConsoleKit.WriteLine("\U0001F680 Starting...");
```

### Emoji

`Emoji(string name)` returns the symbol or its configured text fallback. Names ignore case and may include surrounding colons. An unknown name throws `ArgumentException`.

```csharp
string rocket = ConsoleKit.Emoji("rocket");
ConsoleKit.WriteLine($"{rocket} Starting...");
ConsoleKit.UseEmoji = false;
ConsoleKit.WriteLine(ConsoleKit.Emoji(":check:")); // [OK]
ConsoleKit.UseEmoji = true;
```

### Emojify

`Emojify(string text)` replaces recognized `:name:` aliases. Unknown aliases are left unchanged. It returns a string; call `WriteLine` to print it.

```csharp
ConsoleKit.WriteLine(ConsoleKit.Emojify(":book: Learn :bulb: Practice :trophy: Improve"));
```

All built-in aliases and their fallbacks:

| Alias | Emoji | Text fallback |
| --- | --- | --- |
| `:check:` | ✅ | `[OK]` |
| `:cross:` | ❌ | `[X]` |
| `:warning:` | ⚠️ | `[!]` |
| `:info:` | ℹ️ | `[i]` |
| `:rocket:` | 🚀 | `[GO]` |
| `:star:` | ⭐ | `[*]` |
| `:fire:` | 🔥 | `[HOT]` |
| `:heart:` | ❤️ | `<3` |
| `:smile:` | 😄 | `:)` |
| `:party:` | 🎉 | `[YAY]` |
| `:book:` | 📚 | `[BOOK]` |
| `:folder:` | 📁 | `[DIR]` |
| `:file:` | 📄 | `[FILE]` |
| `:save:` | 💾 | `[SAVE]` |
| `:clock:` | ⏳ | `[WAIT]` |
| `:bulb:` | 💡 | `[TIP]` |
| `:trophy:` | 🏆 | `[WIN]` |
| `:target:` | 🎯 | `[GOAL]` |

Set `UseUnicode = false` or `UseEmoji = false` to select these fallbacks. Literal emoji strings are unaffected.

## Titles, rules and centered text

### Title

`Title(string text)` prints a white heading with a blank line before and after. Tabs and line breaks are flattened to keep the heading on one line.

```csharp
ConsoleKit.Title("Student results");
```

### Rule

`Rule(string title = "", int width = 60, ConsoleColor color = ConsoleColor.DarkGray)` prints a horizontal line with an optional label. Width must be positive. A label longer than the requested width is printed in full.

```csharp
ConsoleKit.Rule();
ConsoleKit.Rule("Results", 50, ConsoleColor.Cyan);
```

### Center

`Center(string text, int width = 0, ConsoleColor? color = null)` centers a single line within a width. Zero uses the terminal width, or 100 cells if unavailable. Negative widths are invalid. Text wider than the target prints without left padding.

```csharp
ConsoleKit.Center("Welcome!", 60);
ConsoleKit.Center("Lesson complete", color: ConsoleColor.Green);
```

The default color is `AccentColor`. This adds spaces; it does not move the cursor to an absolute screen coordinate.

## Panels, badges and details

### Panel

`Panel(string text, string title = "", ConsoleColor color = ConsoleColor.Cyan)` displays multiline text inside a border. The border is rounded when Unicode is enabled and ASCII otherwise. The border uses the requested color, the title is white, and the body keeps the current foreground.

```csharp
ConsoleKit.Panel("Arrays\nLoops\nMethods", "Today's lessons");
ConsoleKit.Panel("Saved successfully.", color: ConsoleColor.Green);
```

Tabs are expanded and line endings normalized. Emoji and wide text are measured in approximate terminal cells. Panels fit their content; they do not wrap long lines. Keep panel text shorter than your terminal width.

### Badge

`Badge(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.DarkBlue)` prints a small padded label **without a newline**. It adds one space on each side.

```csharp
ConsoleKit.Badge("NEW", ConsoleColor.Black, ConsoleColor.Cyan);
ConsoleKit.Write("  ");
ConsoleKit.Badge("PASSED", ConsoleColor.Black, ConsoleColor.Green);
ConsoleKit.WriteLine();
```

With colors disabled, the padded text remains visible.

### KeyValue

`KeyValue(string key, object? value, int keyWidth = 16)` prints a muted label and an accent-colored value on one line. `keyWidth` must be positive. Longer labels are not truncated.

```csharp
ConsoleKit.KeyValue("Student", "Sara");
ConsoleKit.KeyValue("Score", 20);
ConsoleKit.KeyValue("Passed", true, keyWidth: 12);
```

## Arrays, matrices and tables

### PrintArray

`PrintArray(IEnumerable items, string title = "Array")` prints each value beside its **zero-based** index. It works with arrays and lists and uses no object reflection.

```csharp
int[] scores = { 12, 18, 20 };
ConsoleKit.PrintArray(scores, "Scores");
ConsoleKit.PrintArray(new[] { "Apple", "Banana" });
```

Null items appear as `(null)`. An empty collection prints `(no data)` after the title. The collection itself cannot be null.

### Table

`Table(string[] headers, params object?[][] rows)` creates a custom table without requiring a class. Pass the headers, then one array per row.

```csharp
ConsoleKit.Table(
    ["Student", "Score", "Passed"],
    ["Ali", 18, true],
    ["Sara", 20, true],
    ["Reza", 9, false]
);
```

For learners using the older array syntax:

```csharp
string[] headers = { "Student", "Score" };
object?[] firstRow = { "Ali", 18 };
object?[] secondRow = { "Sara", 20 };
ConsoleKit.Table(headers, firstRow, secondRow);
```

At least one header is required. Every row must have the same number of cells as the header array. Null row arrays and mismatched row lengths throw `ArgumentException`. A null cell appears as `(null)`. With headers but no rows, the table structure still prints, and Modern style reports zero rows.

### WriteTable

`WriteTable(IEnumerable items, ConsoleColor? headerColor = null, ConsoleColor? rowColor = null)` creates columns automatically.

Numbers, strings, enums and value types use one `Value` column:

```csharp
ConsoleKit.WriteTable(new[] { 10, 20, 30 });
ConsoleKit.WriteTable(new[] { "Red", "Green", "Blue" });
```

For reference-type objects, public readable properties and public fields become columns:

```csharp
var students = new[]
{
    new { Name = "Ali", Score = 18, Passed = true },
    new { Name = "Sara", Score = 20, Passed = true }
};
ConsoleKit.WriteTable(students);
ConsoleKit.WriteTable(students, ConsoleColor.Yellow, ConsoleColor.Gray);
```

Inherited members come first. A derived member with the same name replaces the base column. Indexers and write-only properties are skipped. A getter that throws displays `(unavailable)` instead of stopping the table. Objects with no displayable members use their string representation.

Use a collection of the same object type. For unrelated types, unavailable members may display `(unavailable)`; use `Table` to build explicit columns instead. Null rows are retained. Empty input prints `(no data)`.

### PrintMatrix

`PrintMatrix<T>(T[,] matrix)` displays a two-dimensional array. Column headers show zero-based indexes.

```csharp
int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };
ConsoleKit.PrintMatrix(grid);
```

Either dimension being zero prints `(no data)`.

### Table styles

The `TableStyle` enum has four values:

```csharp
ConsoleKit.TableStyle = TableStyle.Modern;  // Default: open layout, uppercase headers, row count
ConsoleKit.TableStyle = TableStyle.Minimal; // Compact open layout, no row count
ConsoleKit.TableStyle = TableStyle.Rounded; // Rounded outer border and column separators
ConsoleKit.TableStyle = TableStyle.Ascii;   // +, -, | borders
```

Changing the setting affects `Table`, `WriteTable` and `PrintMatrix`. `UseUnicode = false` replaces generated border characters even in Modern or Minimal mode.

### Alignment, colors and width

- Columns containing only numeric values and optional nulls align to the right. Recognition follows the current culture.
- Numeric cells use `AccentColor`.
- `True` and `Passed` are green; `False` is red; `Needs practice` is yellow. These string matches are case-sensitive.
- Other rows alternate White and Gray text. Null cells use `MutedColor`.
- `WriteTable` can override header and row colors. An explicit row color replaces automatic cell colors.
- Tabs expand; line breaks and control characters become spaces inside cells. Terminal escape controls are not emitted from table cells.
- Long cells are shortened with an ellipsis, or three dots in ASCII mode. Full values remain in your original array.
- Width is capped by `MaxColumnWidth`, then reduced toward four cells per column to fit the terminal when possible. Very many columns may still exceed the terminal width.

```csharp
ConsoleKit.MaxColumnWidth = 20;
ConsoleKit.Table(["Description"], ["This long description will be shortened."]);
ConsoleKit.MaxColumnWidth = 32; // Restore the default
```

Unicode display width is approximate; see [terminal support](#text-measurement-and-terminal-support).

## Lists, steps and trees

### Bullets and NumberedList

Both accept `params string[] items`, so pass individual strings or one string array.

```csharp
ConsoleKit.Bullets("Arrays", "Loops", "Methods");
ConsoleKit.NumberedList("Read the task", "Write a method", "Test your answer");

string[] topics = { "Variables", "Conditions" };
ConsoleKit.Bullets(topics);
```

Bullet prefixes use Unicode bullets or ASCII hyphens. Numbered lists start at **1**. Empty lists print nothing. Items are kept on one line.

### Steps

`Steps(int current, params string[] steps)` shows completed, active and future steps. `current` is **one-based** and must identify an existing step. Completed steps show a green check, the current step has an accent arrow, and future steps use muted markers.

```csharp
ConsoleKit.Steps(2, "Read data", "Process data", "Save results");
```

Here step 1 is complete, step 2 is active, and step 3 is pending. In ASCII mode the completed marker is `+`.

### Tree

`Tree(string root, params string[] children)` prints a root and a single level of children. This intentionally avoids a complex node-building API.

```csharp
ConsoleKit.Tree("MyProject", "Program.cs", "ConsoleKit.cs", "notes.txt");
```

Branches use Unicode connectors, or `+-` in ASCII mode. This is a flat tree, not a recursive filesystem browser; it does not read directories.

## Bar charts

`BarChart(string[] labels, double[] values, int width = 25)` draws horizontal bars, scaled relative to the largest value.

```csharp
ConsoleKit.BarChart(
    ["Arrays", "Loops", "Methods"],
    [12, 9, 6],
    width: 30
);
```

Use equally sized label and value arrays. Width must be positive. Values must be finite and nonnegative; negatives, NaN and infinity throw `ArgumentException`. An empty chart prints `(no data)`. All-zero values produce empty bars without dividing by zero.

Labels are shortened to 24 terminal cells. Bars use `AccentColor` and Unicode blocks, or `#` in ASCII mode. Values print beside the bars. Keep `width` small enough for your terminal: charts do not auto-fit the whole screen.

## Input and menus

Input helpers print a prompt and repeat until valid input is entered. If the input stream ends before a value can be accepted, they throw `EndOfStreamException` rather than looping forever.

### ReadText

`ReadText(string prompt, bool allowEmpty = false)` trims leading/trailing whitespace and rejects empty input unless allowed.

```csharp
string name = ConsoleKit.ReadText("Your name?");
string note = ConsoleKit.ReadText("Optional note:", allowEmpty: true);
```

### ReadInt

`ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)` accepts a whole number within the inclusive range. A minimum above the maximum throws `ArgumentException`.

```csharp
int age = ConsoleKit.ReadInt("Age?", 1, 120);
int count = ConsoleKit.ReadInt("How many items?", min: 0);
```

### ReadDouble

`ReadDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)` accepts a finite floating-point number within the inclusive range. NaN, infinity and out-of-range answers prompt again. NaN bounds or reversed bounds throw `ArgumentException`.

```csharp
double score = ConsoleKit.ReadDouble("Score?", 0, 20);
```

Parsing follows your current culture's decimal and thousands separators. For example, an English culture normally uses `12.5` for twelve and a half. This is a numeric input helper, not a money parser.

### Confirm

`Confirm(string prompt)` returns a Boolean. It accepts `y`, `yes`, `n` and `no`, ignoring case, and retries other responses.

```csharp
if (ConsoleKit.Confirm("Save changes?"))
    ConsoleKit.Success("Saved.");
```

### Menu

`Menu(string title, params string[] options)` displays numbered options and returns the selected **number starting at 1**. At least one option is required.

```csharp
int choice = ConsoleKit.Menu("Main menu", "Add student", "Show scores", "Exit");
switch (choice)
{
    case 1: ConsoleKit.Info("Add a student here."); break;
    case 2: ConsoleKit.WriteTable(new[] { 18, 20, 16 }); break;
    case 3: ConsoleKit.Success("Goodbye!"); break;
}
```

Selection uses a typed number and Enter. It does not require arrow keys or terminal cursor control.

### Select

`Select(string title, params string[] options)` shows the same numbered menu but returns the selected **string**, rather than its number.

```csharp
string language = ConsoleKit.Select("Language", "C#", "Java", "Python");
ConsoleKit.Info($"You chose {language}.");
```

### MultiSelect

`MultiSelect(string title, params string[] options)` lets the user enter comma-separated numbers such as `1,3`. It returns a one-based `int[]`, removes duplicates, and preserves the selection order. Invalid numbers retry the entire answer. Empty input returns an empty array. At least one option is required.

```csharp
string[] topics = { "Arrays", "Loops", "Methods" };
int[] selected = ConsoleKit.MultiSelect("Choose topics", topics);
foreach (int number in selected)
    ConsoleKit.WriteLine(topics[number - 1]); // Convert menu number to array index
```

### Pause

`Pause(string message = "Press Enter to continue...")` prints a message and waits for an input line. It throws `EndOfStreamException` if input has already ended.

```csharp
ConsoleKit.Pause();
ConsoleKit.Pause("Press Enter to return to the menu...");
```

## Loading and animations

Animations require both input and output to be interactive, plus `UseAnimations = true`. With redirected streams or animations disabled, helpers use plain output. Work and timed delays still happen.

Avoid printing other output or prompting for input while a spinner is active: it shares a console line with your work.

### Progress

`Progress(int current, int total, string label = "Progress", int width = 25)` prints a progress bar. You own the loop and the work. `total` and `width` must be positive; `current` must be from zero through total.

```csharp
int[] scores = { 12, 18, 20 };
ConsoleKit.Progress(0, scores.Length, "Processing scores");
for (int i = 0; i < scores.Length; i++)
{
    // Do the real work for scores[i] here.
    Thread.Sleep(100); // Demo only
    ConsoleKit.Progress(i + 1, scores.Length, "Processing scores");
}
```

The final update ends the line. Keep label and width unchanged throughout a progress loop to avoid leftover characters. Handle an empty array before calling progress because zero totals are invalid. Redirected output gets one line per update. Unicode bars use line segments; ASCII bars use `#` and `-`.

### Loading

`Loading(string message, Action work)` runs synchronous work on a worker task while showing a spinner. It blocks until the work finishes, and exceptions reach the caller normally. The action should not require a UI thread.

```csharp
string contents = "";
try
{
    ConsoleKit.Loading("Reading notes", () =>
    {
        contents = File.ReadAllText("notes.txt");
    });
    ConsoleKit.WriteLine(contents);
}
catch (IOException exception)
{
    ConsoleKit.Error(exception.Message);
}
```

A successful action prints a completion message. A failed action does not report success. ConsoleKit does not create `notes.txt` for you.

### LoadingAsync

`LoadingAsync(string message, Func<Task> work, CancellationToken cancellationToken = default)` awaits asynchronous work while animating.

```csharp
string contents = "";
await ConsoleKit.LoadingAsync("Reading notes", async () =>
{
    contents = await File.ReadAllTextAsync("notes.txt");
});
ConsoleKit.WriteLine(contents);
```

For cancellation, pass the same token into the work itself. The helper checks cancellation before starting and after successful work. It always awaits the action so background work does not silently outlive the helper. A token alone cannot forcibly stop an action that ignores cancellation.

```csharp
using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    await ConsoleKit.LoadingAsync("Preparing", async () =>
    {
        await Task.Delay(10_000, cancellation.Token);
    }, cancellation.Token);
}
catch (OperationCanceledException)
{
    ConsoleKit.Warning("Canceled.");
}
```

### SpinnerStyle

The enum supports `Line`, `Dots` and `Bounce`:

```csharp
ConsoleKit.SpinnerStyle = SpinnerStyle.Line;   // | / - backslash
ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;   // Braille dots; default
ConsoleKit.SpinnerStyle = SpinnerStyle.Bounce; // Moving dot
ConsoleKit.Loading("Working", () => Thread.Sleep(1000));
```

`UseUnicode = false` always uses Line frames.

### Wait

`Wait(string message = "Loading", int milliseconds = 1000)` animates a fixed delay. A negative duration is invalid.

```csharp
ConsoleKit.Wait();
ConsoleKit.Wait("Getting ready", 800);
```

This is a demo delay; use `Loading` or `LoadingAsync` for real work. Disabling animations does not skip the delay.

### Typewrite

`Typewrite(string text, int millisecondsPerCharacter = 25)` prints text gradually, then adds a newline. Each visible text element is written together, so joined emoji and combining marks are not split. Negative delays are invalid.

```csharp
ConsoleKit.Typewrite("Welcome to the bootcamp!", 30);
ConsoleKit.Typewrite("🚀 Ready!", 50);
```

Without animation support it prints the whole text immediately. This is a synchronous effect and blocks until it finishes.

## Files and screen control

### PrintFile

`PrintFile(string path, ConsoleColor? color = null)` reads a text file and prints its contents literally, without adding a newline or interpreting markup and aliases.

```csharp
File.WriteAllText("notes.txt", "Arrays start at zero.\nPractice every day.\n");
ConsoleKit.PrintFile("notes.txt", ConsoleColor.Cyan);
```

It reads the whole file into memory. File errors propagate normally, so catch `IOException` or `UnauthorizedAccessException` where appropriate. For a spinner around file reading, see `Loading`; do not print file output from inside an active spinner.

### Clear

`Clear()` clears an interactive console screen and does nothing when output is redirected. Use it between menu pages if desired. It can still fail on a host that does not support console clearing.

```csharp
ConsoleKit.Clear();
ConsoleKit.Title("New page");
```

## Text measurement and terminal support

### DisplayWidth

`DisplayWidth(string text)` returns an approximate number of terminal cells. It counts combining marks as part of their preceding character, recognizes common wide CJK ranges, and treats emoji clusters as wide cells.

```csharp
int plain = ConsoleKit.DisplayWidth("Hello"); // 5
int combined = ConsoleKit.DisplayWidth("e\u0301"); // 1
int emoji = ConsoleKit.DisplayWidth("🚀"); // 2
ConsoleKit.WriteLine($"Widths: {plain}, {combined}, {emoji}");
```

This is for measuring a single line of plain text, not markup, tabs or ANSI escape sequences. Tables and panels sanitize their own cells before measurement.

### Practical limits

- Windows Terminal with an emoji-capable font is a good environment for the examples. Actual colors depend on its palette; console colors are not fixed RGB values.
- UTF-8 output is set automatically for an interactive console. It is left alone when output is redirected, so a caller can control a file or pipe's encoding.
- Width measurement is approximate. Terminal/font choices can render ambiguous characters or emoji differently. Right-to-left shaping and alignment are terminal-dependent.
- Tables try to fit the current terminal width; they cannot make an arbitrary number of columns fit a very small window. Panels and charts do not automatically wrap.
- Redirected output contains no color control codes from these helpers. Generated Unicode is retained unless you disable it.
- Use `UseAnimations = false` for a simple log. For fully plain generated output, also disable colors, Unicode and emoji.
- Loading actions and concurrent writers share the console. Avoid interleaving independent UI operations while a spinner is active.
- This toolbox offers a focused set of learning-friendly methods, not the entire feature set of a terminal UI framework.

## Complete practice project

This example only requires arrays, loops and methods. Copy it into your own `Program.cs` alongside `ConsoleKit.cs`. It prints arrays, a custom table, a chart and a repeatable menu.

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);
ConsoleKit.Title("Student scores");
ConsoleKit.MarkupLine(":book: [cyan]Practice arrays and methods[/]");

string[] names = { "Ali", "Sara", "Reza" };
int[] scores = { 18, 20, 9 };
bool running = true;

while (running)
{
    int choice = ConsoleKit.Menu("Choose an action",
        "Show scores", "Show chart", "Change a score", "Exit");

    switch (choice)
    {
        case 1:
            ShowScores(names, scores);
            break;
        case 2:
            double[] chartValues = new double[scores.Length];
            for (int i = 0; i < scores.Length; i++)
                chartValues[i] = scores[i];
            ConsoleKit.BarChart(names, chartValues);
            break;
        case 3:
            int number = ConsoleKit.Menu("Choose a student", names);
            scores[number - 1] = ConsoleKit.ReadInt("New score?", 0, 20);
            ConsoleKit.Success("Updated!");
            break;
        case 4:
            running = false;
            break;
    }
}

static void ShowScores(string[] names, int[] scores)
{
    object?[][] rows = new object?[names.Length][];
    for (int i = 0; i < names.Length; i++)
    {
        string status = scores[i] >= 10 ? "Passed" : "Needs practice";
        rows[i] = new object?[] { names[i], scores[i], status };
    }
    ConsoleKit.Table(["Name", "Score", "Status"], rows);
}
```

## Public API reference

All methods belong to `ConsoleKit`; all are static. The sections above provide examples and behavior for every entry. Argument types use nullable notation where null is meaningful. Null collection/string arguments are generally invalid unless explicitly documented.

| Group | Methods |
| --- | --- |
| Text | `Write`, `WriteLine`, `Success`, `Error`, `Warning`, `Info`, `Rainbow` |
| Markup | `Markup`, `MarkupLine`, `EscapeMarkup` |
| Emoji | `Emoji`, `Emojify` |
| Theme | `ApplyTheme` |
| Structure | `Title`, `Rule`, `Center`, `Panel`, `Badge`, `KeyValue` |
| Data | `PrintArray`, `Table`, `WriteTable`, `PrintMatrix`, `BarChart` |
| Lists | `Bullets`, `NumberedList`, `Steps`, `Tree` |
| Input | `ReadText`, `ReadInt`, `ReadDouble`, `Confirm`, `Menu`, `Select`, `MultiSelect`, `Pause` |
| Animation | `Progress`, `Loading`, `LoadingAsync`, `Wait`, `Typewrite` |
| Utilities | `PrintFile`, `Clear`, `DisplayWidth` |

| Enum | Values |
| --- | --- |
| `TableStyle` | `Modern` (default), `Rounded`, `Minimal`, `Ascii` |
| `ConsoleTheme` | `Ocean`, `Forest`, `Sunset`, `Mono` |
| `SpinnerStyle` | `Line`, `Dots` (default), `Bounce` |

Settings: `UseColors`, `UseUnicode`, `UseEmoji`, `UseAnimations`, `TableStyle`, `MaxColumnWidth`, `AccentColor`, `MutedColor`, `SpinnerStyle`. See [settings and themes](#settings-and-themes) for every default and example.

Run the checks from the repository root:

```sh
dotnet build ConsoleKit.slnx
dotnet run --project ConsoleKit.Tests
```
