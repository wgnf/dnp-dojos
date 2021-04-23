using System;
using System.Collections.Generic;
using System.Linq;
using CltCalculator.Contracts.Calculation;
using CltCalculator.Models;

namespace CltCalculator.Calculation
{
    public class Calculator : ICalculator
    {
        public decimal Calculate(IEnumerable<Symbol> symbols)
        {
            if (symbols == null) throw new ArgumentNullException(nameof(symbols));

            var symbolsList = symbols.ToList();
            if (!symbolsList.Any()) throw new ArgumentException("Need symbols to calculate", nameof(symbols));

            var localWorkingSymbols = new List<Symbol>(symbolsList);
            var result = CalculateResultFromSymbols(localWorkingSymbols);
            return result;
        }

        private static decimal CalculateResultFromSymbols(IList<Symbol> symbols)
        {
            if (TryConsiderOnlyOneConstantSymbol(symbols, out var result))
                return result;

            if (symbols.Any(s => s.Type == SymbolType.OpenParenthesis))
            {
                CalculateResultFromSymbolsInParentheses(symbols);
                // ReSharper disable once TailRecursiveCall
                return CalculateResultFromSymbols(symbols);
            }
            
            FindAndCalculateSymbol(symbols);
            
            // ReSharper disable once TailRecursiveCall
            return CalculateResultFromSymbols(symbols);
        }

        private static bool TryConsiderOnlyOneConstantSymbol(ICollection<Symbol> symbols, out decimal result)
        {
            result = 0;
            if (symbols.Count != 1 || symbols.Any(s => s.Type != SymbolType.Constant))
                return false;

            var firstAndOnlySymbol = symbols.FirstOrDefault(s => s.Value.HasValue);
            if (firstAndOnlySymbol?.Value == null)
                throw new InvalidOperationException("Hmm. Something went wrong here!");

            result = firstAndOnlySymbol.Value.Value;
            return true;
        }

        private static void CalculateResultFromSymbolsInParentheses(IList<Symbol> symbols)
        {
            if (!TryGetLastOpenParenthesis(symbols, out var open))
                throw new InvalidOperationException("Did not find an open parenthesis but one was expected");

            if (!TryGetClosingParenthesisForOpen(symbols, open, out var close))
                throw new InvalidOperationException(
                    $"Did not find a matching closing parenthesis for the open parenthesis at position {open.ZeroBasedPositionInExpression + 1}");

            var workingList = GetWorkingListForParentheses(symbols, open, close);
            var result = CalculateResultFromSymbols(workingList.ToList());
            
            ReplaceParenthesesWithResult(symbols, workingList, result, open, close);
        }

        private static bool TryGetLastOpenParenthesis(IEnumerable<Symbol> symbols, out Symbol open)
        {
            open = symbols.LastOrDefault(s => s.Type == SymbolType.OpenParenthesis);
            return open != null;
        }

        private static bool TryGetClosingParenthesisForOpen(IList<Symbol> symbols, Symbol open, out Symbol close)
        {
            var indexOfOpen = symbols.IndexOf(open);
            var subLiftOfOpen = symbols.TakeLast(symbols.Count - (indexOfOpen + 1));
            close = subLiftOfOpen.FirstOrDefault(s => s.Type == SymbolType.CloseParenthesis);
            return close != null;
        }

        private static IList<Symbol> GetWorkingListForParentheses(IList<Symbol> symbols, Symbol open, Symbol close)
        {
            var indexOfOpen = symbols.IndexOf(open);
            var indexOfClose = symbols.IndexOf(close);
            var workingList = symbols.Skip(indexOfOpen + 1).Take(indexOfClose - (indexOfOpen + 1)).ToList();

            return workingList;
        }

        private static void ReplaceParenthesesWithResult(
            IList<Symbol> symbols,
            IEnumerable<Symbol> workingList,
            decimal result, 
            Symbol open,
            Symbol close)
        {
            var resultConstant = new Symbol(SymbolType.Constant, open.ZeroBasedPositionInExpression, Value: result);
            
            foreach (var processedItem in workingList.Concat(new[] {close}))
                symbols.Remove(processedItem);

            var indexOfOpen = symbols.IndexOf(open);
            symbols[indexOfOpen] = resultConstant;
        }
        
        private static void FindAndCalculateSymbol(IList<Symbol> symbols)
        {
            var found = TryFindOperation(symbols, out var foundOperation);

            if (!found)
                throw new InvalidOperationException(
                    "Did not find an operation even though we're not finished calculating yet");

            CalculateSymbol(symbols, foundOperation);
        }

        private static bool TryFindOperation(IList<Symbol> symbols, out Symbol operation)
        {
            var found = TryFindExponent(symbols, out operation);
            found = found || TryFindMultiplication(symbols, out operation);
            found = found || TryFindDivision(symbols, out operation);
            found = found || TryFindAddition(symbols, out operation);
            found = found || TryFindSubtraction(symbols, out operation);

            return found;
        }

        private static bool TryFindExponent(IEnumerable<Symbol> symbols, out Symbol exponentSymbol)
        {
            exponentSymbol = symbols.FirstOrDefault(s => s.Type == SymbolType.Exponent);
            return exponentSymbol != null;
        }

        private static bool TryFindMultiplication(IEnumerable<Symbol> symbols, out Symbol multiplicationSymbol)
        {
            multiplicationSymbol = symbols.FirstOrDefault(s => s.Type == SymbolType.Multiplication);
            return multiplicationSymbol != null;
        }

        private static bool TryFindDivision(IEnumerable<Symbol> symbols, out Symbol divisionSymbol)
        {
            divisionSymbol = symbols.FirstOrDefault(s => s.Type == SymbolType.Division);
            return divisionSymbol != null;
        }

        private static bool TryFindAddition(IEnumerable<Symbol> symbols, out Symbol additionSymbol)
        {
            additionSymbol = symbols.FirstOrDefault(s => s.Type == SymbolType.Addition);
            return additionSymbol != null;
        }

        private static bool TryFindSubtraction(IEnumerable<Symbol> symbols, out Symbol subtractionSymbol)
        {
            subtractionSymbol = symbols.FirstOrDefault(s => s.Type == SymbolType.Subtraction);
            return subtractionSymbol != null;
        }

        private static void CalculateSymbol(IList<Symbol> symbols, Symbol operation)
        {
            var (left, right) = GetConstantsForOperation(symbols, operation);
            var result = CalculateOperation(operation, left, right);
            ReplaceWithResult(symbols, result, left, operation, right);
        }

        private static (Symbol, Symbol) GetConstantsForOperation(IList<Symbol> symbols, Symbol operation)
        {
            var index = symbols.IndexOf(operation);
            if (!TryGetConstantLeftOf(index, symbols, out var left))
                throw new InvalidOperationException(
                    $"Could not find value left of {operation.Type} at position {operation.ZeroBasedPositionInExpression + 1}");
            if (!TryGetConstantRightOf(index, symbols, out var right))
                throw new InvalidOperationException(
                    $"Could not find value right of {operation.Type} at position {operation.ZeroBasedPositionInExpression + 1}");

            return (left, right);
        }

        private static bool TryGetConstantLeftOf(int index, IList<Symbol> symbols, out Symbol constantSymbol)
        {
            constantSymbol = null;
            try
            {
                constantSymbol = symbols[index - 1];
                return constantSymbol.Type == SymbolType.Constant;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryGetConstantRightOf(int index, IList<Symbol> symbols, out Symbol constantSymbol)
        {
            constantSymbol = null;
            try
            {
                constantSymbol = symbols[index + 1];
                return constantSymbol.Type == SymbolType.Constant;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static decimal CalculateOperation(Symbol operation, Symbol left, Symbol right)
        {
            if (!left.Value.HasValue)
                throw new InvalidOperationException("Left value has no value");
            if (!right.Value.HasValue)
                throw new InvalidOperationException("Right value has no value");

            var valueLeft = left.Value.Value;
            var valueRight = right.Value.Value;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return operation.Type switch
            {
                SymbolType.Addition => valueLeft + valueRight,
                SymbolType.Subtraction => valueLeft - valueRight,
                SymbolType.Multiplication => valueLeft * valueRight,
                SymbolType.Division => valueLeft / valueRight,
                SymbolType.Exponent => Convert.ToDecimal(Math.Pow(Convert.ToDouble(valueLeft), Convert.ToDouble(valueRight))),
                _ => throw new ArgumentOutOfRangeException(
                    $"{nameof(operation)}.{nameof(operation.Type)}",
                    $"Did not expect {operation.Type} in {nameof(CalculateOperation)}")
            };
        }

        private static void ReplaceWithResult(
            IList<Symbol> symbols,
            decimal result,
            Symbol left,
            Symbol operation,
            Symbol right)
        {
            symbols.Remove(left);
            symbols.Remove(right);

            var resultSymbol = new Symbol(SymbolType.Constant, operation.ZeroBasedPositionInExpression, Value: result);
            var indexOfOperation = symbols.IndexOf(operation);
            symbols[indexOfOperation] = resultSymbol;
        }
    }
}