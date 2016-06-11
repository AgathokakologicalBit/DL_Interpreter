using DL_Interpreter.Parser;
using DL_Interpreter.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DL_Interpreter
{
    public class Interpreter
    {
        public readonly static Operator[] operators = new Operator[] {
            new Operator(".", 20),
            new Operator("=", 10, true),
            new Operator("+=", 10, true),
            new Operator("-=", 10, true),
            new Operator("*=", 10, true),
            new Operator("/=", 10, true),

            new Operator("*", 4),
            new Operator("/", 4),
            new Operator("%", 4),

            new Operator("+", 3),
            new Operator("-", 3),

            new Operator(">",  2),
            new Operator(">=", 2),
            new Operator("<",  2),
            new Operator("<=", 2),

            new Operator("===", 1),
            new Operator("!==", 1),
            new Operator("==", 1),
            new Operator("!=", 1),
        };
        public static List<Variable> variables = new List<Variable>(100);

        public static Action<string> Write;
        public static Action<string> ShowError;

        public static int depth = 0;
        private static bool isReturn;
        private static bool isBreak;

        static Interpreter()
        {
            Reset();
        }

        // Delete all created variables and functions
        public static void Reset()
        {
            variables = new List<Variable>(100);

            AddNativeFunction("typeof", new[] { "object" }, Native.TypeOf);

            AddNativeFunction("float", new[] { "object" }, Native.convert_float);
            AddNativeFunction("int", new[] { "object" }, Native.convert_int);
            AddNativeFunction("bool", new[] { "object" }, Native.convert_bool);
            AddNativeFunction("string", new[] { "object" }, Native.convert_string);
            
            AddNativeFunction("executionTime", new string[0], Native.ExecutionTime);

            AddNativeFunction("sin", new[] { "num" }, Native.Sin);
            AddNativeFunction("cos", new[] { "num" }, Native.Cos);

            AddNativeFunction("pow", new[] { "x", "y" }, Native.Pow);
            AddNativeFunction("sqrt", new[] { "num" }, Native.Sqrt);
        }

        public static string Execute(string code)
        {
            if(variables == null || variables.Count == 0) Reset();

            // Remove 1-line comments starting with '//'
            code = Regex.Replace(code, "//.*", string.Empty);
            // Remove multiline comments
            code = Regex.Replace(code, @"/\*.*?(\*/|$)", string.Empty, RegexOptions.Singleline);

            // Generate tokens from code
            List<Token> tokens = Tokenizer.Tokenizer.Tokenize(code);

            // Create execution tree
            try
            {
                Block ast = Parser.Parser.CreateSyntaxTree(tokens);
                
                string result = "";

                Native.watch.Reset();
                Native.watch.Start();

                foreach(var command in ast.code)
                    result = CalculateTree(command, variables).value;

                Native.watch.Stop();

                return result;
            }
            catch (ParsingError err)
            {
                Native.watch.Stop();
                ShowError?.Invoke(err.Message);
            }

            return "";
        }

        internal static string GetDefaultFor(string type)
        {
            switch(type)
            {
                case "string": return "";
                case "boolean": return "false";
                case "number": return "0";
                case "null": return "null";
                default: return "undefined";
            }
        }

        public static void AddNativeFunction(string name, string[] parameters, FunctionNode.ExecuteFunction func)
        {
            variables.Add(new Variable(name, "function",
                new FunctionNode(name, new List<string>(parameters), func)));
        }

        public static Parser.Variable CalculateTree(Node tree, List<Variable> context)
        {
            var varnode = tree as Parser.Variable;
            if (varnode != null)
            {
                Variable var = null;

                if (varnode.type != "variable")
                {
                    if (varnode.type == "functionCall")
                    {
                        Parser.Variable left = CalculateTree(varnode.expression, context);

                        Parser.Variable caller;
                        var objectOp = varnode.expression as Operation;

                        if (objectOp == null || objectOp.op.symbol != ".") caller = new Parser.Variable();
                        else caller = GetVariableByName(context, objectOp.left);

                        if (left.type == "function")
                            var = new Variable("anonymous", "function", left);
                        else
                        {
                            ShowError(left.type + " is not a function");
                            return new Parser.Variable();
                        }

                        if (varnode.args != null)
                        {
                            var func = var.value as FunctionNode;

                            var args = new List<Parser.Variable>(Math.Max(varnode.args.Count, func.parameters.Count));

                            for (int now = 0; now < args.Capacity; ++now)
                            {
                                if (now < varnode.args.Count)
                                    args.Add(CalculateTree(varnode.args[now], context));
                                else
                                    args.Add(new Parser.Variable());
                            }
                            
                            if (func.native) return func.function(args);
                            else return ExecuteFunction(func, args, caller);
                        }

                        return new Parser.Variable();
                    }
                    return varnode;
                }
                
                var = GetVariableByName(context, varnode.value);

                if (var != null) return var.value;
                else return new Parser.Variable();
            }

            var opnode = tree as Operation;
            if(opnode != null)
            {
                Parser.Variable left = null, right = null;
                if (opnode.op.symbol != ".")
                {
                    var lon = opnode.left as Operation;
                    if (lon?.op.symbol != ".")
                        left = CalculateTree(opnode.left, context);

                    right = CalculateTree(opnode.right, context);
                }

                switch(opnode.op.symbol)
                {
                    case "+": return VariableOperations.sum(left, right);
                    case "-": return VariableOperations.sub(left, right);
                    case "*": return VariableOperations.mul(left, right);
                    case "/": return VariableOperations.div(left, right);
                    case "%": return VariableOperations.res(left, right);

                    case ".": return GetVariableByName(context, opnode);

                    case "=":
                        if (right.fields.Count != 0)
                        {
                            var fields = new Dictionary<Parser.Variable, Parser.Node>(right.fields.Count);
                            foreach (var key in right.fields)
                                fields.Add(key.Key, CalculateTree(key.Value, context));
                            right.fields = fields;
                        }
                        
                        SetVariable(context, opnode.left, right.type, right);
                        
                        return right;
                    case "+=":
                        left = GetVariableByName(context, opnode.left);
                        var sum = VariableOperations.sum(left, right);

                        SetVariable(context, opnode.left, sum.type, sum);

                        return sum;
                    case "-=":
                        left = GetVariableByName(context, opnode.left);
                        var sub = VariableOperations.sub(left, right);

                        SetVariable(context, opnode.left, sub.type, sub);

                        return sub;
                    case "*=":
                        left = GetVariableByName(context, opnode.left);
                        var mult = VariableOperations.mul(left, right);

                        SetVariable(context, opnode.left, mult.type, mult);
                        
                        return mult;
                    case "/=":
                        left = GetVariableByName(context, opnode.left);
                        var div = VariableOperations.div(left, right);

                        SetVariable(context, opnode.left, div.type, div);

                        return div;

                    case "==": return new Parser.Variable(left.IsEqualTo(right) ? "true" : "false" , "boolean");
                    case "!=": return new Parser.Variable(left.IsEqualTo(right) ? "false" : "true" , "boolean");
                    case "===": return new Parser.Variable(left.IsDeepEqualTo(right) ? "true" : "false" , "boolean");
                    case "!==": return new Parser.Variable(left.IsDeepEqualTo(right) ? "false" : "true" , "boolean");

                    case ">": return new Parser.Variable(left.IsGreaterThan(right) ? "true" : "false" , "boolean");
                    case ">=": return new Parser.Variable(left.IsGreaterOrEqualTo(right) ? "true" : "false" , "boolean");
                    case "<": return new Parser.Variable(right.IsGreaterThan(left) ? "true" : "false" , "boolean");
                    case "<=": return new Parser.Variable(right.IsGreaterOrEqualTo(left) ? "true" : "false" , "boolean");
                }
            }

            var conditionalNode = tree as ConditionalBlock;
            if (conditionalNode != null)
            {
                if (conditionalNode.type == "if")
                {
                    if (CalculateTree(conditionalNode.condition, context).IsEqualTo(new Parser.Variable(true)))
                        return CalculateTree(conditionalNode.code, context);
                    else if ( conditionalNode.oppositeCode != null )
                        return CalculateTree(conditionalNode.oppositeCode, context);
                }

                if (conditionalNode.type == "for")
                {
                    CalculateTree(conditionalNode.init, context);

                    while (CalculateTree(conditionalNode.condition, context).IsEqualTo(new Parser.Variable(true)))
                    {
                        varnode = CalculateTree(conditionalNode.code, context);
                        if (isReturn) return varnode;
                        if (isBreak)
                        {
                            isBreak = false;
                            break;
                        }

                        CalculateTree(conditionalNode.post, context);
                    }
                }

                if (conditionalNode.type == "while")
                {
                    while (CalculateTree(conditionalNode.condition, context).IsEqualTo(new Parser.Variable(true)))
                    {
                        varnode = CalculateTree(conditionalNode.code, context);
                        if (isReturn) return varnode;
                        if (isBreak)
                        {
                            isBreak = false;
                            break;
                        }
                    }
                }

                return new Parser.Variable();
            }

            var blocknode = tree as Block;
            if (blocknode != null)
            {
                foreach (var exp in blocknode.code)
                {
                    varnode = CalculateTree(exp, context);
                    if (isReturn) return varnode;
                    if (isBreak) break;
                }
            }
            
            if (tree.value == "return")
            {
                isReturn = true;
                return CalculateTree((tree as ReturnNode).expression, context);
            }
            if (tree.value == "break") isBreak = true;

            return new Parser.Variable();
        }

        private static Parser.Variable ExecuteFunction(FunctionNode func, List<Parser.Variable> args, Parser.Variable caller)
        {
            ++depth;

            Parser.Variable var;

            var context = new List<Variable>(10);
            for (int now = 0; now < func.parameters.Count; ++now)
                if (args.Count >= now)
                    context.Add(new Variable(func.parameters[now], args[now].type, args[now]));
            context.Add(new Variable("this", caller.type, caller));

            foreach (Node line in func.code.code)
            {
                var = CalculateTree(line, context);
                if (isReturn)
                {
                    isReturn = false;
                    --depth;
                    return var;
                }
                if (isBreak)
                {
                    ShowError("break operator need to be placed in cycle");
                    isBreak = false;
                }
            }

            --depth;
            return new Parser.Variable();
        }

        internal static bool IsPredefined(string type)
        {
            if (type == "false") return true;
            if (type == "true") return true;
            
            if (type == "undefined") return true;
            if (type == "null") return true;

            return false;
        }

        internal static string GetTypeOfPredefined(string type)
        {
            if (type == "false") return "boolean";
            if (type == "true") return "boolean";
            
            if (type == "undefined") return "undefined";
            if (type == "null") return "object";

            return "undefined";
        }

        public static void SetVariable(List<Variable> vars, string name, string type, Parser.Variable value)
        {
            for (var now = vars.Count - 1; now != -1; --now)
            {
                if (vars[now].name == name)
                {
                    vars[now].value = value.Clone();
                    vars[now].value.type = type;
                    return;
                }
            }
            
            vars.Add(new Variable(name, type, value));
        }

        public static void SetVariable(List<Variable> vars, Node variable, string type, Parser.Variable value)
        {
            var normal = variable as Parser.Variable;

            if (normal != null) SetVariable(vars, variable.value, type, value.Clone());
            
            var varop = variable as Operation;
            if (varop != null)
            {
                var left = GetVariableByName(vars, varop.left);

                if (left.type == "undefined")
                {
                    ShowError("Cannod read property of undefined");
                }
                else
                {
                    var right = CalculateTree(varop.right, vars);
                    bool found = false;
                    
                    foreach (var key in left.fields)
                        if (key.Key.IsDeepEqualTo(right))
                        {
                            (key.Value as Parser.Variable)?.Set(value.Clone());
                            found = true;
                        }

                    if (!found) left.fields.Add(right.Clone(), value.Clone());
                }
            }
        }

        public static Parser.Variable GetVariableByName(List<Variable> vars, Node variable, bool global = false)
        {
            var normal = variable as Parser.Variable;
            
            if (normal != null) return GetVariableByName(vars, normal.value).value;
            
            var varop = variable as Operation;
            if (varop != null)
            {
                var left = GetVariableByName(vars, varop.left);
                if (left.type == "undefined") return left;

                var right = CalculateTree(varop.right, vars);

                foreach (var key in left.fields)
                    if (key.Key.IsDeepEqualTo(right))
                        return key.Value as Parser.Variable;
            }

            return new Parser.Variable();
        }

        public static Variable GetVariableByName(List<Variable> vars, string name, bool global = false)
        {
            for(var now = vars.Count - 1; now != -1; --now)
                if(vars[now].name == name)
                    return vars[now];
            
            if ( depth != 0 && !global ) return GetVariableByName(variables, name, true);

            return new Variable("", "undefined", new Parser.Variable());
        }
    }
}
