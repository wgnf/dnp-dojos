using System;
using CltCalculator.Contracts;
using CltCalculator.Contracts.Calculation;
using CltCalculator.Contracts.Parsing;

namespace CltCalculator.Calculation
{
    internal class ExpressionCalculator : IExpressionCalculator
    {
        private readonly IParser _parser;
        private readonly ICalculator _calculator;

        public ExpressionCalculator(
            IParser parser,
            ICalculator calculator)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }
        
        public decimal Calculate(string expression)
        {
            var symbols = _parser.Parse(expression);
            var result = _calculator.Calculate(symbols);

            return result;
        }
    }
}