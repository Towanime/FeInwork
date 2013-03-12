//
// Xna Console
// www.codeplex.com/XnaConsole
// Copyright (c) 2008 Samuel Christie
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using IronPython.Modules;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace XnaConsole
{
    /// <remarks>
    /// This class implements an interpreter using IronPython
    /// </remarks>
    public class PythonInterpreter : DrawableGameComponent
    {
        const string Prompt = ">>> ";
        const string PromptCont = "... ";
        string multi;
        public XnaConsoleComponent Console;

        #region Python execution stuff

        PythonEngine PythonEngine;
        MemoryStream PythonOutput;
        ASCIIEncoding ASCIIEncoder;

        #endregion

        /// <summary>
        /// Creates a new PythonInterpreter
        /// </summary>
        public PythonInterpreter(Game1 game, SpriteFont font) : base((Game)game)
        {
            this.PythonEngine = new PythonEngine();
            this.PythonOutput = new MemoryStream();
            this.PythonEngine.SetStandardOutput(PythonOutput);
            this.ASCIIEncoder = new ASCIIEncoding();

            ClrModule clr = this.PythonEngine.Import("clr") as ClrModule;
            clr.AddReference("Microsoft.Xna.Framework");
            clr.AddReference("Microsoft.Xna.Framework.Game");

            this.PythonEngine.Execute("from Microsoft.Xna.Framework import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Graphics import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Content import *");
            multi = "";

            Console = new XnaConsoleComponent(game, font);
            game.Components.Add(Console);
            Console.Prompt(Prompt, Execute);
            AddGlobal("Console", Console);
        }

        /// <summary>
        /// Get string output from IronPythons MemoryStream standard out
        /// </summary>
        /// <returns></returns>
        private string getOutput()
        {
            byte[] statementOutput = new byte[PythonOutput.Length];
            PythonOutput.Position = 0;
            PythonOutput.Read(statementOutput, 0, (int)PythonOutput.Length);
            PythonOutput.Position = 0;
            PythonOutput.SetLength(0);
            
            return ASCIIEncoder.GetString(statementOutput);
        }

        /// <summary>
        /// Executes python commands from the console.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns the execution results or error messages.</returns>
        public void Execute(string input)
        {
            try
            {
                if ((input != "") && ((input[input.Length - 1].ToString() == ":") || (multi != ""))) //multiline block incomplete, ask for more
                {
                    multi += input + "\n";
                    Console.Prompt(PromptCont, Execute);
                }
                else if (multi != "" && input == "") //execute the multiline code after block is finished
                {
                    string temp = multi; // make sure that multi is cleared, even if it returns an error
                    multi = "";
                    PythonEngine.Execute(temp);
                    Console.WriteLine(getOutput());
                    Console.Prompt(Prompt, Execute);
                }
                else // if (multi == "" && input != "") execute single line expressions or statements
                {
                    PythonEngine.Execute(input);
                    Console.WriteLine(Console.Chomp(getOutput()));
                    Console.Prompt(Prompt, Execute);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.Prompt(Prompt, Execute);
            }

        }

        /// <summary>
        /// Adds a global variable to the environment of the interpreter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddGlobal(string name, object value)
        {
            PythonEngine.Globals.Add(name, value);
        }
    }
}
