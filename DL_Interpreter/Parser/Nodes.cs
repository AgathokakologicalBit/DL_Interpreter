using System.Collections.Generic;

namespace DL_Interpreter.Parser
{
    public class Block : Node
    {
        public List<Node> code = new List<Node>(50);
        
        public void Append(Node expr) => code.Add(expr);
    }

    public class Variable : Node
    {
        public string type;

        public Node expression;
        public List<Node> args;

        public Dictionary<Variable, Node> fields = new Dictionary<Variable, Node>(10);
        public Variable prototype;
        public bool constant = false;

        public Variable()
        {
            this.type = "undefined";
            this.value = "undefined";
        }

        public Variable(string value, string type)
        {
            this.value = value;
            this.type = type;
        }

        public Variable(double value)
        {
            this.value = value.ToString();
            this.type = "number";
        }

        public Variable(Node expression, List<Node> args)
        {
            this.expression = expression;
            this.type = "functionCall";
            this.args = args;
        }

        public Variable(bool value)
        {
            this.value = value ? "true" : "false";
            this.type = "boolean";
        }

        internal bool IsNaN() => type == "number" && value == "NaN";

        internal bool CanBeConvertedTo(string type)
        {
            if (this.type == type) return true;
            if (type == "string") return true;

            if (this.type == "string")
            {
                if (type == "number")
                {
                    double val = 0;
                    return double.TryParse(this.value.Replace('.', ','), out val);
                }
                if (type == "boolean") return true;
            }

            if (this.type == "boolean" && type != "object") return true;
            if (this.type == "number" && type != "object") return true;

            return false;
        }

        internal Variable ConvertTo(string type)
        {
            if (this.type == type) return this;
            if (type == "string") return new Variable(this.value, "string");

            if (!this.CanBeConvertedTo(type)) return new Variable(Interpreter.GetDefaultFor(type), type);

            if (this.type == "string")
            {
                if (type == "number")
                    return new Variable(Native.Parse(this.value.Replace('.', ',')));

                if (type == "boolean")
                    return new Variable(this.value == "" ? "false" : "true", "boolean");
            }

            if(this.type == "boolean" && type == "number")
                return new Variable(this.value == "true" ? "1" : "0", "number");

            if(this.type == "number" && type == "boolean")
                return new Variable(this.value == "0" ? "false" : "true", "boolean");

            return new Variable("null", "object");
        }

        internal Variable ToInt()
        {
            if (this.type == "number")
                return new Variable(this.value.Replace(',', '.').Split('.')[0] , "number");
            return this;
        }

        internal bool IsEqualTo(Variable right) => (right.ConvertTo(type).value == value || ConvertTo(right.type).value == right.value) && type != "object" && right.type != "object";
        internal bool IsDeepEqualTo(Variable right) => right.value == value && right.type == type && type != "object" && right.type != "object";

        internal bool IsGreaterThan(Variable right) =>
            Native.Parse(ConvertTo("number").value) > Native.Parse(right.ConvertTo("number").value);
        
        internal bool IsGreaterOrEqualTo(Variable right) =>
            Native.Parse(ConvertTo("number").value) >= Native.Parse(right.ConvertTo("number").value);

        internal void Set(Variable var)
        {
            args = var.args;
            expression = var.expression;
            fields = var.fields;
            prototype = var.prototype;
            type = var.type;
            value = var.value;
        }

        public Variable Clone()
        {
            if (type == "function" || type == "object") return this;

            Variable clone = new Variable(value, type);
            clone.prototype = prototype;
            clone.args = args;
            clone.fields = fields;
            return clone;
        }

        public Variable AddVariable(string name, Variable value, bool constant = true)
        {
            value.constant = constant;
            fields.Add(new Variable(name, "string"), value);
            return this;
        }

        public Variable AddFunction(string name, FunctionNode.ExecuteFunction function, string[] parameters, bool constant = true)
        {
            var func = new FunctionNode(name, new List<string>(parameters), function);
            func.constant = constant;
            fields.Add(new Variable(name, "string"), func);
            return this;
        }

        public Variable SetConstant(bool constant)
        {
            this.constant = constant;
            return this;
        }
    }

    public class FunctionNode : Variable
    {
        public delegate Variable ExecuteFunction(List<Variable> args);

        public string name;
        public List<string> parameters;
        public Block code;

        public ExecuteFunction function;
        public bool native;

        public FunctionNode(string name, List<string> parameters, Block code)
        {
            this.name = name;
            this.parameters = parameters;
            this.code = code;

            this.native = false;

            this.value = "function";
            this.type = "function";
        }

        public FunctionNode(string name, List<string> parameters, ExecuteFunction function)
        {
            this.name = name;
            this.parameters = parameters;
            this.function = function;

            this.native = true;

            this.value = "([native code])";
            this.type = "function";
        }
    }

    public class ReturnNode : Node
    {
        public Node expression;

        public ReturnNode(Node expression)
        {
            this.expression = expression;
            this.value = "return";
        }
    }

    public class BreakNode : Node
    {
        public BreakNode()
        {
            this.value = "break";
        }
    }

    public class ConditionalBlock : Node
    {
        public string type;

        public Node init, condition, post;
        public Node code;

        public Node oppositeCode;

        public ConditionalBlock(string type, Node condition, Node code)
        {
            this.type = type;

            this.condition = condition;
            this.code = code;
        }

        public ConditionalBlock(string type, Node init, Node condition, Node post, Node code)
        {
            this.type = type;

            this.init = init;
            this.condition = condition;
            this.post = post;

            this.code = code;
        }

        public ConditionalBlock(string type, Node condition, Node code, Node oppositeCode)
        {
            this.type = type;

            this.condition = condition;

            this.code = code;
            this.oppositeCode = oppositeCode;
        }
    }

    public class TokenNode : Node
    {
        public TokenNode(string value)
        {
            this.value = value;
        }
    }

    public class Operation : Node
    {
        public Operator op;
        public Node left, right;

        public Operation(Operator op, Node left, Node right)
        {
            this.op = op;
            this.left = left;
            this.right = right;

            this.value = this.op.symbol;
        }
    }
}
