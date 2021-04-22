using System;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace CltCalculator.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Should_Fail_On_Parse_With_Empty_Expression()
        {
            var sut = new Parser();

            Assert.Throws<ArgumentException>(() => sut.Parse(null));
            Assert.Throws<ArgumentException>(() => sut.Parse(string.Empty));
            Assert.Throws<ArgumentException>(() => sut.Parse(" "));
        }

        [Theory]
        [InlineData("   1   ", 1)]
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
            var sut = new Parser();
            var symbols = sut.Parse(expression);

            var firstSymbol = symbols.FirstOrDefault();
            if (firstSymbol == null)
                throw new XunitException();
            
            Assert.Equal(SymbolType.Constant, firstSymbol.SymbolType);
            Assert.Equal(expectedValue, firstSymbol.Value);
        }
    }
}