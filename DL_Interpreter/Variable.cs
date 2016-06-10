using DL_Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_Interpreter
{
    public class Variable
    {
        public string name;
        
        public Parser.Variable value;

        public Variable(string name, string type, Parser.Variable value)
        {
            this.name = name;
            this.value = value;
            this.value.type = type;
        }
    }
}
