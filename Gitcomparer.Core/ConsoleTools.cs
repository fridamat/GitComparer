using System;
using System.ComponentModel;
using System.Reflection;

namespace GitComparer.Core
{
    public class ConsoleTools
    {
        public static void PrintError(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void PrintInfo(string format, params object[] args)
        {
            var currColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(format, args);
            Console.ForegroundColor = currColor;
        }

        public static void PrintListOfAlternatives(string description, string[] alternatives)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("{0}:", description);
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var alternative in alternatives)
            {
                Console.WriteLine("[] {0}", alternative);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Prompt(string description, out string value)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(description);
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Gray;
            value = Console.ReadLine();
        }

        public static void Select(string description, string[] alternatives, out string value)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(description);
            Console.Write(": ");
            int initialCursorLeft = Console.CursorLeft;
            int initialCursorTop = Console.CursorTop;
            int currentAlternative = 0;

        printCurrent:
            Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new String(' ', Console.BufferWidth - initialCursorLeft));
            Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            Console.Write(alternatives[currentAlternative]);

        handleInput:
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    currentAlternative = (currentAlternative - 1 + alternatives.Length) % alternatives.Length;
                    goto printCurrent;
                case ConsoleKey.DownArrow:
                    currentAlternative = (currentAlternative + 1) % alternatives.Length;
                    goto printCurrent;
                case ConsoleKey.Enter:
                    value = alternatives[currentAlternative];
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    return;
                default:
                    goto handleInput;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void SelectEnum(string description, Enum[] alternatives, out Enum value)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(description);
            Console.Write(": ");
            int initialCursorLeft = Console.CursorLeft;
            int initialCursorTop = Console.CursorTop;
            int currentAlternative = 0;

        printCurrent:
            Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new String(' ', Console.BufferWidth - initialCursorLeft));
            Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            Console.Write(GetEnumDescription(alternatives[currentAlternative]));

        handleInput:
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    currentAlternative = (currentAlternative - 1 + alternatives.Length) % alternatives.Length;
                    goto printCurrent;
                case ConsoleKey.DownArrow:
                    currentAlternative = (currentAlternative + 1) % alternatives.Length;
                    goto printCurrent;
                case ConsoleKey.Enter:
                    value = alternatives[currentAlternative];
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    return;
                default:
                    goto handleInput;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (enumAttributes.Length > 0)
            {
                return enumAttributes[0].Description;
            }
            return value.ToString();
        }
    }
}
