using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DL_Interpreter.Tokenizer;
using System.Collections.Generic;
using System.Diagnostics;

namespace DL_InterpreterTest
{
    [TestClass]
    public class TokenizerTest
    {
        [TestMethod]
        public void __Init__()
        {
            TestTokens(Tokenizer.Tokenize("0").ToArray(), new Token[] {
                new Token(TokenType.Number, "0"),
                new Token(TokenType.EOF, "\0")
            });
        }

        [TestMethod]
        public void BasicTest()
        {
            TestTokens(Tokenizer.Tokenize("127").ToArray(), new Token[] {
                new Token(TokenType.Number, "127"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("var").ToArray(), new Token[] {
                new Token(TokenType.Identifier, "var"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("'string'").ToArray(), new Token[] {
                new Token(TokenType.String, "string"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("*").ToArray(), new Token[] {
                new Token(TokenType.Token, "*"),
                new Token(TokenType.EOF, "\0")
            });
        }

        [TestMethod]
        public void StringTest()
        {
            TestTokens(Tokenizer.Tokenize("'normal string'").ToArray(), new Token[] {
                new Token(TokenType.String, "normal string"),
                new Token(TokenType.EOF, "\0"),
            });

            TestTokens(Tokenizer.Tokenize("\"normal string\"").ToArray(), new Token[] {
                new Token(TokenType.String, "normal string"),
                new Token(TokenType.EOF, "\0"),
            });

            TestTokens(Tokenizer.Tokenize("\"a is not 'b'\"").ToArray(), new Token[] {
                new Token(TokenType.String, "a is not 'b'"),
                new Token(TokenType.EOF, "\0"),
            });
        }

        [TestMethod]
        public void NumberTest()
        {
            TestTokens(Tokenizer.Tokenize("1").ToArray(), new Token[] {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("-1").ToArray(), new Token[] {
                new Token(TokenType.Number, "-1"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("1.5").ToArray(), new Token[] {
                new Token(TokenType.Number, "1.5"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("123.125").ToArray(), new Token[] {
                new Token(TokenType.Number, "123.125"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("-123.125").ToArray(), new Token[] {
                new Token(TokenType.Number, "-123.125"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("-123.125.a").ToArray(), new Token[] {
                new Token(TokenType.Number, "-123.125"),
                new Token(TokenType.Token, "."),
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.EOF, "\0")
            });
        }

        [TestMethod]
        public void OperatorTest()
        {
            TestTokens(Tokenizer.Tokenize("!=").ToArray(), new Token[] {
                new Token(TokenType.Token, "!="),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("!==").ToArray(), new Token[] {
                new Token(TokenType.Token, "!=="),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("<<a>>").ToArray(), new Token[] {
                new Token(TokenType.Token, "<"),
                new Token(TokenType.Token, "<"),
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Token, ">"),
                new Token(TokenType.Token, ">"),
                new Token(TokenType.EOF, "\0")
            });
        }

        [TestMethod]
        public void ComplexTest()
        {
            TestTokens(Tokenizer.Tokenize("a = 2;").ToArray(), new Token[] {
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Token, "="),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Token, ";"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("a += 2;").ToArray(), new Token[] {
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Token, "+="),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Token, ";"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("if a == 4 then:").ToArray(), new Token[] {
                new Token(TokenType.Identifier, "if"),
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Token, "=="),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.Identifier, "then"),
                new Token(TokenType.Token, ":"),
                new Token(TokenType.EOF, "\0")
            });

            TestTokens(Tokenizer.Tokenize("a = 2 - 1;").ToArray(), new Token[] {
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Token, "="),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Token, "-"),
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Token, ";"),
                new Token(TokenType.EOF, "\0")
            });
        }

        public void TestTokens(Token[] actual, Token[] expected)
        {
            Assert.AreEqual(expected.Length, actual.Length, string.Format("Expected length <{0}> but instead got <{1}>",
                expected.Length, actual.Length));
            for (int now = 0; now < expected.Length; ++now)
            {
                Assert.IsTrue(actual[now].GetToken() == expected[now].GetToken()
                    && actual[now].GetTokenType() == expected[now].GetTokenType(),
                    string.Format("Expected {0}({1}) but instead got {2}({3}) at {4}",
                        expected[now].GetTokenType(), expected[now].GetToken(),
                        actual[now].GetTokenType(), actual[now].GetToken(),
                        now));
            }
        }

        private TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}
