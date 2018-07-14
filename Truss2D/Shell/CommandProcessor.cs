using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Truss2D.Math;
using static Truss2D.Shell.ConsoleFormat;
namespace Truss2D.Shell
{
    /// <summary>
    /// Future features include:
    /// Add pin support - "pin"
    /// Add roller support - "roll"
    /// Save - save the file as json
    /// Display Edge - display the internal force of edge
    /// </summary>
    public class CommandProcessor
    {
        #region Keywords
        public const string Quit = "quit";
        public const string Help = "help";

        public const string Restart = "restart";
        
        /// <summary>
        /// Starts a sequence of adding (A-Z), up to 26 joints max.
        /// </summary>
        public const string AddJoints = "add";
        public const string ResetJointCoord = "reset";
        public const string PrintJoints = "joints";

        public const string LinkJoints = "link";
        public const string PrintMembers = "members";

        /// <summary>
        /// Syntax: 'exert a 5 3'
        /// </summary>
        public const string AddForce = "exert";
        public const string PrintForce = "force";

        public const string Render = "render";

        /// <summary>
        /// Syntax: 'lookup ab'
        /// </summary>
        public const string Lookup = "lookup";
        public const string MinInternalForce = "minf";
        public const string MaxInternalForce = "maxf";
        
        #endregion

        #region Private Field

        private TrussBuilder builder;

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
                    {
                        string arg = args[i];
                        if (arg.Length!=2)
                            throw new ArgException($"Bad segment length ...");
                        builder.LinkJoints(args[i][0], args[i][1]);
                    }
                    
                    Print($"Linking successful ...");
                    break;
                case AddForce:
                    if (!(args.Length==4 || args[1].Length==1))
                        throw new ArgException($"Bad argument ...");
                    builder.AddForce(builder.GetJoint(args[1][0]), 
                        new Vector(decimal.Parse(args[2]), decimal.Parse(args[3])));
                    Print($"Force successfully added ...");
                    break;
                case Render:
                    builder.Render();
                    Print($"Render success ...");
                    break;
                case Lookup:
                    decimal? internalForce = builder.Model.GetInternalForce(
                        new Edge(builder.GetJoint(args[1][0]),
                        builder.GetJoint(args[1][1])));
                    Print(internalForce == null ? "Unknown ..." :
                        internalForce.Value.ToString("#.##"));
                    break;
                case MinInternalForce:
                    decimal? min = builder.Model.MinInternalForce;
                    Print(min == null ? "Unknown ..." : min.Value.ToString("#.##"));
                    break;
                case MaxInternalForce:
                    decimal? max = builder.Model.MaxInternalForce;
                    Print(max == null ? "Unknown ..." : max.Value.ToString("#.##"));
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
            Vertice newVertice = new Vertice(decimal.Parse(args[0]), decimal.Parse(args[1]));
            builder.AddJoint(newVertice);

            ClearLine();
            Print("Joint " + (char)('A' + builder.CurrentAlphaPos - 1) + $" ({ newVertice.X.ToString("#.##")}, { newVertice.Y.ToString("#.#")})");
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
            Println(PrintForce, Messages.PrintForce);
            Println(Render, Messages.Render);
            Println(PrintMembers, Messages.PrintMembers);
            Println(Lookup, Messages.Lookup);
            Println(MinInternalForce, Messages.MinInternalForce);
            Println(MaxInternalForce, Messages.MaxInternalForce);
        }

        public void Println(string key, string value) {
            PromptWarning($"'{key}': ");
            Print(value);
        }

        #region Life Cycle

        public CommandProcessor()
        {
            builder = new TrussBuilder();
        }

        #endregion

        public class ArgException : Exception
        {
            public ArgException()
            {
            }

            public ArgException(string message) : base(message)
            {
            }

            public ArgException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ArgException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

    }
}
