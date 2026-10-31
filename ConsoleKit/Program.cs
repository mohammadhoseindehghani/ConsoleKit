// Run: dotnet run --project ConsoleKit
// Start with one focused example. Explore other features from the menu below.
ShowResults();
if (args.Contains("--all"))
{
    ShowArrays();
    ShowTables();
    ShowColors();
    ShowExtras();
}

if (args.Contains("--demo") || args.Contains("--all") || Console.IsInputRedirected || Console.IsOutputRedirected)
{
    ConsoleKit.Info("Run in an interactive terminal to explore the menu.");
    return;
}

while (true)
{
    int choice = ConsoleKit.Menu("Try a feature", "Arrays and matrices", "Tables", "Colors and messages",
        "Loading and progress", "Read input", "Student results", "Emoji, markup and charts", "Selection helpers", "Exit");
    if (choice == 9) break;
    switch (choice)
    {
        case 1: ShowArrays(); break;
        case 2: ShowTables(); break;
        case 3: ShowColors(); break;
        case 4: ShowLoading(); break;
        case 5:
            string name = ConsoleKit.ReadText("What is your name?");
            int age = ConsoleKit.ReadInt("Your age?", 1, 120);
            ConsoleKit.Success($"Hello {name}! You are {age} years old.");
            if (ConsoleKit.Confirm("Do you enjoy C#?")) ConsoleKit.Success("Keep practicing!");
            break;
        case 6: ShowResults(); break;
        case 7: ShowExtras(); break;
        case 8:
            string theme = ConsoleKit.Select("Choose a theme", "Ocean", "Forest", "Sunset", "Mono");
            ConsoleKit.ApplyTheme(Enum.Parse<ConsoleTheme>(theme));
            int[] topics = ConsoleKit.MultiSelect("Topics to practice", "Arrays", "Loops", "Methods");
            ConsoleKit.Success($"Selected {topics.Length} topics.");
            break;
    }
    ConsoleKit.Pause();
}

static void ShowExtras()
{
    ConsoleKit.Title("Color and personality");
    ConsoleKit.MarkupLine("  :rocket: [cyan]ConsoleKit[/] / [green]ready to build[/]");
    ConsoleKit.WriteLine(ConsoleKit.Emojify("  :book: Learn   :bulb: Practice   :trophy: Improve"));
    ConsoleKit.Badge(" NEW ", ConsoleColor.Black, ConsoleColor.Cyan);
    ConsoleKit.Write("  ");
    ConsoleKit.Badge(" PASSED ", ConsoleColor.Black, ConsoleColor.Green);
    ConsoleKit.WriteLine();
    ConsoleKit.Rainbow("  Make your console colorful!");
    ConsoleKit.KeyValue("Course", "C# fundamentals");
    ConsoleKit.KeyValue("Week", 4);
    ConsoleKit.Title("Learning progress");
    ConsoleKit.Steps(2, "Learn arrays", "Practice methods", "Build a project");
    ConsoleKit.Title("Exercises completed");
    ConsoleKit.BarChart(["Arrays", "Loops", "Methods"], [12, 9, 6]);
    ConsoleKit.Title("Project structure");
    ConsoleKit.Tree("MyProject", "Program.cs", "ConsoleKit.cs", "notes.txt");
    ConsoleKit.Title("Practice checklist");
    ConsoleKit.Bullets("Print an array", "Create a table", "Add a menu");
    ConsoleKit.NumberedList("Read the problem", "Write a method", "Try different inputs");
    ConsoleKit.Panel("Small methods. Clear output.\nKeep practicing every day.", "Tip");
    ConsoleKit.Center("You are making progress!", 60);
}

static void ShowResults()
{
    ConsoleKit.Title("BOOTCAMP / STUDENTS");
    ConsoleKit.WriteLine("   C# fundamentals", ConsoleColor.Cyan);
    ConsoleKit.WriteLine("   Week 04   /   Arrays and methods", ConsoleColor.Gray);
    ConsoleKit.WriteLine();
    ConsoleKit.TableStyle = TableStyle.Modern;
    // Headers first, then one row per student. No classes or configuration needed.
    ConsoleKit.Table(
        ["Student", "Exercises", "Score / 20", "Status"],
        ["Sara Ahmadi", 12, 20, "Passed"],
        ["Ali Karimi", 10, 18, "Passed"],
        ["Nika Moradi", 11, 19, "Passed"],
        ["Reza Amiri", 6, 9, "Needs practice"]
    );
    ConsoleKit.WriteLine("   3 passed   /   1 needs practice", ConsoleColor.Gray);
    ConsoleKit.WriteLine();
}

static void ShowArrays()
{
    int[] scores = [12, 18, 20, 16];
    ConsoleKit.PrintArray(scores, "Scores with indexes");
    ConsoleKit.WriteTable(new[] { "Apple", "Banana", "Orange" });
    int[,] numbers = { { 1, 2, 3 }, { 4, 5, 6 } };
    ConsoleKit.PrintMatrix(numbers);
}

static void ShowTables()
{
    ConsoleKit.Title("Modern table - default style");
    ConsoleKit.TableStyle = TableStyle.Modern;
    ConsoleKit.Table(["Name", "Score", "Passed"], ["Ali", 18, true], ["Sara", 20, true]);
    // 1. Create a modern table with headers and rows. No class is needed.
    ConsoleKit.Title("Rounded table - optional border");
    ConsoleKit.TableStyle = TableStyle.Rounded;
    ConsoleKit.Table(
        ["Name", "Score", "Passed"],
        ["Ali", 18, true],
        ["Sara", 20, true],
        ["Reza", 9, false]
    );

    // 2. Use the same simple calls with a different appearance.
    ConsoleKit.Title("Minimal table - open layout");
    ConsoleKit.TableStyle = TableStyle.Minimal;
    ConsoleKit.Table(
        ["Course", "Students", "Active"],
        ["C# basics", 24, true],
        ["Arrays", 8, false]
    );

    // 3. An optional fallback for terminals without Unicode borders.
    ConsoleKit.Title("ASCII table - basic terminal support");
    ConsoleKit.TableStyle = TableStyle.Ascii;
    ConsoleKit.Table(["Product", "Price"], ["Notebook", 45], ["Pen", 8]);

    // Restore the default for the rest of the program.
    ConsoleKit.TableStyle = TableStyle.Rounded;

    // 4. Arrays of objects automatically become tables, too.
    ConsoleKit.Title("Objects become columns automatically");
    var students = new[]
    {
        new { Name = "Ali", Score = 18, Passed = true },
        new { Name = "Sara", Score = 20, Passed = true },
        new { Name = "Reza", Score = 9, Passed = false }
    };
    ConsoleKit.WriteTable(students);
    ConsoleKit.TableStyle = TableStyle.Modern;
}

static void ShowColors()
{
    ConsoleKit.Title("Messages");
    ConsoleKit.WriteLine("Choose any ConsoleColor!", ConsoleColor.Magenta);
    ConsoleKit.WriteLine("Highlighted text", ConsoleColor.Black, ConsoleColor.Yellow);
    ConsoleKit.Success("Saved successfully.");
    ConsoleKit.Warning("Please check your input.");
    ConsoleKit.Error("Something went wrong.");
    ConsoleKit.Info("Arrays start at index zero.");
}

static void ShowLoading()
{
    ConsoleKit.Wait("Getting ready", 800);
    // Replace Thread.Sleep with real work, such as reading a file.
    ConsoleKit.Loading("Doing some work", () => Thread.Sleep(800));
    for (int completed = 0; completed <= 10; completed++)
    {
        ConsoleKit.Progress(completed, 10, "Processing");
        Thread.Sleep(100);
    }
    ConsoleKit.Typewrite("All done!", 40);
}
