using System.ComponentModel;

namespace CalculatorServer.Services
{
    public class Parser
    {
        public enum OperatorType
        {
            Plus,
            Minus,
            Mult,
            Div,
        };

        public enum TokenType
        {
            Operator,
            Number,
            Parenthesis,
        };

        public interface Token
        {
            public TokenType GetType();
        };

        public class Operator
            : Token
        {
            public OperatorType Type { get; private set; }

            public Operator(OperatorType type) => Type = type;

            public new TokenType GetType()
            {
                return TokenType.Operator;
            }

            private static int OpPrecedence(OperatorType op)
            {
                switch (op)
                {
                    case OperatorType.Mult:
                    case OperatorType.Div:
                        return 1;
                    case OperatorType.Plus:
                    case OperatorType.Minus:
                        return 0;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported operator");
                }
            }

            public static bool CmpPrecedence(OperatorType op0, OperatorType op1)
            {
                return OpPrecedence(op0) > OpPrecedence(op1);
            }
        }

        public class Number
            : Token
        {
            public double Value { get; private set; }

            public Number(double val) => Value = val;

            public new TokenType GetType()
            {
                return TokenType.Number;
            }
        }

        public class Parenthesis
            : Token
        {
            public string Content { get; private set; }

            public Parenthesis(string content) => Content = content;

            public new TokenType GetType()
            {
                return TokenType.Parenthesis;
            }
        }

        public static List<Token> Parse(string input)
        {
            var parseList = new List<Token>();
            string temp = "";
            bool inParenthesis = false;
            int parenthesisDepth = 0;
            foreach (char c in input)
            {
                if (inParenthesis)
                {
                    if (c == '(')
                    {
                        parenthesisDepth += 1;
                        temp += c;
                    }
                    else if (c == ')')
                    {
                        if (parenthesisDepth == 0)
                        {
                            parseList.Add(new Parenthesis(temp));
                            temp = "";
                            inParenthesis = false;
                        }
                        else
                        {
                            parenthesisDepth -= 1;
                            temp += c;
                        }
                    }
                    else
                    {
                        temp += c;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case ' ':
                            break;
                        case '+':
                            if (temp.Length > 0)
                            {
                                parseList.Add(new Number(Double.Parse(temp)));
                                temp = "";
                            }
                            parseList.Add(new Operator(OperatorType.Plus));
                            break;
                        case '-':
                            if (temp.Length > 0)
                            {
                                parseList.Add(new Number(Double.Parse(temp)));
                                temp = "";
                            }
                            parseList.Add(new Operator(OperatorType.Minus));
                            break;
                        case '*':
                            if (temp.Length > 0)
                            {
                                parseList.Add(new Number(Double.Parse(temp)));
                                temp = "";
                            }
                            parseList.Add(new Operator(OperatorType.Mult));
                            break;
                        case '/':
                            if (temp.Length > 0)
                            {
                                parseList.Add(new Number(Double.Parse(temp)));
                                temp = "";
                            }
                            parseList.Add(new Operator(OperatorType.Div));
                            break;
                        case '(':
                            inParenthesis = true;
                            break;
                        case ')':
                            break;
                        default:
                            temp += c;
                            break;
                    }

                }
            }
            if (temp.Length > 0)
            {
                parseList.Add(new Number(Double.Parse(temp)));
            }
            return parseList;
        }
    }
}
