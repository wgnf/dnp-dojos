using CltCalculator.Calculation;
using CltCalculator.Contracts;
using CltCalculator.Parsing;

namespace CltCalculator
{
    public static class ExpressionCalculatorFactory
    {
        public static IExpressionCalculator GetExpressionCalculator()
        {
            var parser = ParserFactory.GetParser();
            var calculator = new Calculator();

            return new ExpressionCalculator(parser, calculator);
        }
    }
}