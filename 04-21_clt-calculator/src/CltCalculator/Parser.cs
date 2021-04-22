using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CltCalculator
{
    public class Parser : IParser
    {
        private readonly IEnumerable<Func<string, int, Symbol>> _parserParts;

        public Parser()
        {
            _parserParts = new List<Func<string, int, Symbol>>
            {
                ParseConstant
            };
        }

        public IEnumerable<Symbol> Parse(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(expression));

            var symbols = new List<Symbol>();
            var currentPosition = 0;
            
            while (currentPosition < expression.Length)
            {
                if (IsWhiteSpace(expression[currentPosition]))
                {
                    currentPosition++;
                    continue;
                }

                var symbol = ParseCurrent(expression, currentPosition);
                currentPosition += symbol.Length;
                symbols.Add(symbol);
            }
            
            return symbols;
        }

        private static bool IsWhiteSpace(char @char)
        {
            return char.IsWhiteSpace(@char);
        }

        private Symbol ParseCurrent(string expression, int currentPosition)
        {
            Symbol symbol = null;
            foreach (var parserPart in _parserParts)
            {
                if (symbol == null)
                    symbol = parserPart.Invoke(expression, currentPosition);
            }

            if (symbol != null)
                return symbol;
            
            throw new ParseUnexpectedSymbolException(expression[currentPosition], currentPosition);
        }
        
        private static Symbol ParseConstant(string expression, int currentPosition)
        {
            var startingPosition = currentPosition;
            var stringBuilder = new StringBuilder();

            while (currentPosition < expression.Length)
            {
                char result;
                var couldParse = currentPosition + 1 < expression.Length 
                    ? ParseCurrentCharForConstantWhenNextAvailable(expression, currentPosition, out result) 
                    : ParseCurrentCharForConstantWhenNextUnavailable(expression, currentPosition, out result);

                if (couldParse)
                {
                    stringBuilder.Append(result);

                    currentPosition++;
                    continue;
                }

                break;
            }
            
            var value = stringBuilder.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            var decimalValue = decimal.Parse(value, CultureInfo.InvariantCulture);
            var symbol = new Symbol(SymbolType.Constant, startingPosition, value.Length, decimalValue);
            return symbol;
        }

        private static bool ParseCurrentCharForConstantWhenNextAvailable(string expression, int currentPosition, out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            var nextChar = expression[currentPosition + 1];
            parsedChar = currentChar;

            switch (currentChar)
            {
                // something like:
                // +1
                // -1
                // +.5
                // -.5
                case '+' or '-' when nextChar == '.' || IsNumber(nextChar):
                // something like:
                // .5
                case '.' when IsNumber(nextChar):
                    return true;
                default:
                    return ParseCurrentCharForConstantWhenNextUnavailable(expression, currentPosition, out parsedChar);
            }
        }

        private static bool ParseCurrentCharForConstantWhenNextUnavailable(string expression, int currentPosition, out char parsedChar)
        {
            var currentChar = expression[currentPosition];
            parsedChar = currentChar;

            return IsNumber(currentChar);
        }

        private static bool IsNumber(char @char)
        {
            return char.IsDigit(@char);
        }
    }
}