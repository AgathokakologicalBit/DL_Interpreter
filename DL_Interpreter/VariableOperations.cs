using System;
using DL_Interpreter.Parser;

namespace DL_Interpreter
{
    class VariableOperations
    {
        public static Parser.Variable sum(Parser.Variable left, Parser.Variable right)
        {
            left = GetVariableValue(left);
            right = GetVariableValue(right);

            if (left.type == "string" || right.type == "string")
                return new Parser.Variable(left.value + right.value, "string");

            if (left.IsNaN() || right.IsNaN()) return new Parser.Variable("NaN", "number");
            
            if (!left.CanBeConvertedTo("number") || !right.CanBeConvertedTo("number"))
                return new Parser.Variable("NaN", "number");

            left = left.ConvertTo("number");
            right = right.ConvertTo("number");

            return new Parser.Variable(Parse(left.value) + Parse(right.value));
        }

        public static Parser.Variable sub(Parser.Variable left, Parser.Variable right)
        {
            left = GetVariableValue(left);
            right = GetVariableValue(right);

            if (left.IsNaN() || right.IsNaN()) return new Parser.Variable("NaN", "number");
            if (!left.CanBeConvertedTo("number") || !right.CanBeConvertedTo("number"))
                return new Parser.Variable("NaN", "number");

            left = left.ConvertTo("number");
            right = right.ConvertTo("number");
            
            return new Parser.Variable(Parse(left.value) - Parse(right.value));
        }

        public static Parser.Variable mul(Parser.Variable left, Parser.Variable right)
        {
            left = GetVariableValue(left);
            right = GetVariableValue(right);

            if (left.IsNaN() || right.IsNaN()) return new Parser.Variable("NaN", "number");
            if (!left.CanBeConvertedTo("number") || !right.CanBeConvertedTo("number"))
                return new Parser.Variable("NaN", "number");

            left = left.ConvertTo("number");
            right = right.ConvertTo("number");

            return new Parser.Variable(Parse(left.value) * Parse(right.value));
        }

        public static Parser.Variable div(Parser.Variable left, Parser.Variable right)
        {
            left = GetVariableValue(left);
            right = GetVariableValue(right);

            if (left.IsNaN() || right.IsNaN()) return new Parser.Variable("NaN", "number");
            if (!left.CanBeConvertedTo("number") || !right.CanBeConvertedTo("number"))
                return new Parser.Variable("NaN", "number");

            left = left.ConvertTo("number");
            right = right.ConvertTo("number");

            if (Parse(right.value) == 0) return new Parser.Variable(0);

            return new Parser.Variable(Parse(left.value) / Parse(right.value));
        }

        internal static Parser.Variable res(Parser.Variable left, Parser.Variable right)
        {
            left = GetVariableValue(left);
            right = GetVariableValue(right);

            if (left.IsNaN() || right.IsNaN()) return new Parser.Variable("NaN", "number");
            if (!left.CanBeConvertedTo("number") || !right.CanBeConvertedTo("number"))
                return new Parser.Variable("NaN", "number");

            left = left.ConvertTo("number");
            right = right.ConvertTo("number");
            
            return new Parser.Variable(Parse(left.value) % Parse(right.value));
        }

        public static Parser.Variable GetVariableValue(Parser.Variable var)
        {
            if (var.type == "variable")
            {
                var gvar = Interpreter.GetVariableByName(Interpreter.variables, var.value);
                if (gvar == null) return new Parser.Variable("undefined", "undefined");
                return new Parser.Variable(gvar.value.value, gvar.value.type);
            }
            return var;
        }

        private static double Parse(string num)
        {
            var val = 0.0;
            double.TryParse(num.Replace('.', ','), out val);
            return val;
        }
    }
}