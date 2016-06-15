using System.Collections.Generic;

namespace DL_Interpreter.Libs
{
    class Math : Library
    {
        public override void Init()
        {
            RegisterObject("Math")
                .AddVariable("PI", new Parser.Variable(3.14159265359))
                .AddFunction("sin", Sin, new string[] { "rad" })
                .AddFunction("cos", Cos, new string[] { "rad" })
                .AddFunction("sqrt", Sqrt, new string[] { "sqrt" })
                .AddFunction("pow", Pow, new string[] { "pow" });
        }

        public static Parser.Variable Sin(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if (!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(System.Math.Sin(Native.Parse(args[0].ConvertTo("number").value)));
        }

        public static Parser.Variable Cos(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(System.Math.Cos(Native.Parse(args[0].ConvertTo("number").value)));
        }

        public static Parser.Variable Sqrt(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable("NaN", "number");

            double val = Native.Parse(args[0].ConvertTo("number").value);
            if (val < 0) return new Parser.Variable("NaN", "number");

            return new Parser.Variable(System.Math.Sqrt(val));
        }

        public static Parser.Variable Pow(List<Parser.Variable> args)
        {
            if (args[0].IsNaN()) return new Parser.Variable("NaN", "number");
            if (args[1].IsNaN()) return new Parser.Variable("NaN", "number");

            if(!args[0].CanBeConvertedTo("number")) return new Parser.Variable(0);
            if(!args[1].CanBeConvertedTo("number")) return new Parser.Variable(0);

            double x = Native.Parse(args[0].ConvertTo("number").value);
            double y = Native.Parse(args[1].ConvertTo("number").value);

            return new Parser.Variable(System.Math.Pow(x, y));
        }
    }
}
