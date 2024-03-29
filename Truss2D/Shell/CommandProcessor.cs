﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;
using Truss2D.Optimization;
using static Truss2D.Shell.ConsoleFormat;
namespace Truss2D.Shell
{
    /// <summary>
    /// Future features include:
    /// Add pin support - "pin"
    /// Add roller support - "rol"
    /// Save - save the file as json
    /// Display Edge - display the internal force of edge
    /// Draw - draw the shape using simple graphics
    /// </summary>
    public class CommandProcessor
    {
        #region Keywords
        public const string Quit = "quit";
        public const string Help = "help";

        public const string Restart = "reboot";
        
        /// <summary>
        /// Starts a sequence of adding (A-Z), up to 26 joints max.
        /// </summary>
        public const string AddJoints = "add";
        public const string ResetJointCoord = "reset";
        public const string LinkJoints = "link";
        public const string PrintMembers = "members";

        public const string PrintJoints = "printj";
        public const string PrintForces = "printf";
        public const string PrintSupportReactions = "printr";

        /// <summary>
        /// Syntax: 'exert a 5 3'
        /// </summary>
        public const string AddForce = "exert";
        public const string ClearForce = "clearf";
        public const string Render = "render";

        /// <summary>
        /// Syntax: 'lookup ab'
        /// </summary>
        public const string Lookup = "see";
        public const string LookupAll = "seeall";
        public const string MinInternalForce = "minf";
        public const string MaxInternalForce = "maxf";

        public const string PinSupport = "pin";
        public const string RollerSupport = "rol";
        public const string RePin = "repin";
        public const string ReRol = "rerol";
        public const string GetCost = "cost";

        public const string Optimization = "beast-mode";

        #endregion

        #region Private Field

        private TrussBuilder builder;
        private List<string> enteredEdges;

        #endregion

        public void ProcessCommand(string[] args)
        {
            switch (args[0])
            {
                case Help:
                    PrintHelpMenu();
                    break;

                case Restart:
                    builder.Restart();
                    enteredEdges.Clear();
                    Print(Messages.RestartMessage);
                    break;

                case AddJoints:
                    while (AddJointToBuilder()) { }
                    ClearLine();
                    break;

                case LinkJoints:
                    if (!(args.Length>1))
                        throw new ArgException($"Bad argument ...");
                    for (int i=1; i<args.Length; ++i)
                        if (args[i].Length != 2)
                            throw new ArgException($"Bad segment length ...");

                    for (int i=1; i<args.Length; ++i)
                    {
                        string arg = args[i];
                        builder.LinkJoints(args[i][0], args[i][1]);
                        enteredEdges.Add(args[i]);
                    }
                    
                    Print($"Linking successful ...");
                    break;

                case ResetJointCoord:
                    if (!(args.Length == 4 && args[1].Length==1))
                        throw new Exception($"Bad arguments ...");
                    decimal newx = decimal.Parse(args[2]);
                    decimal newy = decimal.Parse(args[3]);
                    char jointc = args[1][0];
                    builder.Model.ResetVertexCoord(builder.GetJoint(jointc), newx, newy);
                    builder.ResetVertex(jointc, newx, newy);
                    break;

                case AddForce:
                    if (!(args.Length==4 || args[1].Length==1))
                        throw new ArgException($"Bad argument ...");
                    builder.AddForce(builder.GetJoint(args[1][0]), 
                        new Vector(decimal.Parse(args[2]), decimal.Parse(args[3])));
                    Print($"Force successfully added ...");
                    break;

                case ClearForce:
                    if (args.Length < 2)
                        throw new Exception("No joint indicated ...");
                    for (int i = 1; i < args.Length; ++i)
                        if (args[i].Length != 1)
                            throw new Exception($"Wtf is '{args[i]}' ...");
                    for (int i = 1; i < args.Length; ++i)
                        builder.Model.ClearJointForce(builder.GetJoint(args[i][0]));
                    Print($"All joints cleared");
                    break;

                case PrintForces:
                    builder.PrintAllReactions();
                    break;

                case PrintJoints:
                    builder.PrintAllJoints();
                    break;

                case PrintSupportReactions:
                    Prompt("Pin: ");
                    builder.PrintPinForces();
                    Prompt("Roller: ");
                    builder.PrintRollerForces();
                    break;

                case Render:
                    bool status = builder.Render();
                    Print(status ? "Render success ..." : "Render incomplete");
                    break;

                case Lookup:
                    if (args.Length < 2)
                        throw new Exception("Wtf are you looking for ...");
                    for (int i = 1; i < args.Length; ++i)
                        if (args[i].Length != 2)
                            throw new Exception($"Wtf is {args[i]} ...");
                    for (int i=1; i<args.Length; ++i)
                    {
                        decimal? internalForce = builder.Model.GetInternalForce(
                            new Edge(builder.GetJoint(args[i][0]), builder.GetJoint(args[i][1])));
                        Prompt($"Member {args[i].ToUpper()}: ");
                        PrintInternalForce(internalForce);
                    }
                    break;

                case LookupAll:
                    foreach(string arg in enteredEdges)
                    {
                        decimal? internalForce = builder.Model.GetInternalForce(
                            new Edge(builder.GetJoint(arg[0]), builder.GetJoint(arg[1])));
                        Prompt($"Member {arg.ToUpper()}: ");
                        PrintInternalForce(internalForce);
                    }
                    break;

                case MinInternalForce:
                    decimal? min = builder.Model.MinInternalForce;
                    PrintInternalForce(min);
                    break;

                case MaxInternalForce:
                    decimal? max = builder.Model.MaxInternalForce;
                    PrintInternalForce(max);
                    break;

                case PinSupport:
                    if (args.Length != 2 || args[1].Length != 1)
                        throw new Exception("Bad arguments ..."); 
                    builder.AddPin(args[1][0]);
                    break;

                case RePin:
                    if (args.Length != 2 || args[1].Length != 1)
                        throw new Exception("Bad arguments ...");
                    builder.ChangePin(args[1][0]);
                    break;

                case RollerSupport:
                    if (args.Length != 2 || args[1].Length != 1)
                        throw new Exception("Bad arguments ...");
                    builder.AddRoller(args[1][0]);
                    break;

                case ReRol:
                    if (args.Length != 2 || args[1].Length != 1)
                        throw new Exception("Bad arguments ...");
                    builder.ChangeRoller(args[1][0]);
                    break;

                case GetCost:
                    Print(builder.Model.GetPrice().ToString("0.00000"));
                    break;

                case Optimization:
                    new OCommandProcessor().Start();
                    break;

                default:
                    throw new ArgException($"Command '{args[0]}' not recognized ...");
            }
        }

        /// <summary>
        /// Returns true if not continue, false otherwise
        /// </summary>
        /// <returns></returns>
        private bool AddJointToBuilder()
        {
            Prompt("Joint " + (char)('A' + builder.CurrentAlphaPos) + ": ");
            string[] args = Console.ReadLine().Trim().ToLower().Split(null);
            if (args[0] == "")
                return false;
            else if (args.Length != 2)
                throw new ArgException("Bad arg length ...");
            Vertex newvertex = new Vertex(decimal.Parse(args[0]), decimal.Parse(args[1]));
            builder.AddJoint(newvertex);

            ClearLine();
            Print("Joint " + (char)('A' + builder.CurrentAlphaPos - 1) + $" ({ newvertex.X.ToString("0.##")}, { newvertex.Y.ToString("0.##")})");
            return true;
        }

        public void PrintHelpMenu()
        {
            Println(Help, Messages.Help);
            Println(Quit, Messages.Quit);
            Println(Restart, Messages.Restart);
            Println(AddJoints, Messages.AddJoints);
            Println(ResetJointCoord, Messages.ResetJointCoord);
            Println(LinkJoints, Messages.LinkJoints);
            Println(PrintJoints, Messages.PrintJoints);
            Println(AddForce, Messages.AddForce);
            Println(PrintForces, Messages.PrintForces);
            Println(Render, Messages.Render);
            Println(PrintMembers, Messages.PrintMembers);
            Println(Lookup, Messages.Lookup);
            Println(MinInternalForce, Messages.MinInternalForce);
            Println(MaxInternalForce, Messages.MaxInternalForce);
            Println(LookupAll, Messages.LookupAll);
            Println(PinSupport, Messages.Pin);
            Println(RollerSupport, Messages.Roller);
            Println(RePin, Messages.RePin);
            Println(ReRol, Messages.ReRol);
            Println(PrintSupportReactions, "Prints all reaction forces");
            Println(GetCost, "Gets the cost of the truss");
            Println(Optimization, "Enter beast mode");
        }

        public void Println(string key, string value) {
            PromptWarning($"'{key}': ");
            Print(value);
        }
        
        public void PrintInternalForce(decimal? internalForce)
        {
            Print(internalForce == null ? "Unknown ..." :
            internalForce.Value.ToString("0.00"));

        }

        #region Life Cycle

        public CommandProcessor()
        {
            builder = new TrussBuilder();
            enteredEdges = new List<string>();
        }

        #endregion

        public class ArgException : Exception
        {
            public ArgException()
            { }

            public ArgException(string message) : base(message)
            { }

            public ArgException(string message, Exception innerException) : base(message, innerException)
            { }

            protected ArgException(SerializationInfo info, StreamingContext context) : base(info, context)
            { }
        }

    }
}
