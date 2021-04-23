using System;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Models;

namespace CltCalculator.Parsing.Parts
{
    public class ParenthesisParserPart : IParserPart
    {
        public bool? ParsesOperations => null;
        
        public bool TryParse(string expression, int currentPosition, out Symbol symbol)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(expression));

            var currentChar = expression[currentPosition];
            var startingPosition = currentPosition;
            symbol = null;

            if (!TryGetSymbolTypeFromChar(currentChar, out var symbolType)) return false;

            symbol = new Symbol(symbolType, startingPosition);
            return true;
        }

        private static bool TryGetSymbolTypeFromChar(char currentChar, out SymbolType symbolType)
        {
            symbolType = SymbolType.OpenParenthesis;
            switch (currentChar)
            {
                case '(':
                    symbolType = SymbolType.OpenParenthesis;
                    break;
                case ')':
                    symbolType = SymbolType.CloseParenthesis;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}