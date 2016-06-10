using DL_Interpreter.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_Interpreter.Parser
{
    public class Operator
    {
        public readonly string symbol;
        public readonly int priority;
        public bool right;

        public Operator(string symbol, int priority, bool right = false)
        {
            this.symbol = symbol;
            this.priority = priority;
            this.right = right;
        }
    }
}
