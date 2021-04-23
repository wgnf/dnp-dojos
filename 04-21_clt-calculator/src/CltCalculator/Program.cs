using System;
using System.Globalization;
using CltCalculator.Calculation;
using CltCalculator.Contracts.Calculation;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Parsing;

namespace CltCalculator
{
    public static class Program
    {
        private static readonly IParser Parser = ParserFactory.GetParser();
        private static readonly ICalculator Calculator = new Calculator();
        
        public static void Main()
        {
            Console.WriteLine("Calculator!");
            
            while (true)
            {
                Console.WriteLine("Enter something that should be calculated:");
                var expression = Console.ReadLine();

                try
                {
                    var result = Calculate(expression);
                    Console.WriteLine($"Result: {result.ToString(CultureInfo.InvariantCulture)}\n\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        private static decimal Calculate(string expression)
        {
            var symbols = Parser.Parse(expression);
            var result = Calculator.Calculate(symbols);

            return result;
        }
    }
}
