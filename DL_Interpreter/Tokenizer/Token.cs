using System;

namespace DL_Interpreter.Tokenizer
{
    public class Token
    {
        private string token;
        private TokenType type;
        private int line, position, index;
        
        public Token(TokenType type, string token, int line, int position, int index)
        {
            this.token = token;
            this.type = type;
            this.position = position;
            this.line = line;
            this.index = index;
        }

        public Token(TokenType type, char token, int line, int position, int index)
            : this(type, token.ToString(), line, position, index)
        {}

        public Token(TokenType type, string token)
            : this(type, token, 0, 0, 0)
        {}

        public void Append(char smb) => token += smb;
        public string GetToken() => token;
        public TokenType GetTokenType() => type;

        public int GetLine() => line;
        public int GetPosition() => position;
        public int GetIndex() => index;
        public int GetLength() => token.Length;
    }
}
