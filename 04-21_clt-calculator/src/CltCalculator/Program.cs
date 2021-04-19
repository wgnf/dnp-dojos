using System;
using sly.lexer;
using sly.parser.generator;

namespace CltCalculator
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Testing CSLY!");

            var parserInstance = new ExpressionParser();
            var builder = new ParserBuilder<ExpressionToken, double>();
            var parser = builder.BuildParser(parserInstance, ParserType.LL_RECURSIVE_DESCENT, "expression").Result;

            const string testExpression = "-1 + 1 - 3";

            var result = parser.Parse(testExpression);

            if (result.IsOk)
                Console.WriteLine($"Result of {testExpression} is {result.Result}");
            else
            {
                result.Errors?.ForEach(error => Console.WriteLine(error.ErrorMessage));
            }
        }
    }

    public enum ExpressionToken
    {
        [Lexeme(@"(-?)(\d*)\.?(\d+)\s*")]
        NUMBER = 1,
        
        [Lexeme(@"\+\s*")]
        ADDITION = 2,
        
        [Lexeme(@"\-\s*")]
        SUBTRACTION = 3
    }

    public class ExpressionParser
    {
        [Production("expression: NUMBER")]
        public double DoubleExpression(Token<ExpressionToken> token)
        {
            var doubleValue = token.DoubleValue;
            return doubleValue;
        }
        
        [Production("term: NUMBER")]
        public double DoubleTerm(Token<ExpressionToken> token)
        {
            var doubleValue = token.DoubleValue;
            return doubleValue;
        }
        
        [Production("expression: term ADDITION expression")]
        public double AdditionExpression(double left, Token<ExpressionToken> operatorToken, double right)
        {
            var added = left + right;
            return added;
        }
        
        [Production("expression: term SUBTRACTION expression")]
        public double SubtractionExpression(double left, Token<ExpressionToken> operatorToken, double right)
        {
            var subtracted = left - right;
            return subtracted;
        }
    }
}
