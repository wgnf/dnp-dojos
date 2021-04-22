using System;
using CltCalculator.Models;
using CltCalculator.Parsing.Parts;
using Xunit;

namespace CltCalculator.Tests.Parsing.Parts
{
    public class OperationParserPartTests
    {
        [Fact]
        public void Should_Fail_On_Empty_Expression()
        {
            var sut = new OperationParserPart();
            
            Assert.Throws<ArgumentException>(() => sut.TryParse(null, 1, out _));
            Assert.Throws<ArgumentException>(() => sut.TryParse(string.Empty, 1, out _));
            Assert.Throws<ArgumentException>(() => sut.TryParse(" ", 1, out _));
        }

        [Theory]
        [InlineData("+", SymbolType.Addition)]
        [InlineData("-", SymbolType.Subtraction)]
        [InlineData("*", SymbolType.Multiplication)]
        [InlineData("/", SymbolType.Division)]
        public void Should_Parse_Operations(string expression, SymbolType expectedSymbolType)
        {
            var sut = new OperationParserPart();
            var couldParse = sut.TryParse(expression, 0, out var symbol);
            
            Assert.True(couldParse);
            Assert.Equal(expectedSymbolType, symbol.Type);
        }
    }
}