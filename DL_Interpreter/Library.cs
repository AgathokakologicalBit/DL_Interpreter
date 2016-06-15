using System.Collections.Generic;

namespace DL_Interpreter
{
    public abstract class Library
    {
        public abstract void Init();

        public static Parser.Variable RegisterObject(string name)
        {
            var value = new Parser.Variable("", "object");
            Interpreter.SetVariable(Interpreter.variables, new Parser.Variable(name, "variable"), "object", value);
            value.constant = true;
            return value;
        }

        public static void RegisterFunction(string name, Parser.FunctionNode.ExecuteFunction function, string[] parameters)
        {
            var func = new Parser.FunctionNode(name, new List<string>(parameters), function);
            Interpreter.SetVariable(Interpreter.variables, new Parser.Variable(name, "variable"), "function", func);
            func.constant = true;
        }
    }
}
