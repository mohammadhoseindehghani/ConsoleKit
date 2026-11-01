using System.Globalization;

// Dependency-free regression checks: dotnet run --project ConsoleKit.Tests
var originalOut = Console.Out;
var originalIn = Console.In;
using var output = new StringWriter();
Console.SetOut(output);
ConsoleKit.UseColors = false;
ConsoleKit.UseEmoji = false;
int passed = 0;
try
{
    Check("default modern appearance", () =>
    {
        Assert(ConsoleKit.TableStyle == TableStyle.Modern, "Modern should be the default");
        ConsoleKit.Table(["Student", "Score"], ["Sara", 20], ["Ali", 9]);
        Contains("STUDENT"); Contains("SCORE"); Contains("2 rows");
        Assert(!output.ToString().Contains('│'), "Modern tables should have no vertical grid");
        Assert(!output.ToString().Contains('?'), "Table borders must not contain replacement question marks");
    });
    ConsoleKit.TableStyle = TableStyle.Rounded;
    Check("array indexes and nulls", () =>
    {
        ConsoleKit.PrintArray(new object?[] { 42, null, "last" });
        Contains("[0] 42"); Contains("[1] (null)"); Contains("[2] last");
    });
    Check("simple and empty tables", () =>
    {
        ConsoleKit.WriteTable(new[] { 1, 200 });
        Contains("│  Value  │"); Contains("│    200  │");
        ConsoleKit.WriteTable(Array.Empty<int>()); Contains("(no data)");
    });
    Check("inline numeric, string and empty arrays", () =>
    {
        ConsoleKit.PrintArrayInline(new[] { 1, 2, 3, 4 });
        Assert(output.ToString() == "[1, 2, 3, 4]" + Environment.NewLine, "Numbers should print on exactly one bracketed line");
        output.GetStringBuilder().Clear();
        ConsoleKit.PrintArrayInline(new[] { "ali", "sina", "kamran" });
        Assert(output.ToString() == "[\"ali\", \"sina\", \"kamran\"]" + Environment.NewLine, "Strings should be quoted");
        ConsoleKit.PrintArrayInline(Array.Empty<int>());
        Contains("[]");
        ConsoleKit.PrintArrayInline(new List<int> { 10, 20 });
        Contains("[10, 20]");
        Throws<ArgumentNullException>(() => ConsoleKit.PrintArrayInline(null!));
    });
    Check("inline quoting, nulls, booleans and culture", () =>
    {
        ConsoleKit.PrintArrayInline(new object?[] { 1, "Ali", true, false, null, 'A' });
        Contains("[1, \"Ali\", true, false, null, \"A\"]");
        output.GetStringBuilder().Clear();
        string[] values = { "He said \"Hi\"", "C:\\notes", "one\ntwo\r\t\u001b", "\U0001f680" };
        ConsoleKit.PrintArrayInline(values);
        Contains("[\"He said \\\"Hi\\\"\", \"C:\\\\notes\", \"one\\ntwo\\r\\t\\u001b\", \"\U0001f680\"]");
        Assert(output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length == 1, "Control characters must not split an inline array across lines");
        Assert(values[0] == "He said \"Hi\"", "Formatting must not change the array");
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            ConsoleKit.PrintArrayInline(new[] { 1.5, 2.75 });
            Contains("[1.5, 2.75]");
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
    });
    Check("inheritance, fields, indexers and throwing getters", () =>
    {
        ConsoleKit.WriteTable(new[] { new Student() });
        Contains("Name"); Contains("Score"); Contains("Age"); Contains("(unavailable)");
        Assert(!output.ToString().Contains("Item"), "Indexer should be excluded");
        Assert(output.ToString().IndexOf("Name", StringComparison.Ordinal) < output.ToString().IndexOf("Score", StringComparison.Ordinal), "Base members should appear first");
    });
    Check("custom tables and multiline cells", () =>
    {
        ConsoleKit.Table(["Name", "Score"], ["Ali\nTest", 18]);
        Contains("Ali Test"); Contains("18");
        Throws<ArgumentException>(() => ConsoleKit.Table(["A", "B"], [1]));
        Throws<ArgumentException>(() => ConsoleKit.Table([]));
    });
    Check("matrix dimensions", () =>
    {
        ConsoleKit.PrintMatrix(new[,] { { 1, 2 }, { 3, 4 } });
        Contains("│    1  │    2  │"); Contains("│    3  │    4  │");
    });
    Check("modern table geometry, styles and numeric alignment", () =>
    {
        try
        {
            foreach (var style in Enum.GetValues<TableStyle>())
            {
                output.GetStringBuilder().Clear();
                ConsoleKit.TableStyle = style;
                ConsoleKit.Table(["Name", "Score"], ["Ali", 2], ["Sara", 100]);
                var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var tableLines = style == TableStyle.Modern ? lines[..^1] : lines;
                Assert(tableLines.Select(line => line.Length).Distinct().Count() == 1, $"All rows and borders must align in {style}");
                if (style == TableStyle.Rounded)
                {
                    Assert(lines[0].StartsWith('╭') && lines[^1].EndsWith('╯'), "Rounded corners should be the default appearance");
                    Contains("│  Ali   │      2  │");
                    Contains("│  Sara  │    100  │");
                }
                if (style == TableStyle.Ascii) Assert(output.ToString().All(character => character <= 127), "ASCII fallback must use only ASCII");
                if (style == TableStyle.Minimal) Assert(!output.ToString().Contains('│'), "Minimal style should omit vertical borders");
            }
        }
        finally { ConsoleKit.TableStyle = TableStyle.Rounded; }
    });
    Check("panel alignment and tab expansion", () =>
    {
        ConsoleKit.Panel("One\tTwo\nThree", "Lessons");
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert(lines.Select(line => line.Length).Distinct().Count() == 1, "All panel borders and rows should align");
        Assert(!output.ToString().Contains('\t'), "Tabs must expand before measuring");
    });
    Check("input retries, bounds, menu numbering and EOF", () =>
    {
        Console.SetIn(new StringReader("oops\n0\n2\n"));
        Assert(ConsoleKit.Menu("Pick", "A", "B") == 2, "Menu returns a one-based selection");
        Console.SetIn(new StringReader("\n Sara \n"));
        Assert(ConsoleKit.ReadText("Name") == "Sara", "Trim and reject empty input");
        Console.SetIn(new StringReader("maybe\nyes\n"));
        Assert(ConsoleKit.Confirm("Continue?"), "Confirm retries");
        Console.SetIn(new StringReader(""));
        Throws<EndOfStreamException>(() => ConsoleKit.ReadInt("Number"));
        Throws<ArgumentException>(() => ConsoleKit.ReadInt("Number", 10, 1));
    });
    Check("floating point validation", () =>
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Console.SetIn(new StringReader("NaN\nInfinity\n2.5\n"));
            Assert(ConsoleKit.ReadDouble("Number", 0, 10) == 2.5, "Reject non-finite numbers");
        }
        finally { CultureInfo.CurrentCulture = culture; }
    });
    Check("progress endpoints and invalid totals", () =>
    {
        ConsoleKit.UseUnicode = false;
        ConsoleKit.Progress(0, 10, "Copy", 5);
        ConsoleKit.Progress(10, 10, "Copy", 5);
        Contains("[-----]"); Contains("[#####]");
        Throws<ArgumentOutOfRangeException>(() => ConsoleKit.Progress(0, 0));
        Throws<ArgumentOutOfRangeException>(() => ConsoleKit.Progress(11, 10));
        ConsoleKit.UseUnicode = true;
    });
    Check("loading runs work and propagates errors", () =>
    {
        bool ran = false;
        ConsoleKit.Loading("Work", () => ran = true);
        Assert(ran, "Work should execute"); Contains("done");
        output.GetStringBuilder().Clear();
        Throws<InvalidOperationException>(() => ConsoleKit.Loading("Fail", () => throw new InvalidOperationException("expected")));
        Assert(!output.ToString().Contains("done"), "Failed work must not report success");
        Throws<InvalidOperationException>(() => ConsoleKit.LoadingAsync("Fail async", () => Task.FromException(new InvalidOperationException())).GetAwaiter().GetResult());
    });
    Check("emoji aliases and text fallback", () =>
    {
        Assert(ConsoleKit.Emoji("check") == "[OK]", "Disabled emoji should use a text fallback");
        Assert(ConsoleKit.Emojify(":ROCKET: :unknown:") == "[GO] :unknown:", "Aliases ignore case and preserve unknown names");
        ConsoleKit.UseEmoji = true;
        Assert(ConsoleKit.Emoji(":rocket:") == "\U0001f680", "Emoji can be requested with colons");
        ConsoleKit.UseUnicode = false;
        Assert(ConsoleKit.Emoji("rocket") == "[GO]", "ASCII mode must disable emoji aliases");
        ConsoleKit.UseUnicode = true;
        ConsoleKit.UseEmoji = false;
        Throws<ArgumentException>(() => ConsoleKit.Emoji("missing"));
    });
    Check("nested markup, literal brackets and invalid input", () =>
    {
        ConsoleKit.MarkupLine("[green]Saved [cyan]file[/]![/] :check:");
        Contains("Saved file! [OK]");
        ConsoleKit.MarkupLine(ConsoleKit.EscapeMarkup("scores[0] = [green]"));
        Contains("scores[0] = [green]");
        output.GetStringBuilder().Clear();
        Throws<FormatException>(() => ConsoleKit.Markup("before [unknown]oops[/]"));
        Assert(output.ToString().Length == 0, "Invalid markup should not write partial output");
        Throws<FormatException>(() => ConsoleKit.Markup("[red]unclosed"));
        Throws<FormatException>(() => ConsoleKit.Markup("[/]"));
    });
    Check("Unicode widths and emoji table alignment", () =>
    {
        Assert(ConsoleKit.DisplayWidth("e\u0301") == 1, "Combining marks do not add width");
        Assert(ConsoleKit.DisplayWidth("\U0001f469\u200d\U0001f4bb") == 2, "Joined emoji should occupy a single wide cluster");
        Assert(ConsoleKit.DisplayWidth("\u4f60\u597d") == 4, "CJK characters are wide");
        ConsoleKit.Table(["Name", "Icon"], ["\u4f60\u597d", "\U0001f680"], ["Ali", "e\u0301"]);
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert(lines.Select(ConsoleKit.DisplayWidth).Distinct().Count() == 1, "Emoji and wide text should align with borders");
    });
    Check("long cell limits and safe control characters", () =>
    {
        ConsoleKit.MaxColumnWidth = 8;
        ConsoleKit.Table(["Description"], ["a long description with \u001b[31m control text"]);
        Assert(!output.ToString().Contains('\u001b'), "Table cells must not emit terminal escapes");
        Contains("\u2026");
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert(lines.All(line => ConsoleKit.DisplayWidth(line) == 14), "Cells must respect the configured width");
        Throws<ArgumentOutOfRangeException>(() => ConsoleKit.MaxColumnWidth = 3);
        ConsoleKit.MaxColumnWidth = 32;
    });
    Check("charts, lists, tree and steps", () =>
    {
        ConsoleKit.UseUnicode = false;
        ConsoleKit.BarChart(["A", "B"], [10, 5], 10);
        Contains("##########"); Contains("#####     ");
        ConsoleKit.BarChart(["zero"], [0]);
        Throws<ArgumentException>(() => ConsoleKit.BarChart(["bad"], [double.NaN]));
        Throws<ArgumentException>(() => ConsoleKit.BarChart(["bad"], [-1]));
        Throws<ArgumentException>(() => ConsoleKit.BarChart(["bad"], []));
        ConsoleKit.Bullets("Arrays", "Loops"); Contains("- Arrays");
        ConsoleKit.NumberedList("First", "Second"); Contains("2. Second");
        ConsoleKit.Tree("Project", "Program.cs"); Contains("+- Program.cs");
        ConsoleKit.Steps(2, "Start", "Work", "Finish"); Contains("+ 1. Start"); Contains("> 2. Work");
        Throws<ArgumentOutOfRangeException>(() => ConsoleKit.Steps(0, "Start"));
        ConsoleKit.UseUnicode = true;
    });
    Check("multiple selection retries and deduplication", () =>
    {
        Console.SetIn(new StringReader("4\n1,no\n3,1,3\n"));
        Assert(ConsoleKit.MultiSelect("Pick", "A", "B", "C").SequenceEqual(new[] { 3, 1 }), "Selections should be one-based, unique, and preserve order");
        Console.SetIn(new StringReader("\n"));
        Assert(ConsoleKit.MultiSelect("Pick", "A").Length == 0, "Empty input means no selection");
        Console.SetIn(new StringReader("2\n"));
        Assert(ConsoleKit.Select("Pick", "A", "B") == "B", "Select should return the chosen text");
    });
    Check("themes, badges, alignment and grapheme typing", () =>
    {
        ConsoleKit.ApplyTheme(ConsoleTheme.Forest);
        Assert(ConsoleKit.AccentColor == ConsoleColor.Green, "Theme should set accent color");
        ConsoleKit.ApplyTheme(ConsoleTheme.Ocean);
        Throws<ArgumentOutOfRangeException>(() => ConsoleKit.ApplyTheme((ConsoleTheme)999));
        ConsoleKit.Center("Hi", 10); Contains("    Hi");
        ConsoleKit.Badge("NEW"); Contains(" NEW ");
        ConsoleKit.KeyValue("Score", 20); Contains("Score"); Contains("20");
        ConsoleKit.Rainbow("\U0001f680 Ready"); Contains("\U0001f680 Ready");
        ConsoleKit.Typewrite("e\u0301\U0001f680", 0); Contains("e\u0301\U0001f680");
    });
    Check("file output", () =>
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Line one\nLine two");
            ConsoleKit.PrintFile(path); Contains("Line one\nLine two");
        }
        finally { File.Delete(path); }
    });
    Check("async loading completion and cancellation", () =>
    {
        bool ran = false;
        ConsoleKit.LoadingAsync("Async work", async () => { await Task.Yield(); ran = true; }).GetAwaiter().GetResult();
        Assert(ran, "Async work must finish before returning");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        ran = false;
        Throws<OperationCanceledException>(() => ConsoleKit.LoadingAsync("Canceled", () => { ran = true; return Task.CompletedTask; }, cancellation.Token).GetAwaiter().GetResult());
        Assert(!ran, "Already canceled work must not start");
    });
}
finally
{
    Console.SetOut(originalOut);
    Console.SetIn(originalIn);
}
Console.WriteLine($"Passed {passed} regression checks.");

void Check(string name, Action test)
{
    output.GetStringBuilder().Clear();
    try { test(); passed++; }
    catch (Exception error) { throw new Exception($"Check failed: {name}", error); }
}
void Contains(string expected) => Assert(output.ToString().Contains(expected), $"Missing output: {expected}");
void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}

class Person { public string Name { get; set; } = "Sara"; }
class Student : Person
{
    public int Score { get; set; } = 20;
    public int Age = 19;
    public string Broken => throw new InvalidOperationException();
    public string this[int index] => "skip";
}
