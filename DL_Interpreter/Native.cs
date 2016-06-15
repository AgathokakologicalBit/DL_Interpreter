using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DL_Interpreter
{
    class Native
    {
        public static Stopwatch watch = new Stopwatch();

        public static Parser.Variable TypeOf(List<Parser.Variable> args) => new Parser.Variable(args[0].type, "string");
        
        public static Parser.Variable convert_bool(List<Parser.Variable> args) => args[0].ConvertTo("boolean");
        public static Parser.Variable convert_string(List<Parser.Variable> args) => args[0].ConvertTo("string");
        public static Parser.Variable convert_float(List<Parser.Variable> args) => args[0].ConvertTo("number");
        public static Parser.Variable convert_int(List<Parser.Variable> args) => args[0].ConvertTo("number").ToInt();

        public static Parser.Variable ExecutionTime(List<Parser.Variable> args) =>
            new Parser.Variable(watch.Elapsed.TotalMilliseconds);

        public static double Parse(string num)
        {
            var val = 0.0;
            double.TryParse(num.Replace('.', ','), out val);
            return val;
        }
    }
}
