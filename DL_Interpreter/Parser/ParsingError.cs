using DL_Interpreter.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_Interpreter.Parser
{
    class ParsingError : Exception
    {
        public ParsingError(string message, Token tk, int startPosition, int length)
            : base("Error occured at " + tk.GetLine() + ":" + tk.GetPosition() + " : " + message)
        {
            Data.Add("position", startPosition);
            Data.Add("length", length);
        }
    }
}
