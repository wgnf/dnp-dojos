using System;
using System.Collections.Generic;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Exceptions;
using CltCalculator.Models;

namespace CltCalculator.Parsing
{
    public class Parser : IParser
    {
        private readonly IEnumerable<IParserPart> _parserParts;

        public Parser(IEnumerable<IParserPart> parserParts)
        {
            _parserParts = parserParts ?? throw new ArgumentNullException(nameof(parserParts));
        }

        public IEnumerable<Symbol> Parse(string expression)
        {
            if (string.IsNullOrEmpty(expression))
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
            var resultFound = false;
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var parserPart in _parserParts)
                resultFound = resultFound || parserPart.TryParse(expression, currentPosition, out symbol);

            if (symbol != null)
                return symbol;
            
            throw new ParseUnexpectedSymbolException(expression[currentPosition], currentPosition);
        }
    }
}