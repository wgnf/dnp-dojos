using System;

namespace CltCalculator.Exceptions
{
    public class ParseUnexpectedSymbolException : Exception
    {
        public ParseUnexpectedSymbolException(char symbol, int zeroBasedPosition, bool operationExpected) : base(
            $"Could not parse: Unexpected symbol '{symbol}' at position {zeroBasedPosition + 1} " +
            $"- {(operationExpected ? "Expected an Operation ('+', '-', '*', '/')" : "Expected no operation")}")
        {
        }
    }
}