using System;
using System.Globalization;
using System.Text;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Models;

namespace CltCalculator.Parsing.Parts
{
    public class ConstantParserPart : IParserPart
    {
        public bool ParsesOperations => false;

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
            var firstRound = true;
            
            while (currentPosition < expression.Length)
            {
                char result;
                bool couldParse;
                var nextAvailable = currentPosition + 1 < expression.Length;

                couldParse = nextAvailable switch
                {
                    true when firstRound => TryParseCurrentCharFirstRoundAndNextAvailable(expression, currentPosition,
                        out result),
                    true => TryParseCurrentCharWhenNextAvailable(expression, currentPosition, out result),
                    _ => TryParseCurrentCharWhenNextUnavailable(expression, currentPosition, out result)
                };

                if (couldParse)
                {
                    stringBuilder.Append(result);

                    currentPosition++;
                    firstRound = false;
                    
                    continue;
                }

                break;
            }

            return stringBuilder.ToString();
        }

        private static bool TryParseCurrentCharFirstRoundAndNextAvailable(
            string expression,
            int currentPosition,
            out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            var nextChar = expression[currentPosition + 1];
            parsedChar = currentChar;

            var isCurrentPlusOrMinus = currentChar == '+' || currentChar == '-';
            var isNextDotOrNumber = nextChar == '.' || IsNumber(nextChar);

            // something like:
            // +111
            // -111
            // +.111
            // -.111
            if (isCurrentPlusOrMinus && isNextDotOrNumber)
                return true;
            
            return TryParseCurrentCharWhenNextAvailable(expression, currentPosition, out parsedChar);
        }

        private static bool TryParseCurrentCharWhenNextAvailable(
            string expression,
            int currentPosition,
            out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            var nextChar = expression[currentPosition + 1];
            parsedChar = currentChar;

            var isCurrentDot = currentChar == '.';
            var isNextANumber = IsNumber(nextChar);

            if (isCurrentDot && isNextANumber)
                return true;

            return TryParseCurrentCharWhenNextUnavailable(expression, currentPosition, out parsedChar);
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