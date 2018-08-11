using System;
using System.IO;
using Truss2D.Shell;
using static Truss2D.Shell.ConsoleFormat;

namespace Truss2D
{
    public class Program
    {

        static CommandProcessor processor = new CommandProcessor();
        public const string QuitKeyword = "quit";
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
                Print("\n" + Messages.Flow_Message);
                PrintWarning("\n" + Messages.Assumption_Message);
                Print("\n" + Messages.Help_Welcome);
                Print(LineBreak);
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
                if (!String.IsNullOrWhiteSpace(raw))
                {
                    string[] command = raw.Split(null);
                    if (command[0].Equals(QuitKeyword))
                        return false;
                    // Use the processor to process the command otherwise
                    processor.ProcessCommand(command);
                }
            }
            catch (CommandProcessor.ArgException a)
            {
                PrintDanger(a.Message);
            }
            catch (Exception e)
            {
                PrintWarning("An error occurred ...\n");
                PrintDanger(e.Message);
            }

            return true;
        }
    }
}
