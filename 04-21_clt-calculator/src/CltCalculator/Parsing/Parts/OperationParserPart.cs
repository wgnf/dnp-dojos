using System;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Models;

namespace CltCalculator.Parsing.Parts
{
    public class OperationParserPart : IParserPart
    {
        public bool ParsesOperations => true;

        public bool TryParse(string expression, int currentPosition, out Symbol symbol)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(expression));
            
            var currentChar = expression[currentPosition];
            symbol = null;

            if (!TryGetSymbolTypeFromChar(currentChar, out var symbolType)) return false;

            symbol = new Symbol(symbolType, currentPosition);
            return true;
        }

        private static bool TryGetSymbolTypeFromChar(char currentChar, out SymbolType symbolType)
        {
            symbolType = SymbolType.Addition;
            switch (currentChar)
            {
                case '+':
                    symbolType = SymbolType.Addition;
                    break;
                case '-':
                    symbolType = SymbolType.Subtraction;
                    break;
                case '*':
                    symbolType = SymbolType.Multiplication;
                    break;
                case '/':
                    symbolType = SymbolType.Division;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}