using System;

namespace CltCalculator.Exceptions
{
    public class ParseUnexpectedSymbolException : Exception
    {
        public ParseUnexpectedSymbolException(char symbol, int position) : base(
            $"Could not parse: Unexpected symbol '{symbol}' at position {position}")
        {
        }
    }
}