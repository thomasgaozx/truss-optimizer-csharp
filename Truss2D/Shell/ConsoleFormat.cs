using System;
using System.Linq;

namespace Truss2D.Shell
{
    /// <summary>
    /// Gives colourful output to console.
    /// </summary>
    public static class ConsoleFormat
    {

        public const string LineBreak = "======================================================";

        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        private static void DefaultColour()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void SetColourAndPrompt(ConsoleColor color, string content)
        {
            Console.ForegroundColor = color;
            Console.Write(content);
            DefaultColour();
        }

        private static void SetColourAndPrint(ConsoleColor color, string content)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            DefaultColour();
        }

        /// <summary>
        /// Write message to console <see cref="Console.WriteLine"/> (default color - green). 
        /// </summary>
        public static void Print(string content)
        {
            SetColourAndPrint(ConsoleColor.Green, content);
        }

        public static void Prompt(string content)
        {
            SetColourAndPrompt(ConsoleColor.Green, content);
        }

        /// <summary>
        /// Write a line of message to the console in red.
        /// </summary>
        /// <param name="content"></param>
        public static void PrintDanger(string content)
        {
            SetColourAndPrint(ConsoleColor.Red, content);
        }

        public static void PromptDanger(string content)
        {
            SetColourAndPrompt(ConsoleColor.Red, content);
        }

        /// <summary>
        /// Write a line of message to console in dark yellow.
        /// </summary>
        /// <param name="content"></param>
        public static void PrintWarning(string content)
        {
            SetColourAndPrint(ConsoleColor.Yellow, content);
        }
            
        public static void PromptWarning(string content)
        {
            SetColourAndPrompt(ConsoleColor.Yellow, content);
        }

        /// <summary>
        /// Write a line of message to console in bright blue.
        /// </summary>
        /// <param name="content"></param>
        public static void PrintBright(string content)
        {
            SetColourAndPrint(ConsoleColor.Cyan, content);
        }

        public static void PromptBright(string content)
        {
            SetColourAndPrompt(ConsoleColor.Cyan, content);
        }

        public static string FillCharToRight(string s, int minLength, char filler = ' ')
        {
            string ns = s;
            int spare = minLength - s.Length;
            if (spare > 0)
                ns += string.Concat(Enumerable.Repeat(filler, 5));
            return ns;
        }
    }
}
