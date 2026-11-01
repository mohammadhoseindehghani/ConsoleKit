// CONSOLEKIT COOKBOOK
// Keep ConsoleKit.cs beside this file. Copy any example into your own Program.cs.
// Run normally for the full visual tour followed by real input exercises.
// --demo: visual examples only. --all: visual examples plus supplied input answers.
// --help: explain these options. No packages are required.

if (args.Contains("--help"))
{
    Console.WriteLine("dotnet run --project ConsoleKit             Full tour + live input (in a terminal)");
    Console.WriteLine("dotnet run --project ConsoleKit -- --demo   Visual tour; no input prompts");
    Console.WriteLine("dotnet run --project ConsoleKit -- --all    Full tour with supplied input answers");
    return;
}

ResetSettings();
ConsoleKit.Title("CONSOLEKIT / LEARN BY EXAMPLE");
ConsoleKit.Info("Follow the comments in Program.cs. Each section is a small example.");

ShowText();
ShowLayout();
ShowArrays();
ShowTables();
ShowListsAndCharts();
ShowMarkupAndEmoji();
ShowSettings();
await ShowFilesAndAnimations();
ShowTextWidth();

bool interactive = !Console.IsInputRedirected && !Console.IsOutputRedirected;
if (args.Contains("--all"))
{
    // Supplied answers make every input example runnable without waiting for a person.
    // The original input stream is always restored. Existing user files are not used.
    ConsoleKit.Info("Input examples use supplied answers: Sara, 19, 18, yes, 2, 1, and 1,3.");
    TextReader originalInput = Console.In;
    try
    {
        Console.SetIn(new DemoAnswersReader("Sara\n19\n18\ny\n2\n1\n1,3\n\n"));
        ShowInput(false);
    }
    finally { Console.SetIn(originalInput); }
}
else if (interactive && !args.Contains("--demo"))
{
    ShowInput(true);
}
else
{
    ConsoleKit.Info("Input examples are in ShowInput below. Use --all to preview their results.");
}

ResetSettings();
ConsoleKit.Success("Tour complete! See docs/LEARNERS.md for one lesson per feature.");

static void ShowText()
{
    ConsoleKit.Title("01 / Text and every console color");

    // WriteLine ends the line. Its optional colors are foreground, then background.
    ConsoleKit.WriteLine("Hello, bootcamp!");
    ConsoleKit.WriteLine("Green text", ConsoleColor.Green);
    ConsoleKit.WriteLine("Highlighted text", ConsoleColor.Black, ConsoleColor.Yellow);
    ConsoleKit.WriteLine(123);
    ConsoleKit.WriteLine(); // Empty line

    // Write keeps the cursor on the same line, so you can mix colors.
    ConsoleKit.Write("Score: ", ConsoleColor.Gray);
    ConsoleKit.Write(20, ConsoleColor.Cyan);
    ConsoleKit.WriteLine(" / 20");

    // Status helpers print their own icon, color and newline.
    ConsoleKit.Success("Saved successfully.");
    ConsoleKit.Error("This is a sample error message; it does not stop the app.");
    ConsoleKit.Warning("Check your answer.");
    ConsoleKit.Info("Array indexes start at zero.");

    // Enum.GetValues gives us every one of the 16 ConsoleColor choices.
    foreach (ConsoleColor color in Enum.GetValues<ConsoleColor>())
    {
        ConsoleColor background = color == ConsoleColor.Black ? ConsoleColor.White : ConsoleColor.Black;
        ConsoleKit.WriteLine($" {color} ", color, background);
    }
    ConsoleKit.Rainbow("A rainbow greeting!");
}

static void ShowLayout()
{
    ConsoleKit.Title("02 / Headings, panels, labels and alignment");
    ConsoleKit.Rule(); // Default horizontal divider, 60 cells wide
    ConsoleKit.Rule("Results", 45, ConsoleColor.Magenta);
    ConsoleKit.Center("Centered in 50 cells", 50);
    ConsoleKit.Center("Centered using the terminal width");
    ConsoleKit.Panel("Arrays\nLoops\nMethods", "Today's lessons");
    ConsoleKit.Panel("Keep practicing.", color: ConsoleColor.Green);

    // Badges do not add a newline; finish the row yourself.
    ConsoleKit.Badge("NEW", ConsoleColor.Black, ConsoleColor.Cyan);
    ConsoleKit.Write("  ");
    ConsoleKit.Badge("PASSED", ConsoleColor.Black, ConsoleColor.Green);
    ConsoleKit.WriteLine();

    ConsoleKit.KeyValue("Student", "Sara");
    ConsoleKit.KeyValue("Score", 20);
    ConsoleKit.KeyValue("Completed exercises", 12, 24); // Custom label width
}

static void ShowArrays()
{
    ConsoleKit.Title("03 / Arrays: indexed and inline");
    int[] numbers = { 1, 2, 3, 4 };
    string[] names = { "ali", "sina", "kamran" };

    // Indexed format is useful when learning zero-based array indexes.
    ConsoleKit.PrintArray(numbers, "Numbers with indexes");
    ConsoleKit.PrintArray(names, "Names with indexes");

    // Inline format adds brackets, commas, and quotes around strings automatically.
    ConsoleKit.PrintArrayInline(numbers); // [1, 2, 3, 4]
    ConsoleKit.PrintArrayInline(names);   // ["ali", "sina", "kamran"]
    ConsoleKit.PrintArrayInline(numbers, ConsoleColor.Magenta); // Optional color

    // Empty collections, booleans, characters, nulls, and lists are supported too.
    ConsoleKit.PrintArrayInline(Array.Empty<int>());              // []
    ConsoleKit.PrintArrayInline(new object?[] { 1, "Ali", true, null });
    ConsoleKit.PrintArrayInline(new[] { 'A', 'B' });              // ["A", "B"]
    ConsoleKit.PrintArrayInline(new List<int> { 10, 20, 30 });
    ConsoleKit.PrintArrayInline(new[] { "He said \"Hi\"", "Line 1\nLine 2" });
    ConsoleKit.PrintArray(Array.Empty<int>(), "An empty indexed array");

    // A two-dimensional array can be displayed as a matrix table.
    int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };
    ConsoleKit.PrintMatrix(grid);
}

static void ShowTables()
{
    ConsoleKit.Title("04 / Tables and every table style");

    // Custom headings followed by rows. Every row has three cells, like the headings.
    ConsoleKit.Table(["Student", "Score", "Passed"],
        ["Ali", 18, true], ["Sara", 20, true], ["Reza", 9, false]);

    // All four TableStyle choices affect Table, WriteTable and PrintMatrix.
    foreach (TableStyle style in Enum.GetValues<TableStyle>())
    {
        ConsoleKit.Title(style + " table");
        ConsoleKit.TableStyle = style;
        ConsoleKit.Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]);
    }
    ConsoleKit.TableStyle = TableStyle.Modern;

    // Turn a simple array into one Value column automatically.
    ConsoleKit.WriteTable(new[] { "Apple", "Banana", "Orange" });

    // Later, when you learn objects: public properties become columns automatically.
    var students = new[]
    {
        new { Name = "Ali", Score = 18, Passed = true },
        new { Name = "Sara", Score = 20, Passed = true }
    };
    ConsoleKit.WriteTable(students);
    ConsoleKit.WriteTable(students, ConsoleColor.Yellow, ConsoleColor.White);

    // Null values remain visible. Empty data gets a friendly message.
    ConsoleKit.Table(["Name", "Note"], ["Ali", null]);
    ConsoleKit.WriteTable(Array.Empty<int>());

    // Limit long columns. The displayed value is shortened, not the original string.
    ConsoleKit.MaxColumnWidth = 16;
    ConsoleKit.Table(["Description"], ["A long value that will be shortened on screen."]);
    ConsoleKit.MaxColumnWidth = 32;
}

static void ShowListsAndCharts()
{
    ConsoleKit.Title("05 / Lists, steps, trees and bar charts");
    ConsoleKit.Bullets("Arrays", "Loops", "Methods");
    ConsoleKit.NumberedList("Read the task", "Write a method", "Test your answer");
    ConsoleKit.Steps(2, "Read data", "Process data", "Save results"); // Current step starts at 1
    ConsoleKit.Tree("MyProject", "Program.cs", "ConsoleKit.cs", "notes.txt");
    ConsoleKit.BarChart(["Arrays", "Loops", "Methods"], [12, 9, 6], 25);
    ConsoleKit.BarChart(["No work yet"], [0]); // Zero is a valid chart value
}

static void ShowMarkupAndEmoji()
{
    ConsoleKit.Title("06 / Inline colors, literal brackets and emoji");
    ConsoleKit.MarkupLine("[cyan]Name:[/] Sara | [green]Score:[/] 20");
    ConsoleKit.Markup("[cyan]Same line:[/] "); // Does not add a newline
    ConsoleKit.MarkupLine("[green]Saved [yellow]3 files[/] successfully.[/]");

    // Escape user-supplied brackets before putting text inside markup.
    string answer = "scores[0]";
    ConsoleKit.MarkupLine("[cyan]You entered:[/] " + ConsoleKit.EscapeMarkup(answer));
    ConsoleKit.MarkupLine("Literal brackets: [[green]]");

    // Emoji returns a symbol; Emojify replaces aliases inside a longer string.
    ConsoleKit.WriteLine(ConsoleKit.Emoji("rocket") + " Starting!");
    ConsoleKit.WriteLine(ConsoleKit.Emojify(":book: Learn :bulb: Practice :trophy: Improve"));
    ConsoleKit.MarkupLine(":party: [magenta]Markup expands emoji aliases too![/]");
    ConsoleKit.WriteLine("\U0001f680 A literal emoji works in ordinary strings too.");

    // Every built-in emoji alias, with a text fallback when emoji are disabled.
    ConsoleKit.WriteLine(ConsoleKit.Emojify(
        ":check: :cross: :warning: :info: :rocket: :star: :fire: :heart: :smile: " +
        ":party: :book: :folder: :file: :save: :clock: :bulb: :trophy: :target:"));
}

static void ShowSettings()
{
    ConsoleKit.Title("07 / Every theme and display setting");
    foreach (ConsoleTheme theme in Enum.GetValues<ConsoleTheme>())
    {
        ConsoleKit.ApplyTheme(theme); // Ocean, Forest, Sunset, Mono
        ConsoleKit.Info(theme + " accent");
    }
    ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);

    ConsoleKit.AccentColor = ConsoleColor.Magenta;
    ConsoleKit.Info("Custom magenta accent");
    ConsoleKit.MutedColor = ConsoleColor.Gray;
    ConsoleKit.KeyValue("Lighter label", "Easier to read");

    ConsoleKit.UseColors = false;
    ConsoleKit.WriteLine("Plain text even though red was requested", ConsoleColor.Red);
    ConsoleKit.UseColors = true;

    ConsoleKit.UseEmoji = false;
    ConsoleKit.Success("Text fallback instead of an emoji");
    ConsoleKit.UseEmoji = true;

    ConsoleKit.UseUnicode = false;
    ConsoleKit.Panel("ASCII borders and symbols", "Simple terminal");
    ConsoleKit.Bullets("ASCII bullet");
    ConsoleKit.UseUnicode = true;

    ConsoleKit.UseAnimations = false;
    ConsoleKit.Typewrite("Animations off: this prints immediately.", 100);
    ResetSettings(); // Restore all nine settings before the next examples
}

static async Task ShowFilesAndAnimations()
{
    ConsoleKit.Title("08 / File output, real-work loading and animations");
    string path = Path.GetTempFileName(); // Use a temporary file, not a learner's existing file
    try
    {
        File.WriteAllText(path, "Arrays start at zero.\nKeep practicing!\n");
        ConsoleKit.PrintFile(path, ConsoleColor.Cyan); // Literal contents; no markup parsing

        string contents = "";
        ConsoleKit.Loading("Reading a file", () => { contents = File.ReadAllText(path); });
        ConsoleKit.WriteLine("Read " + contents.Length + " characters."); // Print after the spinner ends

        // Async methods are for later lessons about Task, async and await.
        await ConsoleKit.LoadingAsync("Reading asynchronously", async () =>
        {
            contents = await File.ReadAllTextAsync(path);
        });
        ConsoleKit.WriteLine("Async read: " + contents.Length + " characters.");
    }
    finally { File.Delete(path); } // Always remove our own temporary sample

    // Show each spinner choice; Wait is only a timed demonstration delay.
    foreach (SpinnerStyle style in Enum.GetValues<SpinnerStyle>())
    {
        ConsoleKit.SpinnerStyle = style; // Line, Dots, Bounce
        ConsoleKit.Wait(style + " spinner", 250);
    }
    ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;

    // You own the loop and the work. Completed starts at 0 and finishes at total.
    for (int completed = 0; completed <= 5; completed++)
    {
        ConsoleKit.Progress(completed, 5, "Processing", 20);
        await Task.Delay(40); // Replace with your actual work
    }
    ConsoleKit.Typewrite("Finished!", 15);

    // Cancellation must be passed to the work as well as the loading helper.
    using var cancellation = new CancellationTokenSource();
    cancellation.Cancel();
    try
    {
        await ConsoleKit.LoadingAsync("Canceled work", () => Task.Delay(100, cancellation.Token), cancellation.Token);
    }
    catch (OperationCanceledException) { ConsoleKit.Warning("Cancellation handled by our program."); }
}

static void ShowTextWidth()
{
    ConsoleKit.Title("09 / Measuring visible text");
    ConsoleKit.KeyValue("Hello", ConsoleKit.DisplayWidth("Hello")); // 5
    ConsoleKit.KeyValue("Rocket emoji", ConsoleKit.DisplayWidth("\U0001f680")); // 2
    ConsoleKit.KeyValue("Combined letter", ConsoleKit.DisplayWidth("e\u0301")); // 1
}

static void ShowInput(bool live)
{
    ConsoleKit.Title("10 / Every input helper and screen control");
    string name = ConsoleKit.ReadText("Your name?");
    int age = ConsoleKit.ReadInt("Your age?", 1, 120);
    double score = ConsoleKit.ReadDouble("Your score?", 0, 20);
    bool save = ConsoleKit.Confirm("Save changes?"); // y/yes/n/no
    ConsoleKit.WriteLine($"Name: {name}, age: {age}, score: {score}, save: {save}");

    // Menu returns a one-based NUMBER. Your code decides what it does.
    int choice = ConsoleKit.Menu("Choose an action", "Add student", "Show scores", "Exit");
    ConsoleKit.Info("Menu returned " + choice);

    // Select returns the chosen TEXT directly.
    string language = ConsoleKit.Select("Choose a language", "C#", "Java", "Python");
    ConsoleKit.Info("Selected " + language);

    // MultiSelect returns one-based numbers, for example [1, 3] for input 1,3.
    string[] topics = { "Arrays", "Loops", "Methods" };
    int[] selected = ConsoleKit.MultiSelect("Choose topics", topics);
    ConsoleKit.PrintArrayInline(selected);
    foreach (int number in selected) ConsoleKit.WriteLine(topics[number - 1]);

    ConsoleKit.Pause("Press Enter to finish these examples...");
    if (live && ConsoleKit.Confirm("Clear the screen to demonstrate a new page?"))
    {
        ConsoleKit.Clear(); // Ask first so you can keep the tour's output visible if you prefer
        ConsoleKit.Title("New page");
    }
    else ConsoleKit.Info("Clear() is demonstrated above when you choose to clear the screen.");
}

static void ResetSettings()
{
    // Every public setting and its default value, in one place for easy copying.
    ConsoleKit.UseColors = true;
    ConsoleKit.UseUnicode = true;
    ConsoleKit.UseEmoji = true;
    ConsoleKit.UseAnimations = true;
    ConsoleKit.TableStyle = TableStyle.Modern;
    ConsoleKit.MaxColumnWidth = 32;
    ConsoleKit.AccentColor = ConsoleColor.Cyan;
    ConsoleKit.MutedColor = ConsoleColor.DarkGray;
    ConsoleKit.SpinnerStyle = SpinnerStyle.Dots;
}

// Only used by --all: echo supplied answers so the transcript reads like a real session.
// Your own app can use ConsoleKit input helpers directly without this demo helper.
sealed class DemoAnswersReader : StringReader
{
    public DemoAnswersReader(string answers) : base(answers) { }

    public override string? ReadLine()
    {
        string? answer = base.ReadLine();
        if (answer is not null) ConsoleKit.WriteLine(answer.Length == 0 ? "<Enter>" : answer, ConsoleColor.Gray);
        return answer;
    }
}
