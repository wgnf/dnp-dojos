using System;
using System.Globalization;
using System.Text;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Models;

namespace CltCalculator.Parsing.Parts
{
    public class ConstantParserPart : IParserPart
    {
        public bool TryParse(string expression, int currentPosition, out Symbol symbol)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(expression));

            var startingPosition = currentPosition;
            symbol = null;


            var result = ParseConstant(expression, currentPosition);
            if (string.IsNullOrWhiteSpace(result))
                return false;

            symbol = CreateSymbolFromParsedResult(result, startingPosition);
            return true;
        }

        private static string ParseConstant(string expression, int currentPosition)
        {
            var stringBuilder = new StringBuilder();

            while (currentPosition < expression.Length)
            {
                char result;
                var couldParse = currentPosition + 1 < expression.Length
                    ? TryParseCurrentCharWhenNextAvailable(expression, currentPosition, out result)
                    : TryParseCurrentCharWhenNextUnavailable(expression, currentPosition, out result);

                if (couldParse)
                {
                    stringBuilder.Append(result);

                    currentPosition++;
                    continue;
                }

                break;
            }

            return stringBuilder.ToString();
        }

        private static bool TryParseCurrentCharWhenNextAvailable(
            string expression,
            int currentPosition,
            out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            var nextChar = expression[currentPosition + 1];
            parsedChar = currentChar;

            switch (currentChar)
            {
                // something like:
                // +111
                // -111
                // +.111
                // -.111
                case '+' or '-' when nextChar == '.' || IsNumber(nextChar):
                // something like:
                // .111
                case '.' when IsNumber(nextChar):
                    return true;
                default:
                    return TryParseCurrentCharWhenNextUnavailable(expression, currentPosition, out parsedChar);
            }
        }

        private static bool TryParseCurrentCharWhenNextUnavailable(
            string expression,
            int currentPosition,
            out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            parsedChar = currentChar;

            return IsNumber(currentChar);
        }

        private static Symbol CreateSymbolFromParsedResult(string parsedResult, int startPosition)
        {
            var decimalValue = decimal.Parse(parsedResult, CultureInfo.InvariantCulture);
            var symbol = new Symbol(
                SymbolType.Constant,
                startPosition,
                parsedResult.Length,
                decimalValue);

            return symbol;
        }
        
        private static bool IsNumber(char @char)
        {
            return char.IsDigit(@char);
        }
    }
}