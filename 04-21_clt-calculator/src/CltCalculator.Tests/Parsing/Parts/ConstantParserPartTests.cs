using System;
using CltCalculator.Models;
using CltCalculator.Parsing.Parts;
using Xunit;

namespace CltCalculator.Tests.Parsing.Parts
{
    public class ConstantParserPartTests
    {
        [Fact]
        public void Should_Fail_On_Empty_Expression()
        {
            var sut = new ConstantParserPart();
            
            Assert.Throws<ArgumentException>(() => sut.TryParse(null, 1, out _));
            Assert.Throws<ArgumentException>(() => sut.TryParse(string.Empty, 1, out _));
            Assert.Throws<ArgumentException>(() => sut.TryParse(" ", 1, out _));
        }
        
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1.5", 1.5)]
        [InlineData("+1.5", 1.5)]
        [InlineData("-1.5", -1.5)]
        [InlineData(".5", .5)]
        [InlineData("+.5", .5)]
        [InlineData("-.5", -.5)]
        [InlineData("1337", 1337)]
        [InlineData("1337.42", 1337.42)]
        [InlineData(".555", .555)]
        [InlineData("+.555", .555)]
        [InlineData("-.555", -.555)]
        public void Should_Parse_Constants(string expression, decimal expectedValue)
        {
            var sut = new ConstantParserPart();
            var couldParse = sut.TryParse(expression, 0, out var symbol);
            
            Assert.True(couldParse);
            
            Assert.Equal(SymbolType.Constant, symbol.Type);
            Assert.Equal(expectedValue, symbol.Value);
        }
    }
}