using System;
using System.Collections.Generic;
using System.IO;

namespace Dormez
{
    public static class Lexer
    {
        public const string SINGLE_LINE_COMMENT = "//";
        public const string MULTILINE_COMMENT_START = "/*";
        public const string MULTILINE_COMMENT_END = "*/";

        public const string IDENTIFIER_CODE = "identifier";
        public const string STRING_CODE = "string";
        public const string CHAR_CODE = "char";
        public const string NUMBER_CODE = "number";
        public const string EOF_CODE = "eof";

        public const char STRING_START = '"';
        public const char STRING_END = '"';

        public const char CHAR_START = '\'';
        public const char CHAR_END = '\'';

        // { text to look for, token ID }
        public static readonly Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            { "declare", "var" },
            { "from", "from" },
            { "with", "with" },
            { "to", "to" },
            { "for", "for" },
            { "each", "each" },
            { "in", "in" },
            { "by", "by" },
            { "of", "of" },
            { "return", "return" },
            { "table", "table" },
            { "template", "structure" },
            { "using", "using" },
            { "this", "this" },
            { "extending", "extending" },
            { "base", "base" },
            { "include", "include" },

            { "if", "if" },
            { "else", "else" },
            { "while", "while" },
            { "until", "until" },
            { "break", "break" },
            { "continue", "continue" },
            { "function", "function" },
            { "try", "try" },
            { "catch", "catch" },
            { "not", "not" },
            { "and", "and" },
            { "or", "or" }
        };

        // When found, will return specific token
        public static readonly Dictionary<string, Token> specialKeywords = new Dictionary<string, Token>
        {
            { "true", new Token("bool", true) },
            { "false", new Token("bool", false) },
            { "undefined", new Token("undefined") },
        };

        // From longest to shortest
        public static readonly Dictionary<string, string> symbols = new Dictionary<string, string>()
        {
            { "++", "increment" },
            { "--", "decrement" },

            { "!=", "not equal" },
            { "==", "double equal" },
            { "<=", "less or equal" },
            { ">=", "greater or equal" },

            { "+", "plus" },
            { "-", "minus" },
            { "*", "multiply" },
            { "/", "divide" },
            { "^", "exponent" },
            { "%", "modulus" },

            { "<", "less than" },
            { ">", "greater than" },

            { "(", "l bracket" },
            { ")", "r bracket" },
            { "{", "l curly" },
            { "}", "r curly" },
            { "[", "l square" },
            { "]", "r square" },

            { "=", "equals" },

            { "!", "not" },
            { "&", "and" },
            { "|", "or" },

            { ".", "dot" },
            { ",", "comma" },
            { ":", "colon" },
            { ";", "semicolon" },
        };

        private static string inputText;

        private static int pointer = 0;
        private static int line = 1;
        private static int column = 0;

        public static List<Token> ScanFile(string file)
        {
            return ScanString(File.ReadAllText(file));
        }

        public static List<Token> ScanString(string input)
        {
            inputText = input;

            List<Token> list = new List<Token>();

            while (true)
            {
                Token tk = GetNextToken();//GetNextToken();

                list.Add(tk);

                if (tk == EOF_CODE)
                {
                    break;
                }
            }

            pointer = 0;
            line = 1;
            column = 0;

            return list;
        }

        private static Token GetNextToken()
        {
            if (pointer >= inputText.Length)
            {
                return new Token(EOF_CODE, CurrentLocation);
            }

            if (CurrentChar == '\n')
            {
                NewLine();
                Eat();
            }

            if (char.IsWhiteSpace(CurrentChar))
            {
                SkipWhitespace();
            }

            if (pointer >= inputText.Length)
            {
                return new Token(EOF_CODE, CurrentLocation);
            }

            // If symbol
            if (IsSymbol(CurrentChar))
            {
                var loc = CurrentLocation;
                string symbolCombo = GetSymbolCombo();

                if(CurrentChar == STRING_START)
                {
                    Eat();
                    return new Token(STRING_CODE, CurrentLocation, GetString());
                }
                else if (CurrentChar == CHAR_START)
                {
                    Eat();

                    if (CurrentChar == '\\')
                    {
                        Eat();
                    }

                    char c = Eat();

                    if (CurrentChar == CHAR_END)
                    {
                        Eat();
                    }
                    else
                    {
                        throw new LexerException(CurrentLocation, "Expected '" + CHAR_END + "' to end character");
                    }

                    return new Token(CHAR_CODE, loc, c);
                }
                else if (symbolCombo == SINGLE_LINE_COMMENT)
                {
                    SkipSingleLineComment();
                    return GetNextToken();
                }
                else if (symbolCombo == MULTILINE_COMMENT_START)
                {
                    Eat(MULTILINE_COMMENT_START.Length);
                    SkipMultilineComment();
                    return GetNextToken();
                }
                else
                {
                    var type = GetSymbol(symbolCombo);
                    return new Token(type, loc);
                }
            }

            // If number
            if (char.IsDigit(CurrentChar))
            {
                return new Token(NUMBER_CODE, CurrentLocation, GetNumber());
            }

            // If letter
            if (char.IsLetter(CurrentChar))
            {
                var loc = CurrentLocation;
                var ident = GetIdentifier();

                if (keywords.ContainsKey(ident))
                {
                    return new Token(keywords[ident], loc);
                }
                else if(specialKeywords.ContainsKey(ident))
                {
                    return specialKeywords[ident];
                }
                else
                {
                    return new Token(IDENTIFIER_CODE, loc, ident);
                }
            }

            throw new LexerException(CurrentLocation, "Unrecognized character: " + CurrentChar);
        }

        private static char Eat()
        {
            var c = CurrentChar;
            pointer++;
            column++;

            if(c == '\n')
            {
                NewLine();
            }

            return c;
        }

        private static void Eat(int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                Eat();
            }
        }

        private static void Eat(char expected)
        {
            if (CurrentChar == expected)
            {
                Eat();
            }
            else
            {
                throw new LexerException(CurrentLocation, "Expected character '" + expected + "'");
            }
        }

        private static CodeLocation CurrentLocation
        {
            get
            {
                return new CodeLocation(line, column);
            }
        }

        private static void NewLine()
        {
            column = 0;
            line++;
        }

        private static string GetString()
        {
            string result = "";

            while (true)
            {
                if (IsSymbol(CurrentChar))
                {
                    if (CurrentChar == STRING_END)
                    {
                        Eat();
                        return result;
                    }
                }

                // \n is hardcoded into strings
                if (CurrentChar == '\\')
                {
                    Eat();

                    if (CurrentChar == 'n')
                    {
                        result += Environment.NewLine;
                        Eat();
                        continue;
                    }
                }

                result += CurrentChar;

                Eat();
            }
        }

        //private static LazuritNumber GetNegativeNumber()
        //{
        //    string result = "-";

        //    bool usedDecimal = false;

        //    while (pointer < inputText.Length && (char.IsDigit(CurrentChar) || (CurrentChar == '.' && !usedDecimal && char.IsDigit(NextChar))))
        //    {
        //        result += CurrentChar;

        //        if (CurrentChar == '.')
        //        {
        //            usedDecimal = true;
        //        }

        //        Eat();
        //    }

        //    return new LazuritNumber(float.Parse(result));
        //}

        private static float GetNumber()
        {
            string result = "";

            bool usedDecimal = false;

            while (pointer < inputText.Length && (char.IsDigit(CurrentChar) || (CurrentChar == '.' && !usedDecimal && char.IsDigit(NextChar))))
            {
                result += CurrentChar;

                if (CurrentChar == '.')
                {
                    usedDecimal = true;
                }

                Eat();
            }

            if (usedDecimal)
            {
                if (CurrentChar == 'f' || CurrentChar == 'd')
                {
                    Eat();
                }
            }
            else
            {
                if (CurrentChar == 'i')
                {
                    Eat();
                }
            }

            return float.Parse(result);
        }

        private static string GetIdentifier()
        {
            string result = "";

            while (pointer < inputText.Length && (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_'))
            {
                result += CurrentChar;
                Eat();
            }

            return result;
        }

        private static void SkipWhitespace()
        {
            while (pointer < inputText.Length && char.IsWhiteSpace(CurrentChar))
            {
                if (CurrentChar == '\n')
                {
                    NewLine();
                }

                Eat();
            }
        }

        private static char CurrentChar
        {
            get {
                if(pointer >= inputText.Length) { return (char)0; }
                else { return inputText[pointer]; }
            }
        }

        private static char NextChar
        {
            get { return inputText[pointer + 1]; }
        }

        private static string GetEquals()
        {
            string result = "=";
            Eat();
            if (CurrentChar == '=')
            {
                result += '=';
                Eat();
            }
            return result;
        }

        private static string GetComparator(string result)
        {
            Eat();
            if (CurrentChar == '=')
            {
                result += '=';
                Eat();
            }
            return result;
        }

        private static void SkipSingleLineComment()
        {
            while (pointer < inputText.Length && CurrentChar != '\n')
            {
                Eat();
            }
        }

        private static void SkipMultilineComment()
        {
            while (true)
            {
                if (pointer >= inputText.Length)
                {
                    break;
                }

                if (IsSymbol(CurrentChar))
                {
                    if (GetSymbolCombo() == MULTILINE_COMMENT_END)
                    {
                        Eat(MULTILINE_COMMENT_END.Length);
                        break;
                    }
                }

                Eat();
            }
        }

        private static int GetPosition()
        {
            return pointer;
        }

        public static bool IsSymbol(char c)
        {
            return !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c);
        }

        private static string GetSymbolCombo()
        {
            string symbolCollection = "";

            int op = pointer;

            while (pointer < inputText.Length && IsSymbol(CurrentChar))
            {
                symbolCollection += CurrentChar;
                pointer++;
            }

            pointer = op;

            return symbolCollection;
        }

        private static string GetSymbol(string symbolCollection)
        {
            var location = CurrentLocation;

            for (int i = symbolCollection.Length; i > 0; i--)
            {
                string trimmed;

                if (i == symbolCollection.Length)
                {
                    trimmed = symbolCollection;
                }
                else
                {
                    trimmed = symbolCollection.Remove(i);
                }

                if (symbols.ContainsKey(trimmed))
                {
                    pointer += trimmed.Length;
                    return symbols[trimmed];
                }
            }

            throw new LexerException(location, "Unrecognized symbol combination (" + symbolCollection + ")");
        }

    }
}
