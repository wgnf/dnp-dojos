using System;
using System.Linq;
using CltCalculator.Contracts.Parsing;
using CltCalculator.Exceptions;
using CltCalculator.Models;
using CltCalculator.Parsing;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace CltCalculator.Tests.Parsing
{
    public class ParserTests
    {
        [Fact]
        public void Should_Fail_On_Parse_With_Empty_Expression()
        {
            var sut = new Parser(Enumerable.Empty<IParserPart>());

            Assert.Throws<ArgumentException>(() => sut.Parse(null));
            Assert.Throws<ArgumentException>(() => sut.Parse(string.Empty));
        }

        [Fact]
        public void Should_Work_With_Parser_Parts()
        {
            var parserPart = new Mock<IParserPart>();
            var sut = new Parser(new[] {parserPart.Object});

            const string expression = "something";
            var expectedSymbol = new Symbol(SymbolType.Constant, 1);

            parserPart
                .Setup(pp =>
                    pp.TryParse(expression, It.IsAny<int>(), out expectedSymbol))
                .Returns(true);

            var symbols = sut.Parse(expression);

            var firstSymbol = symbols.FirstOrDefault();
            if (firstSymbol == null)
                throw new XunitException();

            Assert.Equal(expectedSymbol, firstSymbol);
        }

        [Fact]
        public void Should_Fail_When_Could_Not_Parse()
        {
            var parserPart = new Mock<IParserPart>();
            var sut = new Parser(new[] {parserPart.Object});

            const string expression = "something";

            Symbol symbol;
            parserPart
                .Setup(pp =>
                    pp.TryParse(expression, It.IsAny<int>(), out symbol))
                .Returns(false);

            Assert.Throws<ParseUnexpectedSymbolException>(() => sut.Parse(expression));
        }

        [Fact]
        public void Should_Ignore_Whitespaces()
        {
            var parserPart = new Mock<IParserPart>();
            var sut = new Parser(new[] {parserPart.Object});

            sut.Parse("     ");

            Symbol symbol;
            parserPart
                .Verify(
                    pp => pp.TryParse(It.IsAny<string>(), It.IsAny<int>(),
                        out symbol), Times.Never);
        }
    }
}