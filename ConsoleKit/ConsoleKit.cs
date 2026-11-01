using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

public enum TableStyle { Rounded, Minimal, Ascii, Modern }
public enum ConsoleTheme { Ocean, Forest, Sunset, Mono }
public enum SpinnerStyle { Line, Dots, Bounce }

/// <summary>Beginner-friendly console helpers. Copy this file into any .NET 8+ console project.</summary>
public static class ConsoleKit
{
    private static readonly object OutputLock = new();
    /// <summary>Set false to disable colors (for example, when saving output).</summary>
    public static bool UseColors { get; set; } = true;
    public static TableStyle TableStyle { get; set; } = global::TableStyle.Modern;
    public static bool UseUnicode { get; set; } = true;
    public static bool UseEmoji { get; set; } = true;
    public static bool UseAnimations { get; set; } = true;
    public static ConsoleColor AccentColor { get; set; } = ConsoleColor.Cyan;
    public static ConsoleColor MutedColor { get; set; } = ConsoleColor.DarkGray;
    public static SpinnerStyle SpinnerStyle { get; set; } = global::SpinnerStyle.Dots;
    private static int maxColumnWidth = 32;
    public static int MaxColumnWidth
    {
        get => maxColumnWidth;
        set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 4); maxColumnWidth = value; }
    }

    static ConsoleKit()
    {
        // Windows legacy code pages cannot encode rounded corners.
        if (!Console.IsOutputRedirected) Console.OutputEncoding = new System.Text.UTF8Encoding(false);
    }

    public static void Write(object? text, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        lock (OutputLock)
        {
            bool colored = UseColors && !Console.IsOutputRedirected;
            var oldForeground = Console.ForegroundColor;
            var oldBackground = Console.BackgroundColor;
            try
            {
                if (colored && foreground.HasValue) Console.ForegroundColor = foreground.Value;
                if (colored && background.HasValue) Console.BackgroundColor = background.Value;
                Console.Write(text);
            }
            finally
            {
                if (colored)
                {
                    Console.ForegroundColor = oldForeground;
                    Console.BackgroundColor = oldBackground;
                }
            }
        }
    }

    public static void WriteLine(object? text = null, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        lock (OutputLock) Write($"{text}{Environment.NewLine}", foreground, background);
    }

    public static void Success(string text) => WriteLine($"{Emoji("check")} {text}", ConsoleColor.Green);
    public static void Error(string text) => WriteLine($"{Emoji("cross")} {text}", ConsoleColor.Red);
    public static void Warning(string text) => WriteLine($"{Emoji("warning")} {text}", ConsoleColor.Yellow);
    public static void Info(string text) => WriteLine($"{Emoji("info")} {text}", AccentColor);

    public static void Rule(string title = "", int width = 60, ConsoleColor color = ConsoleColor.DarkGray)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        string label = string.IsNullOrEmpty(title) ? "" : $" {SingleLine(title)} ";
        WriteLine(label + new string(UseUnicode ? '\u2500' : '-', Math.Max(0, width - DisplayWidth(label))), color);
    }

    public static void Title(string text)
    {
        WriteLine();
        WriteLine("  " + SingleLine(text), ConsoleColor.White);
        WriteLine();
    }

    public static void Panel(string text, string title = "", ConsoleColor color = ConsoleColor.Cyan)
    {
        ArgumentNullException.ThrowIfNull(text);
        string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Select(SingleLine).ToArray();
        int width = Math.Max(lines.Max(DisplayWidth), DisplayWidth(SingleLine(title)));
        string edge = new(UseUnicode ? '\u2500' : '-', width + 4);
        string vertical = UseUnicode ? "\u2502" : "|";
        WriteLine((UseUnicode ? "\u256d" : "+") + edge + (UseUnicode ? "\u256e" : "+"), color);
        if (title.Length > 0)
        {
            Write(vertical, color);
            Write("  " + PadCell(SingleLine(title), width) + "  ", ConsoleColor.White);
            WriteLine(vertical, color);
            WriteLine((UseUnicode ? "\u251c" : "+") + edge + (UseUnicode ? "\u2524" : "+"), color);
        }
        foreach (string line in lines)
        {
            Write(vertical, color);
            Write("  " + PadCell(line, width) + "  ");
            WriteLine(vertical, color);
        }
        WriteLine((UseUnicode ? "\u2570" : "+") + edge + (UseUnicode ? "\u256f" : "+"), color);
    }

    /// <summary>Print array indexes beside values. Works with lists, too.</summary>
    public static void PrintArray(IEnumerable items, string title = "Array")
    {
        ArgumentNullException.ThrowIfNull(items);
        Title(title);
        int index = 0;
        foreach (object? item in items)
        {
            Write($"[{index++}] ", MutedColor);
            WriteLine(item ?? "(null)", AccentColor);
        }
        if (index == 0) WriteLine("(no data)");
    }

    /// <summary>Print a flat array on one line: [1, 2, 3] or ["Ali", "Sara"].</summary>
    public static void PrintArrayInline(IEnumerable items, ConsoleColor? color = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        string text = "[" + string.Join(", ", items.Cast<object?>().Select(FormatInlineItem)) + "]";
        WriteLine(text, color ?? AccentColor);
    }

    private static string FormatInlineItem(object? item) => item switch
    {
        null => "null",
        string text => QuoteInline(text),
        char character => QuoteInline(character.ToString()),
        bool boolean => boolean ? "true" : "false",
        IFormattable number => number.ToString(null, CultureInfo.InvariantCulture) ?? "null",
        _ => QuoteInline(item.ToString() ?? "")
    };

    private static string QuoteInline(string text)
    {
        var result = new StringBuilder("\"");
        foreach (char character in text)
            result.Append(character switch
            {
                '"' => "\\\"", '\\' => "\\\\", '\n' => "\\n", '\r' => "\\r", '\t' => "\\t",
                _ when char.IsControl(character) => "\\u" + ((int)character).ToString("x4"),
                _ => character.ToString()
            });
        return result.Append('"').ToString();
    }

    /// <summary>Print numbers, strings, or objects. Public properties and fields become columns.</summary>
    public static void WriteTable(IEnumerable items, ConsoleColor? headerColor = null, ConsoleColor? rowColor = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        var values = items.Cast<object?>().ToArray();
        object? first = values.FirstOrDefault(value => value is not null);
        if (values.Length == 0) { WriteLine("(no data)"); return; }
        if (first is null || IsSimple(first.GetType()))
        {
            TableCore(["Value"], values.Select(value => new[] { Format(value) }), headerColor, rowColor);
            return;
        }
        var members = GetMembers(first.GetType());
        if (members.Count == 0)
        {
            TableCore(["Value"], values.Select(value => new[] { Format(value) }), headerColor, rowColor);
            return;
        }
        TableCore(members.Select(member => member.Name).ToArray(), values.Select(value => members.Select(member =>
        {
            if (value is null) return "(null)";
            try
            {
                return Format(member is PropertyInfo property ? property.GetValue(value) : ((FieldInfo)member).GetValue(value));
            }
            catch (Exception exception) when (exception is TargetException or TargetInvocationException or ArgumentException)
            {
                return "(unavailable)";
            }
        }).ToArray()), headerColor, rowColor);
    }

    /// <summary>Build a table without classes: Table(["Name", "Score"], ["Ali", 18], ["Sara", 20]).</summary>
    public static void Table(string[] headers, params object?[][] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        TableCore(headers, rows.Select(row => row?.Select(Format).ToArray()
            ?? throw new ArgumentException("Rows cannot be null.", nameof(rows))), null, null);
    }

    /// <summary>Print a two-dimensional array as a table.</summary>
    public static void PrintMatrix<T>(T[,] matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        int columns = matrix.GetLength(1);
        if (matrix.GetLength(0) == 0 || columns == 0) { WriteLine("(no data)"); return; }
        string[] headers = Enumerable.Range(0, columns).Select(index => $"[{index}]").ToArray();
        var rows = new List<string[]>();
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            var cells = new string[columns];
            for (int column = 0; column < columns; column++) cells[column] = Format(matrix[row, column]);
            rows.Add(cells);
        }
        TableCore(headers, rows, null, null);
    }

    private static void TableCore(string[] headers, IEnumerable<string[]> rows, ConsoleColor? headerColor, ConsoleColor? rowColor)
    {
        ArgumentNullException.ThrowIfNull(headers);
        if (headers.Length == 0) throw new ArgumentException("Provide at least one column.", nameof(headers));
        var data = rows.ToArray();
        if (data.Any(row => row.Length != headers.Length))
            throw new ArgumentException("Every row must have the same number of cells as the headers.", nameof(rows));
        var names = headers.Select(SingleLine).ToArray();
        data = data.Select(row => row.Select(SingleLine).ToArray()).ToArray();
        int[] widths = names.Select((name, index) => Math.Max(DisplayWidth(name), data.Select(row => DisplayWidth(row[index])).DefaultIfEmpty().Max())).ToArray();
        bool ascii = !UseUnicode || TableStyle == global::TableStyle.Ascii;
        bool modern = TableStyle == global::TableStyle.Modern;
        bool minimal = modern || TableStyle == global::TableStyle.Minimal;
        string vertical = minimal ? " " : ascii ? "|" : "│";
        bool[] numeric = Enumerable.Range(0, names.Length).Select(column =>
            data.Length > 0 && data.Any(row => row[column] != "(null)") &&
            data.All(row => row[column] == "(null)" || decimal.TryParse(row[column], NumberStyles.Number, CultureInfo.CurrentCulture, out _))).ToArray();
        if (modern)
        {
            names = names.Select(name => name.ToUpperInvariant()).ToArray();
            for (int column = 0; column < widths.Length; column++)
                widths[column] = Math.Max(Math.Max(widths[column], DisplayWidth(names[column])), numeric[column] ? 7 : 12);
        }
        for (int column = 0; column < widths.Length; column++) widths[column] = Math.Min(widths[column], MaxColumnWidth);
        int available = TerminalWidth() - (widths.Length * 5 + 1);
        while (widths.Sum() > available && widths.Max() > 4)
        {
            int widest = Array.IndexOf(widths, widths.Max());
            widths[widest]--;
        }
        void Border(string left, string join, string right) => WriteLine(left +
            string.Join(join, widths.Select(width => new string(ascii ? '-' : '─', width + 4))) + right, MutedColor);
        void Row(string[] cells, bool header, int index = 0)
        {
            Write(vertical, MutedColor);
            for (int column = 0; column < cells.Length; column++)
            {
                string cell = cells[column];
                string padded = PadCell(Truncate(cell, widths[column]), widths[column], numeric[column] && (!header || modern));
                ConsoleColor color = header ? headerColor ?? (modern ? ConsoleColor.Gray : AccentColor)
                    : rowColor ?? (cell == "True" ? ConsoleColor.Green : cell == "False" ? ConsoleColor.Red
                    : cell == "Passed" ? ConsoleColor.Green : cell == "Needs practice" ? ConsoleColor.Yellow
                    : cell == "(null)" ? MutedColor : numeric[column] ? AccentColor
                    : index % 2 == 0 ? ConsoleColor.White : ConsoleColor.Gray);
                Write("  " + padded + "  ", color);
                Write(vertical, MutedColor);
            }
            WriteLine();
        }
        lock (OutputLock)
        {
            if (!minimal) Border(ascii ? "+" : "╭", ascii ? "+" : "┬", ascii ? "+" : "╮");
            Row(names, true);
            Border(minimal ? " " : ascii ? "+" : "├", minimal ? (ascii ? "-" : "─") : ascii ? "+" : "┼", minimal ? " " : ascii ? "+" : "┤");
            for (int index = 0; index < data.Length; index++) Row(data[index], false, index);
            if (!minimal) Border(ascii ? "+" : "╰", ascii ? "+" : "┴", ascii ? "+" : "╯");
            if (modern)
            {
                WriteLine();
                WriteLine($"   {data.Length} {(data.Length == 1 ? "row" : "rows")}", MutedColor);
                WriteLine();
            }
        }
    }

    public static void ApplyTheme(ConsoleTheme theme)
    {
        AccentColor = theme switch
        {
            ConsoleTheme.Ocean => ConsoleColor.Cyan,
            ConsoleTheme.Forest => ConsoleColor.Green,
            ConsoleTheme.Sunset => ConsoleColor.Magenta,
            ConsoleTheme.Mono => ConsoleColor.White,
            _ => throw new ArgumentOutOfRangeException(nameof(theme))
        };
        MutedColor = ConsoleColor.DarkGray;
    }

    private static readonly Dictionary<string, (string Symbol, string Fallback)> EmojiNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["check"] = ("\u2705", "[OK]"), ["cross"] = ("\u274c", "[X]"),
        ["warning"] = ("\u26a0\ufe0f", "[!]"), ["info"] = ("\u2139\ufe0f", "[i]"),
        ["rocket"] = ("\U0001f680", "[GO]"), ["star"] = ("\u2b50", "[*]"),
        ["fire"] = ("\U0001f525", "[HOT]"), ["heart"] = ("\u2764\ufe0f", "<3"),
        ["smile"] = ("\U0001f604", ":)"), ["party"] = ("\U0001f389", "[YAY]"),
        ["book"] = ("\U0001f4da", "[BOOK]"), ["folder"] = ("\U0001f4c1", "[DIR]"),
        ["file"] = ("\U0001f4c4", "[FILE]"), ["save"] = ("\U0001f4be", "[SAVE]"),
        ["clock"] = ("\u23f3", "[WAIT]"), ["bulb"] = ("\U0001f4a1", "[TIP]"),
        ["trophy"] = ("\U0001f3c6", "[WIN]"), ["target"] = ("\U0001f3af", "[GOAL]")
    };

    public static string Emoji(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (!EmojiNames.TryGetValue(name.Trim(':'), out var emoji))
            throw new ArgumentException($"Unknown emoji name: {name}", nameof(name));
        return UseEmoji && UseUnicode ? emoji.Symbol : emoji.Fallback;
    }

    public static string Emojify(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        // Unknown aliases are left intact; ordinary text is never treated as markup.
        return System.Text.RegularExpressions.Regex.Replace(text, ":([a-zA-Z]+):", match =>
            EmojiNames.ContainsKey(match.Groups[1].Value) ? Emoji(match.Groups[1].Value) : match.Value);
    }

    public static string EscapeMarkup(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return text.Replace("[", "[[").Replace("]", "]]");
    }

    public static void Markup(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        // Parse everything before writing, so invalid tags cannot leave partial output.
        var segments = new List<(string Text, ConsoleColor? Color)>();
        var colors = new Stack<ConsoleColor>();
        var buffer = new StringBuilder();
        void Flush()
        {
            if (buffer.Length == 0) return;
            segments.Add((Emojify(buffer.ToString()), colors.Count > 0 ? colors.Peek() : null));
            buffer.Clear();
        }
        for (int index = 0; index < text.Length; index++)
        {
            char character = text[index];
            if ((character == '[' || character == ']') && index + 1 < text.Length && text[index + 1] == character)
            { buffer.Append(character); index++; continue; }
            if (character != '[') { buffer.Append(character); continue; }
            int end = text.IndexOf(']', index + 1);
            if (end < 0) throw new FormatException("A markup tag is missing its closing ]. Use [[ for a literal [.");
            string tag = text[(index + 1)..end];
            Flush();
            if (tag == "/")
            {
                if (colors.Count == 0) throw new FormatException("A closing [/] tag has no opening color tag.");
                colors.Pop();
            }
            else if (Enum.TryParse<ConsoleColor>(tag, true, out var color) && Enum.IsDefined(color) && !int.TryParse(tag, out _)) colors.Push(color);
            else throw new FormatException($"Unknown color tag [{tag}]. Use a ConsoleColor name such as green or cyan.");
            index = end;
        }
        if (colors.Count != 0) throw new FormatException("Close each color tag with [/].");
        Flush();
        lock (OutputLock) foreach (var segment in segments) Write(segment.Text, segment.Color);
    }

    public static void MarkupLine(string text)
    {
        lock (OutputLock) { Markup(text); WriteLine(); }
    }

    public static void Badge(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.DarkBlue)
        => Write(" " + SingleLine(text) + " ", foreground, background);

    public static void KeyValue(string key, object? value, int keyWidth = 16)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(keyWidth);
        Write("  " + PadCell(SingleLine(key), keyWidth) + "  ", MutedColor);
        WriteLine(value, AccentColor);
    }

    public static void Bullets(params string[] items) => PrintList(items, false);
    public static void NumberedList(params string[] items) => PrintList(items, true);
    private static void PrintList(string[] items, bool numbered)
    {
        ArgumentNullException.ThrowIfNull(items);
        for (int index = 0; index < items.Length; index++)
        {
            Write(numbered ? $"  {index + 1}. " : UseUnicode ? "  \u2022 " : "  - ", AccentColor);
            WriteLine(SingleLine(items[index]));
        }
    }

    public static void Steps(int current, params string[] steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        if (current < 1 || current > steps.Length) throw new ArgumentOutOfRangeException(nameof(current), "Current step starts at 1 and must exist.");
        for (int index = 0; index < steps.Length; index++)
        {
            string marker = index + 1 < current ? (UseUnicode ? "\u2713" : "+") : index + 1 == current ? ">" : "-";
            Write($"  {marker} {index + 1}. ", index + 1 < current ? ConsoleColor.Green : index + 1 == current ? AccentColor : MutedColor);
            WriteLine(SingleLine(steps[index]), index + 1 == current ? ConsoleColor.White : ConsoleColor.Gray);
        }
    }

    public static void Tree(string root, params string[] children)
    {
        ArgumentNullException.ThrowIfNull(children);
        WriteLine("  " + SingleLine(root), AccentColor);
        for (int index = 0; index < children.Length; index++)
        {
            Write(UseUnicode ? index == children.Length - 1 ? "  \u2514\u2500 " : "  \u251c\u2500 " : "  +- ", MutedColor);
            WriteLine(SingleLine(children[index]));
        }
    }

    public static void Center(string text, int width = 0, ConsoleColor? color = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        text = SingleLine(text);
        if (width == 0) width = TerminalWidth();
        WriteLine(new string(' ', Math.Max(0, (width - DisplayWidth(text)) / 2)) + text, color ?? AccentColor);
    }

    public static void Rainbow(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ConsoleColor[] colors = [ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta];
        int index = 0;
        lock (OutputLock)
        {
            foreach (string element in TextElements(text)) Write(element, colors[index++ % colors.Length]);
            WriteLine();
        }
    }

    public static void BarChart(string[] labels, double[] values, int width = 25)
    {
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        if (labels.Length != values.Length) throw new ArgumentException("Provide one value for each label.");
        if (values.Any(value => !double.IsFinite(value) || value < 0)) throw new ArgumentException("Chart values must be finite and nonnegative.", nameof(values));
        if (values.Length == 0) { WriteLine("(no data)"); return; }
        string[] names = labels.Select(SingleLine).ToArray();
        int labelWidth = Math.Min(24, names.Max(DisplayWidth));
        double maximum = values.Max();
        for (int index = 0; index < values.Length; index++)
        {
            int count = maximum == 0 ? 0 : (int)Math.Round(values[index] / maximum * width);
            Write("  " + PadCell(Truncate(names[index], labelWidth), labelWidth) + "  ");
            Write(new string(UseUnicode ? '\u2588' : '#', count).PadRight(width), AccentColor);
            WriteLine($"  {values[index]}", ConsoleColor.Gray);
        }
    }

    public static void PrintFile(string path, ConsoleColor? color = null)
        => Write(File.ReadAllText(path), color);

    /// <summary>Return the chosen text. Numbered menus work with keyboard and redirected input alike.</summary>
    public static string Select(string title, params string[] options) => options[Menu(title, options) - 1];

    public static int[] MultiSelect(string title, params string[] options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Length == 0) throw new ArgumentException("Provide at least one option.", nameof(options));
        Title(title);
        NumberedList(options);
        while (true)
        {
            string answer = ReadText("Choose numbers separated by commas (Enter for none):", true);
            if (answer.Length == 0) return [];
            string[] parts = answer.Split(',');
            var choices = new List<int>();
            bool valid = true;
            foreach (string part in parts)
            {
                if (!int.TryParse(part.Trim(), out int choice) || choice < 1 || choice > options.Length) { valid = false; break; }
                if (!choices.Contains(choice)) choices.Add(choice);
            }
            if (valid) return choices.ToArray();
            Warning($"Use numbers between 1 and {options.Length}, separated by commas.");
        }
    }

    public static void Clear()
    {
        if (!Console.IsOutputRedirected) Console.Clear();
    }

    /// <summary>Approximate terminal cells, handling combining marks, wide text and emoji clusters.</summary>
    public static int DisplayWidth(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return TextElements(text).Sum(ElementWidth);
    }

    private static IEnumerable<string> TextElements(string text)
    {
        var elements = StringInfo.GetTextElementEnumerator(text);
        while (elements.MoveNext()) yield return elements.GetTextElement();
    }

    private static int ElementWidth(string element)
    {
        int width = 0;
        foreach (var rune in element.EnumerateRunes())
        {
            var category = Rune.GetUnicodeCategory(rune);
            if (category is UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or UnicodeCategory.Format or UnicodeCategory.Control) continue;
            int code = rune.Value;
            bool wide = code is >= 0x1100 and <= 0x115f or >= 0x2e80 and <= 0xa4cf
                or >= 0xac00 and <= 0xd7a3 or >= 0xf900 and <= 0xfaff or >= 0xfe10 and <= 0xfe6f
                or >= 0xff01 and <= 0xff60 or >= 0xffe0 and <= 0xffe6 or >= 0x1f000 and <= 0x1faff
                or >= 0x20000 and <= 0x3ffff or 0x2705 or 0x274c or 0x2b50 or 0x23f3;
            width = Math.Max(width, wide ? 2 : 1);
        }
        return element.Contains('\ufe0f') || element.Contains('\u20e3') ? 2 : width;
    }

    private static string PadCell(string text, int width, bool right = false)
    {
        string padding = new(' ', Math.Max(0, width - DisplayWidth(text)));
        return right ? padding + text : text + padding;
    }

    private static string Truncate(string text, int width)
    {
        if (DisplayWidth(text) <= width) return text;
        string suffix = UseUnicode ? "\u2026" : "...";
        int remaining = width - suffix.Length;
        var result = new StringBuilder();
        foreach (string element in TextElements(text))
        {
            int cells = ElementWidth(element);
            if (cells > remaining) break;
            result.Append(element);
            remaining -= cells;
        }
        return result + suffix;
    }

    private static int TerminalWidth()
    {
        if (!Console.IsOutputRedirected)
        {
            try { if (Console.WindowWidth > 0) return Console.WindowWidth; }
            catch (IOException) { }
            catch (PlatformNotSupportedException) { }
        }
        return 100;
    }

    public static string ReadText(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Write($"{prompt} ", AccentColor);
            string input = ReadInput().Trim();
            if (allowEmpty || input.Length > 0) return input;
            Warning("Please enter a value.");
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        if (min > max) throw new ArgumentException("Minimum cannot be greater than maximum.");
        while (true)
        {
            Write($"{prompt} ", AccentColor);
            if (int.TryParse(ReadInput(), out int number) && number >= min && number <= max) return number;
            Warning($"Enter a whole number between {min} and {max}.");
        }
    }

    public static double ReadDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)
    {
        if (double.IsNaN(min) || double.IsNaN(max) || min > max) throw new ArgumentException("Provide a valid minimum and maximum.");
        while (true)
        {
            Write($"{prompt} ", AccentColor);
            if (double.TryParse(ReadInput(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double number)
                && double.IsFinite(number) && number >= min && number <= max) return number;
            Warning($"Enter a number between {min} and {max} (using your system's decimal separator).");
        }
    }

    public static bool Confirm(string prompt)
    {
        while (true)
        {
            string answer = ReadText($"{prompt} (y/n)").ToLowerInvariant();
            if (answer is "y" or "yes") return true;
            if (answer is "n" or "no") return false;
            Warning("Enter y or n.");
        }
    }

    /// <summary>Show a numbered menu and return the selected number, starting at 1.</summary>
    public static int Menu(string title, params string[] options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Length == 0) throw new ArgumentException("Provide at least one menu option.", nameof(options));
        Title(title);
        for (int i = 0; i < options.Length; i++)
        {
            Write($"  {i + 1,2}  ", AccentColor);
            WriteLine(options[i], ConsoleColor.White);
        }
        return ReadInt("Choose:", 1, options.Length);
    }

    public static void Pause(string message = "Press Enter to continue...")
    {
        WriteLine(message, MutedColor);
        ReadInput();
    }

    /// <summary>Draw a progress bar. Call in your own loop with the number of completed items.</summary>
    public static void Progress(int current, int total, string label = "Progress", int width = 25)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(total);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        if (current < 0 || current > total) throw new ArgumentOutOfRangeException(nameof(current), "Current must be between zero and total.");
        double fraction = (double)current / total;
        int filled = (int)(fraction * width);
        string line = $"{SingleLine(label)} [{new string(UseUnicode ? '\u2501' : '#', filled)}{new string(UseUnicode ? '\u2500' : '-', width - filled)}] {fraction,4:P0}";
        lock (OutputLock)
        {
            if (CanAnimate) Write("\r", ConsoleColor.Green);
            Write(line, AccentColor);
            if (!CanAnimate || current == total) WriteLine();
        }
    }

    /// <summary>Run real work while showing a spinner; errors still reach the caller.</summary>
    public static void Loading(string message, Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        LoadingAsync(message, () => Task.Run(work)).GetAwaiter().GetResult();
    }

    public static async Task LoadingAsync(string message, Func<Task> work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        cancellationToken.ThrowIfCancellationRequested();
        message = SingleLine(message);
        if (!CanAnimate)
        {
            Info(message);
            await work().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            Success($"{message} - done");
            return;
        }
        // The work owns cancellation; we always await it so it never continues unnoticed.
        using var stop = new CancellationTokenSource();
        Task animation = AnimateAsync(message, stop.Token);
        bool completed = false;
        try
        {
            await work().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            completed = true;
        }
        finally
        {
            stop.Cancel();
            await animation.ConfigureAwait(false);
            WriteLine();
        }
        if (completed) Success($"{message} - done");
    }

    /// <summary>Show an animated wait for a fixed duration, in milliseconds.</summary>
    public static void Wait(string message = "Loading", int milliseconds = 1000)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(milliseconds);
        LoadingAsync(message, () => Task.Delay(milliseconds)).GetAwaiter().GetResult();
    }

    public static void Typewrite(string text, int millisecondsPerCharacter = 25)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(millisecondsPerCharacter);
        if (!CanAnimate) { WriteLine(text); return; }
        foreach (string character in TextElements(text))
        {
            Write(character);
            Thread.Sleep(millisecondsPerCharacter);
        }
        WriteLine();
    }

    private static async Task AnimateAsync(string message, CancellationToken token)
    {
        string[] frames = !UseUnicode || SpinnerStyle == global::SpinnerStyle.Line ? ["|", "/", "-", "\\"]
            : SpinnerStyle == global::SpinnerStyle.Bounce ? [".  ", " . ", "  .", " . "]
            : ["\u280b", "\u2819", "\u2839", "\u2838", "\u283c", "\u2834", "\u2826", "\u2827", "\u2807", "\u280f"];
        int frame = 0;
        try
        {
            while (true)
            {
                Write($"\r{frames[frame++ % frames.Length]} {message}", AccentColor);
                await Task.Delay(100, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
    }

    private static bool CanAnimate => UseAnimations && !Console.IsOutputRedirected && !Console.IsInputRedirected;
    private static string ReadInput() => Console.ReadLine() ?? throw new EndOfStreamException("Console input ended before a value was entered.");
    private static string Format(object? value) => value?.ToString() ?? "(null)";
    private static string SingleLine(string? text) => string.Concat((text ?? "").Replace("\t", "    ").Select(character => char.IsControl(character) ? ' ' : character));
    private static bool IsSimple(Type type) => type.IsPrimitive || type.IsEnum || type.IsValueType || type == typeof(string);

    private static List<MemberInfo> GetMembers(Type type)
    {
        var levels = new Stack<Type>();
        for (Type? current = type; current is not null && current != typeof(object); current = current.BaseType) levels.Push(current);
        var members = new List<MemberInfo>();
        foreach (Type level in levels)
        {
            var declared = level.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(member => member is FieldInfo || member is PropertyInfo property && property.GetMethod?.IsPublic == true && property.GetIndexParameters().Length == 0)
                .OrderBy(member => member.MetadataToken);
            foreach (var member in declared)
            {
                int existing = members.FindIndex(item => item.Name == member.Name);
                if (existing >= 0) members[existing] = member;
                else members.Add(member);
            }
        }
        return members;
    }
}

