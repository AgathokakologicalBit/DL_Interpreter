using DL_Interpreter;
using System;
using System.Collections.Generic;
using System.IO;

namespace DL_console_interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string file;
            if (args.Length != 0) file = args[0];
            else file = Console.ReadLine();

            if (File.Exists(file))
            {
                var code = File.ReadAllText(file);

                Interpreter.Reset();

                Interpreter.ShowError = ShowError;
                Interpreter.Write = Console.Write;

                Interpreter.AddNativeFunction("input", new string[0], Input);

                Interpreter.AddNativeFunction("print", new[] { "object" }, Print);
                Interpreter.AddNativeFunction("println", new[] { "object" }, Println);
                
                Interpreter.Execute(code);
            }
            else
                Console.WriteLine("File {0} doesn't exists", args[0]);

            if(File.Exists("_.temp.dl")) File.Delete("_.temp.dl");

            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadKey();
        }

        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static DL_Interpreter.Parser.Variable Input(List<DL_Interpreter.Parser.Variable> args)
        {
            return new DL_Interpreter.Parser.Variable(Console.ReadLine(), "string");
        }

        public static DL_Interpreter.Parser.Variable Write(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.WriteLine(args[0].value);
            return new DL_Interpreter.Parser.Variable();
        }

        public static DL_Interpreter.Parser.Variable Print(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.Write(args[0].value);
            return new DL_Interpreter.Parser.Variable();
        }

        public static DL_Interpreter.Parser.Variable Println(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.WriteLine(args[0].value);
            return new DL_Interpreter.Parser.Variable();
        }
    }
}