using System;
using System.Linq;
using CltCalculator.Calculation;
using CltCalculator.Models;
using FluentAssertions;
using Xunit;

namespace CltCalculator.Tests.Calculation
{
    public class CalculatorTests
    {
        [Fact]
        public void Should_Fail_For_Invalid_Parameter()
        {
            var sut = new Calculator();

            Assert.Throws<ArgumentNullException>(() => sut.Calculate(null));
            Assert.Throws<ArgumentException>(() => sut.Calculate(Enumerable.Empty<Symbol>()));
        }

        [Fact]
        public void Should_Fail_For_Invalid_Combination_Of_Symbols()
        {
            var sut = new Calculator();

            var twoOperations = new[]
            {
                new Symbol(SymbolType.Addition, 0),
                new Symbol(SymbolType.Addition, 1)
            };

            Assert.Throws<InvalidOperationException>(() => sut.Calculate(twoOperations));

            var twoConstants = new[]
            {
                new Symbol(SymbolType.Constant, 0, 1, 42),
                new Symbol(SymbolType.Constant, 1, 1, 1337)
            };

            Assert.Throws<InvalidOperationException>(() => sut.Calculate(twoConstants));
        }

        [Fact]
        public void Should_Calculate_For_Single_Constant()
        {
            var sut = new Calculator();

            const decimal expectedValue = 20;
            var symbols = new[]
            {
                new Symbol(SymbolType.Constant, 0, 1, 20)
            };

            var result = sut.Calculate(symbols);
            result
                .Should()
                .Be(expectedValue);
        }

        [Fact]
        public void Should_Calculate_One_Plus_One()
        {
            var sut = new Calculator();

            var symbols = new[]
            {
                new Symbol(SymbolType.Constant, 0, 1, 1),
                new Symbol(SymbolType.Addition, 0),
                new Symbol(SymbolType.Constant, 0, 1, 1)
            };

            var result = sut.Calculate(symbols);
            result
                .Should()
                .Be(2);
        }

        [Fact]
        public void Should_Respect_Point_Before_Line_Calculation()
        {
            var sut = new Calculator();

            var symbols = new[]
            {
                new Symbol(SymbolType.Constant, 0, 1, 1),
                new Symbol(SymbolType.Addition, 0),
                new Symbol(SymbolType.Constant, 0, 1, 2),
                new Symbol(SymbolType.Multiplication, 0),
                new Symbol(SymbolType.Constant, 0, 1, 3),
                new Symbol(SymbolType.Division, 0),
                new Symbol(SymbolType.Constant, 0, 1, 4),
                new Symbol(SymbolType.Subtraction, 0),
                new Symbol(SymbolType.Constant, 0, 1, 5)
            };

            var result = sut.Calculate(symbols);
            result
                .Should()
                .Be((decimal)-2.5);
        }
    }
}