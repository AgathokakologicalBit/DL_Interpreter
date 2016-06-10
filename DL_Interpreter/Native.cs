using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DL_Interpreter
{
    class Native
    {
        public static Stopwatch watch = new Stopwatch();

        public static Parser.Variable TypeOf(List<Parser.Variable> args) => new Parser.Variable(args[0].type, "string");
        
        public static Parser.Variable Sin(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if (!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(Math.Sin(Parse(args[0].ConvertTo("number").value)));
        }

        public static Parser.Variable Cos(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(Math.Cos(Parse(args[0].ConvertTo("number").value)));
        }

        public static Parser.Variable Sqrt(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            double val = Parse(args[0].ConvertTo("number").value);
            if (val < 0) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(Math.Sqrt(val));
        }

        public static Parser.Variable Pow(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if (args[1].IsNaN()) return new Parser.Variable("NaN", "number");

            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable(0);
            if(!args[1].CanBeConvertedTo("number")) return new Parser.Variable(0);

            double x = Parse(args[0].ConvertTo("number").value);
            double y = Parse(args[1].ConvertTo("number").value);

            return new Parser.Variable(Math.Pow(x, y));
        }

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
