namespace CalculatorServer.Services
{
    public interface ICalculatorService
    {
        public void Calculate(string exp);
        public double GetAns();
        public Task<double> CalculateComplexExpression(string exp);
    }
}
