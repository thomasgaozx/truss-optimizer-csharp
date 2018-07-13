using System;
using System.IO;
using static Truss2D.Shell.ConsoleFormat;

namespace Truss2D
{
    public class Program
    {
        const string QuitKeyword = "quit";

        public static void Main(string[] args)
        {
            Welcome();

            while (HandleCommand())
            { }

            PrintWarning("Program exits ...");
            System.Threading.Thread.Sleep(300);
        }

        private static void Welcome()
        {
            try
            {
                string[] welcome = File.ReadAllLines("Welcome.txt");
                foreach (string line in welcome)
                {
                    Print(line);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Returns false if exits
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool HandleCommand()
        {
            Prompt("> ");
            try
            {
                string raw = Console.ReadLine().Trim().ToLower();
                string[] command = raw.Split(null);
                if (command[0].Equals(QuitKeyword))
                    return false;
                // Use the processor to process the command otherwise
            }
            catch (Exception e)
            {
                PrintWarning("An error occurred");
                Print($"{e.Message}\n\n{e.Source}\n\n{e.StackTrace}");
            }

            return true;
        }
    }
}
