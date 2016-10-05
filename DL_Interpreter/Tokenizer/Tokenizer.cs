using System;
using System.Collections.Generic;

namespace DL_Interpreter.Tokenizer
{
    public class Tokenizer
    {
        public static List<Token> Tokenize(string code)
        {
            int parsingMode = 0;
            bool escapeNext = false, hd = false;
            char stringStart = '\0';

            List<Token> tokens = new List<Token>(500);
            Token current = new Token(TokenType.Undefined, "");
            char smb;

            int line = 1, position = 0;

            for (var now = 0; now < code.Length; ++now)
            {
                smb = code[now];

                ++position;
                if( smb == '\n')
                {
                    ++line;
                    position = 1;
                }

                switch (parsingMode)
                {
                    case 0:
                        if (char.IsWhiteSpace(smb)) continue;
                        if (smb == '"' || smb == '\'')
                        {
                            stringStart = smb;
                            parsingMode = 1;
                            tokens.Add(new Token(TokenType.String, "", line, position, now));
                        }
                        else if (char.IsDigit(smb))
                        {
                            parsingMode = 2;
                            hd = false;
                            if(current.GetToken() == "-"
                                &&(tokens.Count == 1
                                    || tokens[tokens.Count - 2].GetTokenType() == TokenType.Token))
                                tokens[tokens.Count - 1] = new Token(TokenType.Number, "-" + smb, line, position, now);
                            else
                                tokens.Add(new Token(TokenType.Number, smb, line, position, now));
                        }
                        else if (char.IsLetter(smb) || smb == '_')
                        {
                            parsingMode = 3;
                            tokens.Add(new Token(TokenType.Identifier, smb, line, position, now));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Token, smb, line, position, now));
                            parsingMode = 4;
                        }
                        current = tokens[tokens.Count - 1];
                        break;
                    case 1:
                        if (escapeNext)
                        {
                            switch (smb)
                            {
                                case 'n': current.Append('\n'); break;
                                case 't': current.Append('\t'); break;
                                case 's': current.Append(' '); break;
                                default: current.Append(smb); break;
                            }
                            escapeNext = false;
                        }
                        else if (smb == stringStart)
                            parsingMode = 0;
                        else if (smb == '\\')
                            escapeNext = true;
                        else
                            current.Append(smb);
                        break;
                    case 2:
                        if((!char.IsDigit(smb) && smb != '.')
                            || (smb == '.' && hd))
                        {
                            parsingMode = 0;
                            if (!char.IsWhiteSpace(code[now]))
                            {
                                --now;
                                --position;
                            }
                            continue;
                        }
                        if(smb == '.') hd = true;

                        current.Append(smb);
                        break;
                    case 3:
                        if(!char.IsLetterOrDigit(smb) && smb != '_')
                        {
                            parsingMode = 0;
                            if (!char.IsWhiteSpace(code[now]))
                            {
                                --now;
                                --position;
                            }
                            continue;
                        }
                        current.Append(smb);
                        break;
                    case 4:
                        if (Interpreter.IsOperator(current.GetToken() + smb))
                        {
                            current.Append(smb);
                        }
                        else
                        {
                            parsingMode = 0;
                            if (!char.IsWhiteSpace(code[now]))
                            {
                                --now;
                                --position;
                            }
                        }
                        break;
                }
            }
            tokens.Add(new Token(TokenType.EOF, '\0', line + 1, 0, code.Length));

            return tokens;
        }
    }
}