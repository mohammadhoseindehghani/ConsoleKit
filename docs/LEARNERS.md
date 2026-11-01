# ConsoleKit: a learner's handbook

Use this handbook to **use ConsoleKit in your own console app**. Each numbered lesson explains one public feature, shows a copy-and-paste example, and tells you what to expect.

The [technical reference](GUIDE.md) contains exact signatures, defaults, error rules and terminal details.

## Set up your project once

1. Create a C# **Console App** using .NET 8 or newer. This repository's demo uses .NET 10.
2. Copy [ConsoleKit.cs](../ConsoleKit/ConsoleKit.cs) into your project's folder, beside `Program.cs`.
3. In Visual Studio, check that `ConsoleKit.cs` appears under your project. If needed, use **Add > Existing Item** to include it.
4. Open `Program.cs` and replace its starter `Hello, World!` line with an example below.
5. Run your project in a terminal.

Keep implicit usings and nullable references enabled, as in a new console project. Examples use C# 12, supported by .NET 8+ projects. You need no packages and no `using ConsoleKit;` line.

Keep both files: `ConsoleKit.cs` contains the helper; `Program.cs` contains your own program.

For command-line setup:

```sh
dotnet new console -n MyPractice
```

Copy `ConsoleKit.cs` into `MyPractice`, edit that folder's `Program.cs`, then run:

```sh
dotnet run --project MyPractice
```

### Your first working program

```csharp
ConsoleKit.Title("My practice app");
ConsoleKit.WriteLine("Hello, Sara!", ConsoleColor.Cyan);
ConsoleKit.Success("ConsoleKit is ready.");
```

You should see a heading, a cyan greeting and a green success message. Exact shades depend on your terminal.

### How to read a call

In `ConsoleKit.WriteLine("Hello", ConsoleColor.Green);`:

- `ConsoleKit` is the helper class.
- `WriteLine` is the method you want to use.
- `"Hello"` is the text you pass to it.
- `ConsoleColor.Green` is your chosen color.
- The semicolon ends the statement.

Start with lessons 1–17 while learning arrays. Later lessons introduce input, menus, animations and settings. Each C# code block is an independent example you can try in `Program.cs` after setup. Examples with prompts need you to type an answer.

## Find a feature

| I want to… | Lessons |
| --- | --- |
| Print colored text | [1. WriteLine](#1-writeline), [2. Write](#2-write) |
| Print status messages | [3. Success](#3-success), [4. Error](#4-error), [5. Warning](#5-warning), [6. Info](#6-info) |
| Add headings and layout | [7. Title](#7-title), [8. Rule](#8-rule), [9. Center](#9-center), [10. Panel](#10-panel), [11. Badge](#11-badge), [12. KeyValue](#12-keyvalue) |
| Display arrays and tables | [13. PrintArray](#13-printarray), [14. PrintArrayInline](#14-printarrayinline), [15. Table](#15-table), [16. WriteTable](#16-writetable), [17. PrintMatrix](#17-printmatrix) |
| Display lists and workflows | [18. Bullets](#18-bullets), [19. NumberedList](#19-numberedlist), [20. Steps](#20-steps), [21. Tree](#21-tree) |
| Compare numbers visually | [22. BarChart](#22-barchart) |
| Read user input | [23. ReadText](#23-readtext), [24. ReadInt](#24-readint), [25. ReadDouble](#25-readdouble), [26. Confirm](#26-confirm) |
| Show choices | [27. Menu](#27-menu), [28. Select](#28-select), [29. MultiSelect](#29-multiselect), [30. Pause](#30-pause) |
| Color parts of a line | [31. MarkupLine](#31-markupline), [32. Markup](#32-markup), [33. EscapeMarkup](#33-escapemarkup) |
| Use emoji and more colors | [34. Emoji](#34-emoji), [35. Emojify](#35-emojify), [36. Rainbow](#36-rainbow), [37. ApplyTheme](#37-applytheme) |
| Show loading and animation | [38. Progress](#38-progress), [39. Loading](#39-loading), [40. LoadingAsync](#40-loadingasync), [41. Wait](#41-wait), [42. Typewrite](#42-typewrite) |
| Use files and screen utilities | [43. PrintFile](#43-printfile), [44. Clear](#44-clear), [45. DisplayWidth](#45-displaywidth) |
| Change settings | [46–54. Settings](#settings) |
| Build a complete app | [Practice project](#practice-project-student-scores) |
| Fix an issue | [Common questions](#common-questions) |

### Run the commented cookbook

The repository's [Program.cs](../ConsoleKit/Program.cs) demonstrates every ConsoleKit method and setting. Read its comments and copy the examples you need. The tour covers text, layout, both array formats, tables, lists, charts, markup, emoji, themes, files, animations, measurement and input.

From the repository root:

```sh
# Visual examples, followed by live input exercises in an interactive terminal:
dotnet run --project ConsoleKit
# Visual examples only; no input exercises:
dotnet run --project ConsoleKit -- --demo
# The complete tour, with supplied answers for every input example:
dotnet run --project ConsoleKit -- --all
# Explain the available run options:
dotnet run --project ConsoleKit -- --help
```

In `--all` mode, the demo prints the supplied answers beside each prompt and restores the original input stream afterward. It uses its own temporary sample file and removes it when finished. The live input exercise asks before demonstrating screen clearing, so you can keep the tour output visible. When streams are redirected, colors and in-place animations are disabled automatically; examples still print their results.

## 1. WriteLine

**Use it to:** print text and move to the next line.

```csharp
ConsoleKit.WriteLine("Hello!");
ConsoleKit.WriteLine("I am learning C#.", ConsoleColor.Green);
ConsoleKit.WriteLine(123, ConsoleColor.Cyan);
ConsoleKit.WriteLine();
ConsoleKit.WriteLine("This follows a blank line.");
```

**What you see:** each message on its own line. The second message is green and the number is cyan. An empty call creates a blank line.

You can also set a background:

```csharp
ConsoleKit.WriteLine("Important", ConsoleColor.Black, ConsoleColor.Yellow);
```

The first color is the text; the second is the background. Previous console colors are restored after each call.

## 2. Write

**Use it to:** print several pieces on the same line.

```csharp
ConsoleKit.Write("Your score: ", ConsoleColor.Gray);
ConsoleKit.Write(20, ConsoleColor.Cyan);
ConsoleKit.WriteLine(" / 20", ConsoleColor.Gray);
```

**What you see:** `Your score: 20 / 20` on one line. Only `20` is cyan. Finish with `WriteLine` when you want a new line.

## 3. Success

**Use it to:** tell the user something worked.

```csharp
ConsoleKit.Success("Your answer is correct!");
ConsoleKit.Success("Student saved.");
```

**What you see:** green messages with success icons. With emoji disabled, the prefix is `[OK]`.

## 4. Error

**Use it to:** display a problem.

```csharp
int score = -5;
if (score < 0)
    ConsoleKit.Error("A score cannot be negative.");
```

**What you see:** a red error message. This prints a message; it does not stop the program or throw an exception.

## 5. Warning

**Use it to:** ask the user to pay attention.

```csharp
int attemptsLeft = 1;
ConsoleKit.Warning($"You have {attemptsLeft} attempt left.");
```

**What you see:** a yellow warning message.

## 6. Info

**Use it to:** display a helpful note.

```csharp
ConsoleKit.Info("Array indexes start at zero.");
```

**What you see:** an information icon and a message in the accent color, which is cyan by default.

## 7. Title

**Use it to:** start a new section.

```csharp
ConsoleKit.Title("Student results");
ConsoleKit.WriteLine("Sara: 20");
ConsoleKit.WriteLine("Ali: 18");
```

**What you see:** a white heading with a blank line above and below it.

## 8. Rule

**Use it to:** separate sections with a horizontal line.

```csharp
ConsoleKit.Rule();
ConsoleKit.Rule("Results");
ConsoleKit.Rule("End", 40, ConsoleColor.Magenta);
```

**What you see:** three dividers. The last is 40 cells wide and magenta. The default width is 60. Width must be positive. A label longer than the requested width prints in full.

## 9. Center

**Use it to:** put a short line in the middle of a chosen width.

```csharp
ConsoleKit.Center("Welcome!", 40);
ConsoleKit.Center("You did it!", 40, ConsoleColor.Green);
ConsoleKit.Center("My app");
```

**What you see:** spaces position the first two messages in the middle of 40 cells. Omitting the width uses the terminal width. It handles one line at a time and does not truncate text.

## 10. Panel

**Use it to:** put a short message or several lines inside a border.

```csharp
ConsoleKit.Panel("Arrays\nLoops\nMethods", "Today's lessons");
ConsoleKit.Panel("Practice every day.", "Tip", ConsoleColor.Green);
```

**What you see:** two titled panels. `\n` starts a new body line. The color argument changes the border. Keep lines short enough to fit your terminal; panels do not wrap them automatically.

## 11. Badge

**Use it to:** print a small colored label.

```csharp
ConsoleKit.Badge("NEW", ConsoleColor.Black, ConsoleColor.Cyan);
ConsoleKit.Write("  ");
ConsoleKit.Badge("PASSED", ConsoleColor.Black, ConsoleColor.Green);
ConsoleKit.WriteLine();
```

**What you see:** two labels on one line. The first color controls the text; the second controls the background. `Badge` does not add a newline.

## 12. KeyValue

**Use it to:** display a label beside a value.

```csharp
ConsoleKit.KeyValue("Name", "Sara");
ConsoleKit.KeyValue("Age", 19);
ConsoleKit.KeyValue("Score", 20);
ConsoleKit.KeyValue("Completed exercises", 12, 24);
```

**What you see:** labels on the left and accent-colored values on the right. The last argument chooses the label width, here 24 cells. It must be positive. The default label width is 16.

## 13. PrintArray

**Use it to:** see an array's values and indexes.

```csharp
int[] scores = { 12, 18, 20 };
ConsoleKit.PrintArray(scores, "Scores");
```

**What you see:**

```text
  Scores

[0] 12
[1] 18
[2] 20
```

Indexes start at **0**. String arrays work too:

```csharp
string[] names = { "Ali", "Sara", "Reza" };
ConsoleKit.PrintArray(names);
```

Empty arrays display `(no data)`. Null values display `(null)`.

## 14. PrintArrayInline

**Use it to:** print the whole array together on one line, like the arrays you write in code.

```csharp
int[] numbers = { 1, 2, 3, 4 };
ConsoleKit.PrintArrayInline(numbers);
```

**What you see:**

```text
[1, 2, 3, 4]
```

Strings automatically get double quotes:

```csharp
string[] names = { "ali", "sina", "kamran" };
ConsoleKit.PrintArrayInline(names);
```

```text
["ali", "sina", "kamran"]
```

Choose a color by passing it as the second argument:

```csharp
int[] numbers = { 1, 2, 3, 4 };
ConsoleKit.PrintArrayInline(numbers, ConsoleColor.Magenta);
```

Without a color argument, the method uses the current accent color. It adds a newline automatically. It does not print a heading or indexes.

### Choose the array format you need

```csharp
int[] numbers = { 1, 2, 3 };
ConsoleKit.PrintArray(numbers, "Numbers with indexes");
ConsoleKit.PrintArrayInline(numbers);
```

```text
  Numbers with indexes

[0] 1
[1] 2
[2] 3
[1, 2, 3]
```

Use `PrintArray` to study indexes, and `PrintArrayInline` to see the values together.

### Empty arrays, lists and mixed values

```csharp
ConsoleKit.PrintArrayInline(Array.Empty<int>());
ConsoleKit.PrintArrayInline(new List<string> { "ali", "sina" });
ConsoleKit.PrintArrayInline(new object?[] { 1, "Ali", true, null });
ConsoleKit.PrintArrayInline(new[] { 'A', 'B' });
```

```text
[]
["ali", "sina"]
[1, "Ali", true, null]
["A", "B"]
```

An empty array prints `[]`. Boolean values print as `true` or `false`; a missing value prints as `null`. Lists work just like arrays.

### Strings containing quotes or newlines

```csharp
string[] messages = { "He said \"Hi\"", "Line 1\nLine 2", "C:\\notes" };
ConsoleKit.PrintArrayInline(messages);
```

```text
["He said \"Hi\"", "Line 1\nLine 2", "C:\\notes"]
```

Backslashes make special characters visible, so the array stays on one logical line. The original strings are unchanged. Numeric values always use a decimal dot, keeping commas clear as item separators. A very long line can still wrap at the edge of a small terminal.

Use this for flat arrays. It does not expand nested arrays or turn object properties into columns; use `PrintMatrix` or `WriteTable` for those jobs. The array itself cannot be null.

## 15. Table

**Use it to:** make a table by supplying the headings and rows yourself.

```csharp
ConsoleKit.Table(
    ["Name", "Score", "Passed"],
    ["Ali", 18, true],
    ["Sara", 20, true],
    ["Reza", 9, false]
);
```

**What you see:** a modern table with three columns and three rows. The first array supplies the headings. Every following array is one row. Numeric columns align to the right; `true` is green and `false` is red.

Every row must have the same number of values as there are headings: **3** here.

With the older array syntax, you can write:

```csharp
string[] headings = { "Name", "Score" };
object?[] firstRow = { "Ali", 18 };
object?[] secondRow = { "Sara", 20 };
ConsoleKit.Table(headings, firstRow, secondRow);
```

An `object?[]` can hold mixed values such as strings and integers. The question mark also allows null values.

## 16. WriteTable

**Use it to:** turn an existing array or list into a table automatically.

```csharp
string[] fruits = { "Apple", "Banana", "Orange" };
ConsoleKit.WriteTable(fruits);
```

**What you see:** one `VALUE` column containing the fruits. Numbers work too:

```csharp
int[] scores = { 18, 20, 16 };
ConsoleKit.WriteTable(scores);
```

When you learn objects, named properties can become columns:

```csharp
var students = new[]
{
    new { Name = "Ali", Score = 18 },
    new { Name = "Sara", Score = 20 }
};
ConsoleKit.WriteTable(students);
```

Here `new { ... }` creates a small object with named properties. Return to this example after learning objects; arrays of strings and numbers work immediately.

To choose header and row colors:

```csharp
int[] scores = { 18, 20, 16 };
ConsoleKit.WriteTable(scores, ConsoleColor.Yellow, ConsoleColor.White);
```

Public fields and inherited properties are supported too. An explicit row color replaces automatic numeric and status colors. Empty input displays `(no data)`.

## 17. PrintMatrix

**Use it to:** print a two-dimensional array.

```csharp
int[,] grid =
{
    { 1, 2, 3 },
    { 4, 5, 6 }
};
ConsoleKit.PrintMatrix(grid);
```

**What you see:** two rows and three columns, with headings `[0]`, `[1]` and `[2]`. The comma in `int[,]` means this array has two dimensions.

## 18. Bullets

**Use it to:** display an unordered list.

```csharp
ConsoleKit.Bullets("Arrays", "Loops", "Methods");
```

**What you see:** each topic on a separate line with a bullet. An array works too:

```csharp
string[] topics = { "Arrays", "Loops", "Methods" };
ConsoleKit.Bullets(topics);
```

## 19. NumberedList

**Use it to:** display a list with numbers.

```csharp
ConsoleKit.NumberedList("Read the task", "Write your code", "Test your answer");
```

**What you see:**

```text
  1. Read the task
  2. Write your code
  3. Test your answer
```

This displays a list. Use `Menu` when the user should choose an item.

## 20. Steps

**Use it to:** show the current step in a workflow.

```csharp
ConsoleKit.Steps(2, "Read data", "Process data", "Save results");
```

**What you see:** step 1 is complete, step 2 is active, and step 3 is waiting. The first number is the active step, starting at **1**. It must identify one of the supplied steps. Printing the steps does not run their work.

## 21. Tree

**Use it to:** display a root with connected items beneath it.

```csharp
ConsoleKit.Tree("MyProject", "Program.cs", "ConsoleKit.cs", "notes.txt");
```

**What you see:**

```text
  MyProject
  ├─ Program.cs
  ├─ ConsoleKit.cs
  └─ notes.txt
```

The first string is the root; the rest are children. This shows one level of items. It does not search your computer or read folders.

## 22. BarChart

**Use it to:** compare numbers visually.

```csharp
ConsoleKit.BarChart(["Arrays", "Loops", "Methods"], [12, 9, 6]);
```

**What you see:** three horizontal bars. Arrays has the longest bar because 12 is the largest value. The first array contains labels; the second contains matching numbers. Their lengths must be equal.

Use zero or positive finite values. A custom bar width looks like this:

```csharp
ConsoleKit.BarChart(["Ali", "Sara"], [18, 20], 15);
```

The last number is the maximum bar width, here 15 cells. Exact values print beside the bars.

## 23. ReadText

**Use it to:** ask for a string and store the answer.

```csharp
string name = ConsoleKit.ReadText("What is your name?");
ConsoleKit.WriteLine($"Hello, {name}!");
```

**Try:** enter `Sara`, then press Enter. You should see `Hello, Sara!`. Empty input prompts again. For optional input:

```csharp
string note = ConsoleKit.ReadText("Optional note:", true);
ConsoleKit.WriteLine($"Note: {note}");
```

Spaces at the beginning and end are removed.

## 24. ReadInt

**Use it to:** read a whole number safely.

```csharp
int age = ConsoleKit.ReadInt("How old are you?", 1, 120);
ConsoleKit.WriteLine($"Your age is {age}.");
```

**Try:** enter `hello`, then `-1`, then `19`. Only 19 is accepted. The second and third arguments are the smallest and largest accepted numbers, including both ends.

You can omit the limits:

```csharp
int number = ConsoleKit.ReadInt("Enter a whole number:");
ConsoleKit.WriteLine(number);
```

## 25. ReadDouble

**Use it to:** read a number that may have a decimal part.

```csharp
double score = ConsoleKit.ReadDouble("Enter your score:", 0, 20);
ConsoleKit.WriteLine($"Your score is {score}.");
```

**Try:** enter `18.5` if your system uses a dot as the decimal separator. Some system languages use a comma instead. Answers outside 0–20 prompt again.

## 26. Confirm

**Use it to:** ask a yes/no question.

```csharp
bool save = ConsoleKit.Confirm("Save your changes?");
if (save)
    ConsoleKit.Success("Saved!");
else
    ConsoleKit.Info("Changes were not saved.");
```

**Try:** enter `y` or `yes` to get `true`; enter `n` or `no` to get `false`. Capital letters work too. Other answers prompt again.

## 27. Menu

**Use it to:** let the user choose a numbered option.

```csharp
int choice = ConsoleKit.Menu("Main menu", "Add student", "Show scores", "Exit");
if (choice == 1)
    ConsoleKit.Info("You chose Add student.");
else if (choice == 2)
    ConsoleKit.WriteTable(new[] { 18, 20, 16 });
else
    ConsoleKit.Success("Goodbye!");
```

**Try:** type `2` and press Enter. The score table appears.

Menu numbers start at **1**; array indexes start at **0**. Your code decides what each returned number does. An option named `Exit` does not close the app automatically. See the practice project for a repeating menu.

## 28. Select

**Use it to:** choose an option and receive its text directly.

```csharp
string language = ConsoleKit.Select("Choose a language", "C#", "Java", "Python");
ConsoleKit.WriteLine($"You chose {language}.");
```

**Try:** type `1`. The method returns the string `C#`. `Menu` returns a number; `Select` returns the text. Both use typed numbers and Enter.

## 29. MultiSelect

**Use it to:** choose several options at once.

```csharp
string[] topics = { "Arrays", "Loops", "Methods" };
int[] choices = ConsoleKit.MultiSelect("Choose topics", topics);
foreach (int number in choices)
{
    ConsoleKit.WriteLine(topics[number - 1]);
}
```

**Try:** enter `1,3`. You should see Arrays and Methods. Returned numbers start at 1, so subtract 1 for an array index. Duplicates are removed. Empty input returns an empty array. Invalid selections prompt again.

## 30. Pause

**Use it to:** wait for Enter before continuing.

```csharp
ConsoleKit.WriteLine("Read this message first.");
ConsoleKit.Pause();
ConsoleKit.WriteLine("Now we can continue.");
```

You can change the prompt:

```csharp
ConsoleKit.Pause("Press Enter to return to the menu...");
```

## 31. MarkupLine

**Use it to:** color different parts of one line using small tags.

```csharp
ConsoleKit.MarkupLine("[cyan]Name:[/] Sara | [green]Score:[/] 20");
ConsoleKit.MarkupLine(":rocket: [magenta]Ready to learn![/]");
```

**What you see:** text inside each color tag uses that color. Emoji aliases expand too. Open with `[green]`; close with `[/]`. Colors are normal ConsoleColor names, ignoring case.

Nested colors work:

```csharp
ConsoleKit.MarkupLine("[green]Saved [yellow]3 files[/] successfully.[/]");
```

Here `3 files` is yellow and the surrounding sentence is green. Every opening tag needs a closing tag. Unsupported or unclosed tags cause a `FormatException` before that call prints anything.

## 32. Markup

**Use it to:** print colored parts without moving to a new line.

```csharp
ConsoleKit.Markup("[cyan]Score:[/] ");
ConsoleKit.MarkupLine("[green]20[/] / 20");
```

**What you see:** `Score: 20 / 20` on one line. Finish with `MarkupLine` or `WriteLine` for a newline.

## 33. EscapeMarkup

**Use it to:** display brackets literally when inserting a value into markup.

```csharp
string answer = "scores[0]";
string safeAnswer = ConsoleKit.EscapeMarkup(answer);
ConsoleKit.MarkupLine("[cyan]You entered:[/] " + safeAnswer);
```

**What you see:** `You entered: scores[0]`. Its brackets are displayed rather than treated as a tag. For a fixed string, double the brackets yourself:

```csharp
ConsoleKit.MarkupLine("Write [[green]] to open a color tag.");
```

`EscapeMarkup` returns text; it does not print it. For fully literal text without tag or alias interpretation, use `WriteLine`.

## 34. Emoji

**Use it to:** get an emoji by a simple name.

```csharp
string rocket = ConsoleKit.Emoji("rocket");
ConsoleKit.WriteLine($"{rocket} Starting my app!");
```

**What you see:** a rocket and your message. Names ignore case. `"rocket"` and `":rocket:"` both work. Use an icon in a table like this:

```csharp
ConsoleKit.Table(
    ["Task", "Status"],
    ["Read arrays", ConsoleKit.Emoji("check")],
    ["Practice methods", ConsoleKit.Emoji("clock")]
);
```

Unknown names cause an `ArgumentException`. Use a name from the next lesson, or write your own literal emoji:

```csharp
ConsoleKit.WriteLine("🚀 My app is starting!");
```

Emoji appearance and color depend on your terminal and font.

## 35. Emojify

**Use it to:** replace several emoji names inside a string.

```csharp
string message = ConsoleKit.Emojify(":book: Learn  :bulb: Practice  :trophy: Improve");
ConsoleKit.WriteLine(message);
```

**What you see:** books, a light bulb and a trophy beside the words. `Emojify` returns a string; `WriteLine` prints it. Unknown aliases stay unchanged.

Every built-in alias:

| Alias | Icon | Text fallback |
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

Try all of them:

```csharp
ConsoleKit.WriteLine(ConsoleKit.Emojify(
    ":check: :cross: :warning: :info: :rocket: :star: :fire: :heart: :smile: " +
    ":party: :book: :folder: :file: :save: :clock: :bulb: :trophy: :target:"
));
```

`WriteLine(":rocket:")` prints the name literally. Use `Emojify`, `Emoji` or `MarkupLine` for alias replacement.

## 36. Rainbow

**Use it to:** print playful text in a repeating color pattern.

```csharp
ConsoleKit.Rainbow("Welcome to the bootcamp!");
```

**What you see:** characters in red, yellow, green, cyan, blue and magenta. The method adds a newline automatically.

## 37. ApplyTheme

**Use it to:** change the accent color in one call.

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Forest);
ConsoleKit.Info("The accent is now green.");
ConsoleKit.BarChart(["Arrays", "Loops"], [12, 8]);
```

**What you see:** a green information message and chart bars. Themes also affect menu numbers, input prompts, numeric table cells and loading displays. Success remains green, errors red, and warnings yellow.

Every `ConsoleTheme` choice:

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);
ConsoleKit.Info("Ocean uses cyan.");
ConsoleKit.ApplyTheme(ConsoleTheme.Forest);
ConsoleKit.Info("Forest uses green.");
ConsoleKit.ApplyTheme(ConsoleTheme.Sunset);
ConsoleKit.Info("Sunset uses magenta.");
ConsoleKit.ApplyTheme(ConsoleTheme.Mono);
ConsoleKit.Info("Mono uses white.");
ConsoleKit.ApplyTheme(ConsoleTheme.Ocean); // Restore the default accent
```

Explicit colors passed to methods are kept. `Mono` changes the accent to white; `UseColors = false` disables all coloring. Applying any theme resets `MutedColor` to DarkGray.

## 38. Progress

**Use it to:** show how much work your loop has completed.

```csharp
string[] names = { "Ali", "Sara", "Reza" };
ConsoleKit.Progress(0, names.Length, "Processing students");
for (int i = 0; i < names.Length; i++)
{
    // Replace this pause with your own work for names[i].
    Thread.Sleep(300);
    ConsoleKit.Progress(i + 1, names.Length, "Processing students");
}
```

**What you see:** a bar moving from 0% to 100%. The arguments are completed items, total items, label, and optionally width:

```csharp
ConsoleKit.Progress(5, 10, "Completed", 15);
ConsoleKit.Progress(10, 10, "Completed", 15);
```

The first call is 50%; the second is 100% and ends the line. Keep label and width unchanged during a loop. Total and width must be positive, and completed items must be between zero and total. Check for an empty array before starting. `Thread.Sleep(300)` is a 300-millisecond demonstration pause.

## 39. Loading

**Use it to:** run work while a spinner tells the user to wait.

```csharp
int total = 0;
ConsoleKit.Loading("Adding scores", () =>
{
    int[] scores = { 18, 20, 16 };
    for (int i = 0; i < scores.Length; i++)
        total += scores[i];
    Thread.Sleep(800); // Demo pause so you can see the spinner
});
ConsoleKit.WriteLine($"Total: {total}");
```

**What you see:** a spinner, a completion message, then `Total: 54`.

The `() => { ... }` part passes a block of work as the second argument. ConsoleKit runs that block and waits for it to finish. Put real calculation or file reading inside it. Print results **after** the loading call to avoid overlapping the spinner. Errors still reach your program.

## 40. LoadingAsync

**Use it to:** show a spinner while awaiting asynchronous work. Return to this lesson when you learn `Task`, `async` and `await`.

```csharp
await ConsoleKit.LoadingAsync("Preparing results", async () =>
{
    await Task.Delay(1000);
});
ConsoleKit.Success("The results are ready.");
```

**What you see:** a spinner for about one second, followed by completion messages. In a modern top-level `Program.cs`, `await` can be written directly as shown.

When you learn cancellation, try:

```csharp
using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(1));
try
{
    await ConsoleKit.LoadingAsync("Long task", async () =>
    {
        await Task.Delay(5000, cancellation.Token);
    }, cancellation.Token);
}
catch (OperationCanceledException)
{
    ConsoleKit.Warning("The task was canceled.");
}
```

The task cancels after about one second. Pass the token to both the helper and the work. The helper always waits for the work to end; it cannot force work that ignores cancellation to stop.

## 41. Wait

**Use it to:** display a spinner during a fixed delay.

```csharp
ConsoleKit.Wait("Getting ready", 1000);
ConsoleKit.WriteLine("Ready!");
```

**What you see:** a spinner for about one second. The number is milliseconds: 1000 milliseconds equals one second. Omitting the arguments uses the label Loading and a one-second delay:

```csharp
ConsoleKit.Wait();
```

Use this for a demonstration pause. Use `Loading` for actual work.

## 42. Typewrite

**Use it to:** reveal text gradually, like typing.

```csharp
ConsoleKit.Typewrite("Welcome to the bootcamp!", 40);
ConsoleKit.Typewrite("🚀 Let's build something.", 30);
```

**What you see:** text appearing a little at a time. The number is the delay per text element in milliseconds; smaller is faster. The method adds a newline. With animations disabled or redirected streams, it prints immediately.

## 43. PrintFile

**Use it to:** display a text file's contents.

```csharp
File.WriteAllText("practice-notes.txt", "Arrays start at zero.\nKeep practicing!\n");
ConsoleKit.PrintFile("practice-notes.txt", ConsoleColor.Cyan);
```

**What you see:** two cyan lines from the file. The first call creates or overwrites the sample file in the app's current working directory. Choose another name if you already have that file.

To handle a missing file:

```csharp
try
{
    ConsoleKit.PrintFile("my-notes.txt");
}
catch (IOException exception)
{
    ConsoleKit.Error($"Could not read the file: {exception.Message}");
}
```

`PrintFile` prints literal contents without interpreting color tags or adding a newline. It reads the whole file into memory.

## 44. Clear

**Use it to:** clear the terminal before displaying a new page.

```csharp
ConsoleKit.WriteLine("This is the old page.");
ConsoleKit.Pause("Press Enter to open the new page...");
ConsoleKit.Clear();
ConsoleKit.Title("New page");
```

**What you see:** after Enter, the old page disappears and the new title appears. Clearing does nothing for redirected output. Hosts without screen-clearing support may report an error.

## 45. DisplayWidth

**Use it to:** measure approximately how many terminal cells text needs. This helps when making your own aligned output later.

```csharp
int letters = ConsoleKit.DisplayWidth("Hello");
int emoji = ConsoleKit.DisplayWidth("🚀");
int combined = ConsoleKit.DisplayWidth("e\u0301");
ConsoleKit.WriteLine($"Hello: {letters}");
ConsoleKit.WriteLine($"Rocket: {emoji}");
ConsoleKit.WriteLine($"Combined letter: {combined}");
```

**What you see:** 5, 2 and 1. Emoji and combining letters can contain more characters in memory than the number of screen cells they occupy. Measurement is approximate and depends on the terminal. Use single-line plain text, without markup or escape sequences.

## Settings

Settings affect future calls. Put them near the start of `Program.cs`. Each example below changes one setting and demonstrates its result.

## 46. UseColors

**Use it to:** turn coloring on or off. Default: `true`.

```csharp
ConsoleKit.UseColors = false;
ConsoleKit.WriteLine("Normal terminal color", ConsoleColor.Red);
ConsoleKit.UseColors = true;
ConsoleKit.WriteLine("Red text", ConsoleColor.Red);
```

**What you see:** the first line keeps the terminal's normal color; the second is red. Colors are always disabled when output goes to a file or pipe.

## 47. UseUnicode

**Use it to:** use simple ASCII symbols in a terminal without Unicode support. Default: `true`.

```csharp
ConsoleKit.UseUnicode = false;
ConsoleKit.Panel("Simple borders", "ASCII mode");
ConsoleKit.Bullets("Arrays", "Methods");
ConsoleKit.WriteLine(ConsoleKit.Emoji("rocket"));
ConsoleKit.UseUnicode = true;
```

**What you see:** `+`, `-` and `|` panel borders, hyphen bullets and `[GO]` instead of a rocket. This changes generated symbols; it does not rewrite literal Unicode in your own strings.

## 48. UseEmoji

**Use it to:** choose emoji icons or text fallbacks. Default: `true`.

```csharp
ConsoleKit.UseEmoji = false;
ConsoleKit.Success("Saved.");
ConsoleKit.WriteLine(ConsoleKit.Emojify(":rocket: Start"));
ConsoleKit.UseEmoji = true;
ConsoleKit.Success("Saved.");
```

**What you see:** `[OK] Saved.` and `[GO] Start`, then a success message with an emoji. Literal emoji strings remain unchanged.

## 49. UseAnimations

**Use it to:** keep output still. Default: `true`.

```csharp
ConsoleKit.UseAnimations = false;
ConsoleKit.Typewrite("This prints immediately.", 100);
ConsoleKit.Wait("Still waiting for the work", 300);
ConsoleKit.UseAnimations = true;
```

**What you see:** immediate text and ordinary loading messages. Actual work and timed delays still happen. Animations also require interactive input and output.

## 50. TableStyle

**Use it to:** choose the appearance of tables and matrices. Default: `TableStyle.Modern`.

### Modern

```csharp
ConsoleKit.TableStyle = TableStyle.Modern;
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
```

**What you see:** spacious columns, uppercase headings, one divider and a row count.

### Rounded

```csharp
ConsoleKit.TableStyle = TableStyle.Rounded;
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
```

**What you see:** rounded outer corners and vertical column separators.

### Minimal

```csharp
ConsoleKit.TableStyle = TableStyle.Minimal;
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
```

**What you see:** a compact open table with a header divider and no row count.

### Ascii

```csharp
ConsoleKit.TableStyle = TableStyle.Ascii;
ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
```

**What you see:** a boxed table using `+`, `-` and `|`, suitable for simpler terminals. Restore the default with:

```csharp
ConsoleKit.TableStyle = TableStyle.Modern;
```

`TableStyle` is an enum, meaning a named set of choices: Modern, Rounded, Minimal and Ascii.

## 51. MaxColumnWidth

**Use it to:** keep long table values from making huge tables. Default: 32 cells. Minimum: 4.

```csharp
ConsoleKit.MaxColumnWidth = 16;
ConsoleKit.Table(["Description"], ["This description is longer than sixteen cells."]);
ConsoleKit.MaxColumnWidth = 32;
```

**What you see:** the long value shortened with an ellipsis, or three dots in ASCII mode. Its original value remains unchanged. Tables also try to fit the terminal width, so columns may be narrower than the configured limit.

## 52. AccentColor

**Use it to:** choose your own main UI color. Default: Cyan.

```csharp
ConsoleKit.AccentColor = ConsoleColor.Magenta;
ConsoleKit.Info("My app uses a magenta accent.");
ConsoleKit.BarChart(["Arrays", "Loops"], [10, 6]);
ConsoleKit.AccentColor = ConsoleColor.Cyan;
```

**What you see:** magenta information text and chart bars. Menu numbers, prompts, progress and numeric table cells also use the accent. Explicitly supplied colors are kept.

## 53. MutedColor

**Use it to:** change supporting labels and borders. Default: DarkGray.

```csharp
ConsoleKit.MutedColor = ConsoleColor.Gray;
ConsoleKit.KeyValue("Course", "C# basics");
ConsoleKit.Tree("Project", "Program.cs", "ConsoleKit.cs");
ConsoleKit.MutedColor = ConsoleColor.DarkGray;
```

**What you see:** lighter supporting labels and tree connectors. Try Gray when DarkGray is difficult to read on your terminal background.

## 54. SpinnerStyle

**Use it to:** choose a loading animation. Default: `SpinnerStyle.Dots`.

### Dots

```csharp
ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;
ConsoleKit.Wait("Dots spinner", 1000);
```

**What you see:** rotating Unicode Braille dots.

### Line

```csharp
ConsoleKit.SpinnerStyle = SpinnerStyle.Line;
ConsoleKit.Wait("Line spinner", 1000);
```

**What you see:** a rotating line using `|`, `/`, `-` and backslash.

### Bounce

```csharp
ConsoleKit.SpinnerStyle = SpinnerStyle.Bounce;
ConsoleKit.Wait("Bouncing dot", 1000);
```

**What you see:** a dot moving back and forth. Restore the default with:

```csharp
ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;
```

`SpinnerStyle` is an enum with Dots, Line and Bounce choices. When `UseUnicode` is false, all styles use Line frames. A spinner requires animations enabled and interactive input and output.

## All available console colors

Use these with text, badges and other color arguments. Markup uses the same names, ignoring case.

| Darker colors | Brighter colors |
| --- | --- |
| `ConsoleColor.Black` | `ConsoleColor.Gray` |
| `ConsoleColor.DarkGray` | `ConsoleColor.White` |
| `ConsoleColor.DarkBlue` | `ConsoleColor.Blue` |
| `ConsoleColor.DarkGreen` | `ConsoleColor.Green` |
| `ConsoleColor.DarkCyan` | `ConsoleColor.Cyan` |
| `ConsoleColor.DarkRed` | `ConsoleColor.Red` |
| `ConsoleColor.DarkMagenta` | `ConsoleColor.Magenta` |
| `ConsoleColor.DarkYellow` | `ConsoleColor.Yellow` |

Try every color:

```csharp
ConsoleColor[] colors =
{
    ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen,
    ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta,
    ConsoleColor.DarkYellow, ConsoleColor.Gray, ConsoleColor.DarkGray,
    ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan,
    ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White
};
foreach (ConsoleColor color in colors)
{
    ConsoleColor background = color == ConsoleColor.Black ? ConsoleColor.White : ConsoleColor.Black;
    ConsoleKit.WriteLine($" {color} ", color, background);
}
```

Exact shades depend on the terminal palette.

## Practice project: student scores

This complete `Program.cs` combines arrays, loops, a menu, input and a table. Keep `ConsoleKit.cs` beside it. No student class is needed.

```csharp
ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);
ConsoleKit.Title("Student scores");
string[] names = { "Ali", "Sara", "Reza" };
int[] scores = { 18, 20, 9 };
bool running = true;

while (running)
{
    int choice = ConsoleKit.Menu("Choose an action",
        "Show scores", "Change a score", "Show chart", "Exit");
    if (choice == 1)
    {
        object?[][] rows = new object?[names.Length][];
        for (int i = 0; i < names.Length; i++)
        {
            string status = scores[i] >= 10 ? "Passed" : "Needs practice";
            rows[i] = new object?[] { names[i], scores[i], status };
        }
        ConsoleKit.Table(["Name", "Score", "Status"], rows);
    }
    else if (choice == 2)
    {
        int studentNumber = ConsoleKit.Menu("Choose a student", names);
        int index = studentNumber - 1;
        scores[index] = ConsoleKit.ReadInt("Enter the new score:", 0, 20);
        ConsoleKit.Success("Score updated!");
    }
    else if (choice == 3)
    {
        double[] values = new double[scores.Length];
        for (int i = 0; i < scores.Length; i++)
            values[i] = scores[i];
        ConsoleKit.BarChart(names, values);
    }
    else
        running = false;
}
ConsoleKit.Success("Goodbye!");
```

### Try the app

1. Enter `1` to see the scores.
2. Enter `2`, choose Reza, then enter `15`.
3. Enter `1` again: Reza now has 15 and the status Passed.
4. Enter `3` to compare scores as bars.
5. Enter `4` to exit.

`names[i]` and `scores[i]` refer to the same student. Subtracting 1 converts a menu number to an array index. The table uses a jagged array: an array whose elements are row arrays.

## Common questions

### The name ConsoleKit does not exist

Check that `ConsoleKit.cs` is included in the same project as `Program.cs`, with build action Compile. Spell the class exactly `ConsoleKit` and open the project that contains it.

### Should I add using ConsoleKit?

No. Call it directly, such as `ConsoleKit.Success("Done!");`.

### Square-bracket array examples do not compile

Use a .NET 8+ console project with C# 12 or later. Standard arrays can also use `new[] { 1, 2, 3 }`; mixed table rows can use `new object?[] { "Ali", 18 }`.

### I see question marks or empty boxes

Use a terminal and font that support Unicode and emoji. ConsoleKit selects UTF-8 for interactive output. For a terminal that still cannot display the symbols:

```csharp
ConsoleKit.UseUnicode = false;
ConsoleKit.UseEmoji = false;
ConsoleKit.TableStyle = TableStyle.Ascii;
ConsoleKit.Panel("Simple output", "Ready");
```

### My table does not fit

Use fewer columns, shorter headings, a wider terminal, or a smaller `MaxColumnWidth`. Many columns can still exceed a small window. Panels and charts do not automatically wrap.

### The menu's first item is 1 but my array starts at 0

Menu numbers are for people. Use `options[choice - 1]` to convert to an array index.

### There is no animation or color

Check `UseAnimations` and `UseColors`. Output redirected to a file or pipe is uncolored; animations require both input and output to be interactive. Exact colors depend on the terminal palette.

### Wait still takes time with animations disabled

That setting changes the display. It does not skip delays or actual work.

### My output overlaps the spinner

Collect results inside the loading action and print them after it finishes. Avoid prompts and other output while a spinner is running.

### An input method throws EndOfStreamException

The input stream ended before a valid answer was available. Run interactively or supply enough piped input. This prevents an endless retry loop.

### Emoji alignment looks different on my computer

Width calculation handles common emoji, combined letters and wide text, but fonts and terminals differ. Try text fallbacks or simpler text if alignment is inconsistent.

### Where are the exact defaults and error rules?

Read the [technical reference](GUIDE.md). All 45 public methods, 9 settings and 3 enum types have usage examples in this handbook.
