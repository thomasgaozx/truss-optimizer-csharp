using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Truss2D.Shell.ConsoleFormat;
using static Truss2D.Shell.CommandProcessor;
using System.IO;

namespace Truss2D.Optimization
{
    public class OCommandProcessor
    {
        private static OTruss truss;

        public const string OptimizeCommand = "op";
        public const string GridOptimizationCommand = "gop";
        public const string SeeAllCoordinateCommand = "newcoord";
        public const string Works = "works";

        public const string Triangle = "tri";
        public const string Cross = "tetra";
        public const string Hexagon = "hex";
        public const string Octagon = "oct";
        public const string Grid = "grid";

        public OCommandProcessor()
        {
            truss = new OTruss();
        }

        public void Start()
        {
            Welcome();

            while (HandleCommand()) {  }
        }

        private static void Welcome()
        {
            try
            {
                string[] welcome = File.ReadAllLines("optimization_help.txt");
                foreach (string line in welcome)
                {
                    PrintWarning(line);
                }
                PrintWarning(LineBreak);
            }
            catch (Exception) { }
        }

        private static bool HandleCommand()
        {
            PromptWarning(">>> ");
            try
            {
                string raw = Console.ReadLine().Trim().ToLower();
                if (!String.IsNullOrWhiteSpace(raw))
                {
                    string[] command = raw.Split(null);
                    if (command[0].Equals(Program.QuitKeyword))
                        return false;
                    // Use the processor to process the command otherwise
                    ProcessCommand(command);
                }
            }
            catch (Exception e)
            {
                PrintWarning("An error occurred ...");
                PrintDanger(e.Message);
            }

            return true;
        }

        static void ProcessCommand(string[] args)
        {
            switch(args[0])
            {
                case AddJoints:
                    while (AddJointToTruss()) { }
                    ClearLine();
                    break;
                case LinkJoints:
                    if (!(args.Length > 1))
                        throw new ArgException($"Bad argument ...");
                    for (int i = 1; i < args.Length; ++i)
                        if (args[i].Length != 2)
                            throw new ArgException($"Bad segment length ...");

                    for (int i = 1; i < args.Length; ++i)
                    {
                        string arg = args[i];
                        truss.AddEdge(args[i][0], args[i][1]);
                    }

                    PrintWarning($"Linking successful ...");
                    break;
                case AddForce:
                    if (!(args.Length == 4 || args[1].Length == 1))
                        throw new ArgException($"Bad argument ...");
                    truss.AddForce(args[1][0],decimal.Parse(args[2]), decimal.Parse(args[3]));
                    PrintWarning($"Force successfully added ...");
                    break;

                    // optimize hex 2
                case OptimizeCommand:
                    truss.Optimize(int.Parse(args[1]), decimal.Parse(args[2]));
                    break;

                case GridOptimizationCommand:
                    truss.GridOptimize(decimal.Parse(args[1]));
                    break;

                case SeeAllCoordinateCommand:
                    truss.PrintJoints();
                    break;

                case GetCost:
                    PrintWarning(truss.GetCost().ToString("0.###"));
                    break;

                case Works:
                    PrintWarning((truss.Pass() && truss.MemberWorks()?"true":"false"));
                    break;

                default:
                    throw new ArgException($"Command '{args[0]}' not recognized ...");
            }

        }

        static bool AddJointToTruss()
        {

            PromptWarning("Joint " + (char)('A' + truss.NumOfJoints) + ": ");
            string[] args = Console.ReadLine().Trim().ToLower().Split(null);
            if (args[0] == "")
                return false;
            else if (!(args.Length == 2||args.Length==3))
                throw new ArgException("Bad arg length ...");

            Joint newJoint = new Joint(decimal.Parse(args[0]), decimal.Parse(args[1]));
            bool isFree = args.Length == 3 && args[2] == "-f";
            truss.AddJoint(newJoint, isFree);

            ClearLine();
            //PrintWarning("Joint " + (char)('A' + truss.NumOfJoints - 1) + $" ({ newJoint.X.ToString("0.##")}, { newJoint.Y.ToString("0.##")}){(isFree?" [FREE]":"")}");
            return true;

        }

    }
}
