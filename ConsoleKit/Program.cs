// START HERE
// These examples use variables, arrays, for, while and if.
// Run: dotnet run --project ConsoleKit
// Read from top to bottom. You can copy one section into your own Program.cs.

// 1. Print text. WriteLine moves to the next line.
ConsoleKit.Title("My first console app");
ConsoleKit.WriteLine("Hello!");
ConsoleKit.WriteLine("Green text", ConsoleColor.Green);
ConsoleKit.WriteLine("Yellow background", ConsoleColor.Black, ConsoleColor.Yellow);

// Write stays on the same line.
int score = 18;
ConsoleKit.Write("My score: ");
ConsoleKit.WriteLine(score, ConsoleColor.Cyan);

// 2. Use if to choose a message.
if (score >= 10)
{
    ConsoleKit.Success("You passed!");
}
else
{
    ConsoleKit.Error("Try again.");
}
ConsoleKit.Warning("Remember to practice.");
ConsoleKit.Info("Array indexes start at zero.");
ConsoleKit.Rule();

// 3. Print a numeric array in both formats.
int[] numbers = { 1, 2, 3, 4 };
ConsoleKit.PrintArray(numbers, "Numbers with indexes");
// [0] 1
// [1] 2
// [2] 3
// [3] 4

ConsoleKit.PrintArrayInline(numbers);
// [1, 2, 3, 4]

// 4. Strings are quoted automatically in the inline format.
string[] names = { "ali", "sina", "kamran" };
ConsoleKit.PrintArrayInline(names);
// ["ali", "sina", "kamran"]

ConsoleKit.PrintArray(names, "Names with indexes");
ConsoleKit.PrintArrayInline(names, ConsoleColor.Magenta);

// 5. Work with the array using a for loop.
int total = 0;
for (int i = 0; i < numbers.Length; i++)
{
    total = total + numbers[i];
}
ConsoleKit.Write("The total is: ");
ConsoleKit.WriteLine(total); // 10

// Print your own index and value using the same loop you already know.
for (int i = 0; i < names.Length; i++)
{
    ConsoleKit.Write(i, ConsoleColor.Yellow);
    ConsoleKit.Write(" -> ");
    ConsoleKit.WriteLine(names[i], ConsoleColor.Cyan);
}

// 6. Turn an array into a table with one call.
ConsoleKit.Title("Names as a table");
ConsoleKit.WriteTable(names);
ConsoleKit.Title("Numbers as a table");
ConsoleKit.WriteTable(numbers);

// Change the table style by choosing a different name.
ConsoleKit.TableStyle = TableStyle.Rounded;
ConsoleKit.WriteTable(names);
ConsoleKit.TableStyle = TableStyle.Modern;

// 7. Add simple lists and a panel.
ConsoleKit.Bullets("Arrays", "Loops", "If");
ConsoleKit.NumberedList("Read the task", "Write the code", "Run the program");
ConsoleKit.Panel("Practice a little every day.", "Tip");
ConsoleKit.KeyValue("Score", score);

// 8. Print an emoji. You can use a name or paste an emoji into your text.
ConsoleKit.WriteLine(ConsoleKit.Emoji("rocket") + " Keep learning!");
ConsoleKit.WriteLine("You are doing well! \U0001f389");

// 9. A progress bar uses an ordinary for loop.
// The first number is completed work; the second is the total.
for (int completed = 0; completed <= 4; completed++)
{
    ConsoleKit.Progress(completed, 4, "Practice");
}

// 10. Optional input practice.
// Change false to true when you want to type answers into the terminal.
bool tryInput = false;
if (tryInput)
{
    string name = ConsoleKit.ReadText("Your name?");
    int age = ConsoleKit.ReadInt("Your age?", 1, 120);
    ConsoleKit.WriteLine("Hello, " + name);
    ConsoleKit.WriteLine("Your age is " + age);

    // A menu returns a number starting at 1.
    // A while loop keeps the menu open until you choose Exit.
    bool running = true;
    while (running)
    {
        int choice = ConsoleKit.Menu("Choose", "Show numbers", "Show names", "Exit");
        if (choice == 1)
        {
            ConsoleKit.PrintArrayInline(numbers);
        }
        else if (choice == 2)
        {
            ConsoleKit.PrintArrayInline(names);
        }
        else
        {
            running = false;
        }
    }
}
ConsoleKit.Success("Finished! Try changing the values in the arrays.");

// More features are documented one by one in docs/LEARNERS.md.
// The separate examples/AdvancedDemo project covers every feature for later lessons.
