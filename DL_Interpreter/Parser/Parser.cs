using DL_Interpreter.Tokenizer;
using System.Collections.Generic;

namespace DL_Interpreter.Parser
{
    public class Parser
    {
        private static List<Token> tokens;
        private static int index = -1;

        public static Block CreateSyntaxTree(List<Token> ts)
        {
            tokens = ts;
            var block = new Block();
            index = 0;

            while (index < tokens.Count - 1)
                block.Append(OptimizeExpression(ParseLine()));

            return block;
        }

        /* private static void DebugAstPrint(Node tree, int depth = 0)
        {
            var top = tree as Operation;
            if (top != null)
            {
                if (depth != 0) Interpreter.Write("(");
                DebugAstPrint(top.left, depth + 1);

                Interpreter.Write(" " + top.op.symbol + " ");

                DebugAstPrint(top.right, depth + 1);
                if (depth != 0) Interpreter.Write(")");

                return;
            }

            var tvar = tree as Variable;
            if (tvar != null) Interpreter.Write(tvar.value);
        } */

        public static Node OptimizeExpression(Node expression)
        {
            var eop = expression as Operation;
            if (eop != null)
            {
                var left = OptimizeExpression(eop.left);
                var right = OptimizeExpression(eop.right);

                var var_l = left as Variable;
                var var_r = right as Variable;

                if ( var_l != null && var_r != null )
                {
                    if(var_l.type != "variable" && var_r.type != "variable")
                    {
                        switch(eop.op.symbol)
                        {
                            case "+": return VariableOperations.sum(var_l, var_r);
                            case "-": return VariableOperations.sub(var_l, var_r);
                            case "*": return VariableOperations.mul(var_l, var_r);
                            case "/": return VariableOperations.div(var_l, var_r);
                            case "%": return VariableOperations.res(var_l, var_r);
                        }
                    }
                }

                return new Operation(eop.op, left, right);
            }

            return expression;
        }

        private static Block ParseBlock()
        {
            ++index;
            Block block = new Block();

            while(Current().GetToken() != "}")
                block.Append(ParseLine());
            ++index;

            return block;
        }

        private static Node ParseLine()
        {
            var t = Current().GetToken();
            if (t == "if" || t == "for" || t == "while") return ParseConditionalOperator();
            if (t == "return")
            {
                ++index;
                return new ReturnNode(ParseExpression());
            }
            if (t == "break")
            {
                index += 2;
                return new BreakNode();
            }

            return ParseExpression();
        }

        private static Node ParseConditionalOperator()
        {
            string type = Peek().GetToken();
            if (Current().GetToken() != "(")
                throw new ParsingError("Unexpected symbol after keyword " + type, Current(), Current().GetIndex(), Current().GetLength());
            ++index;

            Node init, condition, post, code;

            // FOR CYCLE PARSING
            if (type == "for")
            {
                init = ParseExpression();
                condition = ParseExpression();
                post = ParseMathExpression(ParseUnit());

                if (Current().GetToken() == "{") code = ParseBlock();
                else code = ParseLine();

                return new ConditionalBlock("for", init, condition, post, code);
            }

            // IF OPERATOR PARSING
            if (type == "if")
            {
                condition = ParseMathExpression(ParseUnit());

                if (Current().GetToken() == "{") code = ParseBlock();
                else code = ParseLine();

                Node oppositeCode = null;

                if (Current().GetToken() == "else")
                {
                    ++index;
                    if (Current().GetToken() == "{") oppositeCode = ParseBlock();
                    else oppositeCode = ParseLine();
                }

                return new ConditionalBlock("if", condition, code, oppositeCode);
            }

            // WHILE CYCLE PARSING
            condition = ParseMathExpression(ParseUnit());

            if (Current().GetToken() == "{") code = ParseBlock();
            else code = ParseLine();

            return new ConditionalBlock("while", condition, code);
        }

        /// <summary>
        /// Parse inline expression. Stops at semicolon(;). If there is no semicolon at the end, it throws Exception
        /// </summary>
        /// <returns>Parsed AST of expression as Node</returns>
        private static Node ParseExpression()
        {
            if (Current().GetToken() == ";" || Current().GetToken() == "}")
            {
                ++index;
                return new Block();
            }

            var position = Current().GetIndex();
            Node expression = ParseMathExpression(ParseUnit());
            --index;
            if (Current().GetToken() != ";")
                throw new ParsingError("Expected semicolon(;) at the end of expression",
                    tokens[--index], position, Current().GetIndex() - position);
            ++index;
            return expression;
        }

        private static Node ParseMathExpression(Node left, int level = 0)
        {
            if (left as TokenNode != null) return new Block();

            var l_op = GetOperator(Peek().GetToken());
            if (l_op == null) return left;

            var right = ParseUnit();

            // If operator is rigth-sided(like equal sign(=)) then execute block after it first.
            if (l_op.right) return new Operation(l_op, left, ParseMathExpression(right));

            // Get next operator
            var r_op = GetOperator(Peek().GetToken());

            while (r_op != null && r_op.priority > level)
            {
                if (r_op.priority > l_op.priority)
                {
                    --index;
                    right = ParseMathExpression(right, l_op.priority);
                    --index;

                    left = new Operation(l_op, left, right);
                    if (Current().GetToken() == ";")
                    {
                        ++index;
                        return left;
                    }

                    l_op = GetOperator(Peek().GetToken());
                    if (l_op == null) return left;
                }
                else
                {
                    left = new Operation(l_op, left, right);
                    l_op = r_op;
                }

                right = ParseUnit();

                r_op = GetOperator(ParseUnit().value);
            }
            
            return new Operation(l_op, left, right);
        }

        private static Node ParseUnit()
        {
            Node unit = GetCurrentNode();

            if (unit != null)
            {
                if (Current().GetToken() == "." || Current().GetToken() == "[")
                {
                    while (true)
                    {
                        if (Current().GetToken() == ".")
                        {
                            ++index;

                            var right = GetTokenNode(Peek()) as Variable;
                            if (right?.type != "variable")
                                throw new ParsingError("Expected identifier after dot",
                                    Current(), Current().GetIndex(), Current().GetLength());

                            right.type = "string";
                            unit = new Operation(GetOperator("."), unit, right);
                        }
                        else if (Current().GetToken() == "[")
                        {
                            ++index;
                            unit = new Operation(GetOperator("."), unit, ParseMathExpression(ParseUnit()));
                        }
                        else if (Current().GetToken() == "(")
                        {
                            unit = ParseFunctionCall(unit);
                        }
                        else break;
                    }
                }

                return unit;
            }

            return new TokenNode(Peek().GetToken());
        }

        private static Node GetCurrentNode()
        {
            if (Current().GetTokenType() == TokenType.Token && Current().GetToken() == "(")
            {
                ++index;
                var unit = ParseMathExpression(ParseUnit());

                if (Current().GetToken() == "(") return ParseFunctionCall(unit);
                return unit;
            }

            if (Current().GetTokenType() == TokenType.EOF)
            {
                var last = Current();
                if (tokens.Count != 1) last = tokens[tokens.Count - 2];
                throw new ParsingError("Unexpected end of file", last, last.GetIndex(), last.GetLength());
            }

            if (Current().GetTokenType() == TokenType.Identifier)
            {
                if (Interpreter.IsPredefined(Current().GetToken()))
                    return new Variable(Current().GetToken(), Interpreter.GetTypeOfPredefined(Peek().GetToken()));

                if (tokens[index + 1].GetToken() == "(" && tokens[index + 1].GetTokenType() == TokenType.Token)
                {
                    if (Current().GetToken() == "function")
                        return ParseFunctionCreation();

                    return ParseFunctionCall(new Variable(Peek().GetToken(), "variable"));
                }

                return new Variable(Peek().GetToken(), "variable");
            }

            if (Current().GetTokenType() == TokenType.Number)
                return new Variable(Peek().GetToken(), "number");

            if (Current().GetTokenType() == TokenType.String)
                return new Variable(Peek().GetToken(), "string");

            if (Current().GetToken() == "{")
                return ParseObjectCreation();

            return null;
        }

        public static Node GetTokenNode(Token token)
        {
            if (Current().GetTokenType() == TokenType.EOF)
            {
                var last = Current();
                if (tokens.Count != 1) last = tokens[tokens.Count - 2];
                throw new ParsingError("Unexpected end of file", last, last.GetIndex(), last.GetLength());
            }

            if (token.GetTokenType() == TokenType.Identifier)
                return new Variable(token.GetToken(), "variable");

            if (token.GetTokenType() == TokenType.Number)
                return new Variable(token.GetToken(), "number");

            if (token.GetTokenType() == TokenType.String)
                return new Variable(token.GetToken(), "string");

            return new TokenNode(token.GetToken());
        }

        private static Node ParseFunctionCall(Node expression)
        {
            ++index;
            List<Node> args = new List<Node>(10);
            while (true)
            {
                var start = Current().GetIndex();
                args.Add(ParseMathExpression(ParseUnit()));
                --index;

                if (Current().GetToken() == ")") break;

                if (Current().GetToken() != "," && Current().GetTokenType() != TokenType.Token)
                    throw new ParsingError("Expected comma(,) between function arguments",
                        Current(), start, Current().GetIndex() - start);
                
                ++index;
            }
            ++index;
            
            if ( Current().GetTokenType() == TokenType.Token && Current().GetToken() == "(")
                return ParseFunctionCall(new Variable(expression, args));
            return new Variable(expression, args);
        }

        private static Node ParseFunctionCreation()
        {
            index += 2;

            var args = new List<string>(10);
            Token currentArgument;

            while(Current().GetToken() != ")")
            {
                if ((currentArgument = Current()).GetTokenType() != TokenType.Identifier)
                    throw new ParsingError("Expected parameter name",
                        Current(), Current().GetIndex(), Current().GetLength());
                ++index;
                args.Add(currentArgument.GetToken());

                if (Current().GetTokenType() == TokenType.Token && Current().GetToken() == ")") break;

                if ( Current().GetTokenType() != TokenType.Token || Current().GetToken() != ",")
                    throw new ParsingError("Expected comma(,) between parameters",
                        Current(), Current().GetIndex(), Current().GetLength());

                ++index;
            }
            ++index;

            Block code = new Block();
            if (Current().GetToken() == "{")
            {
                code = ParseBlock();
                ++index;
            }
            else if (Peek().GetToken() == "=" && Peek().GetToken() == ">")
                code.Append(new ReturnNode(ParseMathExpression(ParseUnit())));
            else
                throw new ParsingError("Unexpected symbol",
                    Current(), Current().GetIndex(), Current().GetLength());

            --index;
            return new FunctionNode("anonymous", args, code);
        }

        private static Node ParseObjectCreation()
        {
            ++index;
            if (Current().GetToken() == "}" && Current().GetTokenType() == TokenType.Token)
            {
                ++index;
                return new Variable("", "object");
            }

            var obj = new Variable("", "object");

            Variable key;
            Node value;

            while(true)
            {
                key = ParseUnit() as Variable;
                if (key.type == "variable") key.type = "string";

                if (key == null)
                {
                    --index;
                    throw new ParsingError("Expected value as key", Current(), Current().GetIndex(), Current().GetLength());
                }

                if (Current().GetToken() != ":" || Peek().GetTokenType() != TokenType.Token)
                    throw new ParsingError("Expected (:) after key", tokens[--index], Current().GetIndex(), Current().GetLength());

                value = ParseMathExpression(ParseUnit());
                --index;
                obj.fields.Add(key, value);

                if (Current().GetToken() == "}" && Current().GetTokenType() == TokenType.Token) break;
                
                if (Current().GetToken() != "," || Peek().GetTokenType() != TokenType.Token)
                    throw new ParsingError("Expected comma(,) after value",
                        tokens[--index], Current().GetIndex(), Current().GetLength());
            }
            ++index;

            return obj;
        }

        private static Operator GetOperator(string symbol)
        {
            foreach (Operator op in Interpreter.operators)
                if (op.symbol == symbol)
                    return op;
            return null;
        }

        private static Token Current() => tokens[index];
        private static Token Peek() => tokens[index++];
    }
}