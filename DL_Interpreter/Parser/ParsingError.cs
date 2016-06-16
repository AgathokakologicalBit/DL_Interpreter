using DL_Interpreter.Tokenizer;
using System;

namespace DL_Interpreter.Parser
{
    class ParsingError : Exception
    {
        public ParsingError(string message) : base(message) {}

        public ParsingError(string message, Token tk, int startPosition, int length)
            : base("Error occured at " + tk.GetLine() + ":" + tk.GetPosition() + " : " + message)
        {
            Data.Add("position", startPosition);
            Data.Add("length", length);
        }
    }
}
