namespace CalculatorServer.Services
{
    public class Calculator
    : ICalculatorService
    {
        private double State = 0;
        //private Parser.OperatorType CurrOp;
        private double Ans = 0;

        private double[] Register = new double[] { 0, 0, 0, 0, 0, 0 };

        public Calculator() { }

        private Calculator(double State, Parser.OperatorType CurrOp, double Ans, double[] Register)
        {
            this.State = State;
            //this.CurrOp = CurrOp;
            this.Ans = Ans;
            this.Register = Register;
        }

        private double ArithmeticOps(Parser.OperatorType op, double lhs, double rhs)
        {
            if (op == Parser.OperatorType.Plus)
            {
                return lhs + rhs;
            }
            else if (op == Parser.OperatorType.Minus)
            {
                return lhs - rhs;
            }
            else if (op == Parser.OperatorType.Mult)
            {
                return lhs * rhs;
            }
            else
            {
                if (rhs == 0)
                {
                    Console.WriteLine("Division by zero.");
                    throw new DivideByZeroException();
                }
                else
                {
                    return lhs / rhs;
                }
            }
        }

        private List<Parser.Token> Reduce(List<Parser.Token> parseList)
        {
            if (parseList.Count <= 1)
            {
                return parseList;
            }
            int currMaxIdx;
            Parser.OperatorType currMaxOp;

            if (parseList[0] is Parser.Operator opToken0)
            {
                currMaxIdx = 0;
                currMaxOp = opToken0.Type;
            }
            else if (parseList[1] is Parser.Operator opToken1)
            {
                currMaxIdx = 1;
                currMaxOp = opToken1.Type;
            }
            else
            {
                currMaxIdx = 0;
                currMaxOp = Parser.OperatorType.Minus;
            }
            for (int i = 0; i < parseList.Count; i++)
            {
                if (parseList[i] is Parser.Operator operatorToken)
                {
                    Parser.OperatorType currOp = operatorToken.Type;
                    if (Parser.Operator.CmpPrecedence(currOp, currMaxOp))
                    {
                        currMaxIdx = i;
                        currMaxOp = currOp;
                    }
                }
                else if (parseList[i] is Parser.Parenthesis parenthesisToken)
                {
                    List<Parser.Token> innerParseList = Parser.Parse(parenthesisToken.Content);
                    while (innerParseList.Count > 1)
                    {
                        innerParseList = Reduce(innerParseList);
                    }
                    parseList[i] = innerParseList[0];
                }
            }
            if (parseList[currMaxIdx - 1] is Parser.Number numTokenLhs
                && parseList[currMaxIdx + 1] is Parser.Number numTokenRhs)
            {
                parseList[currMaxIdx] = new Parser.Number(ArithmeticOps(currMaxOp, numTokenLhs.Value, numTokenRhs.Value));
                parseList.RemoveAt(currMaxIdx + 1);
                parseList.RemoveAt(currMaxIdx - 1);
            }
            else
            {
                throw new Exception("parseList not parsed as expected");
            }
            return parseList;
        }

        public void Calculate(string line)
        {
            List<Parser.Token> parseList = Parser.Parse(line);
            if (parseList.Count == 0)
            {
                return;
            }
            if (parseList[0] is Parser.Operator)
            {
                parseList.Insert(0, new Parser.Number(Ans));
            }
            while (parseList.Count > 1)
            {
                parseList = Reduce(parseList);
            }
            if (parseList[0] is Parser.Number numberToken)
            {
                State = 0;
                Ans = numberToken.Value;
            }
            else
            {
                throw new Exception("parseList not parsed as expected");
            }
        }

        public async Task<double> CalculateComplexExpression(string line)
        {
            await Task.Delay(5000);
            var calculator = new Calculator();
            calculator.Calculate(line);
            return calculator.GetAns();
        }

        public void Print()
        {
            Console.WriteLine($"Answer: {Ans}");
        }

        public double GetAns() { return Ans; }

        public void Clear()
        {
            State = 0;
            Ans = 0;
        }
    }
}
