namespace CltCalculator.Models
{
    public record Symbol(SymbolType Type, int ZeroBasedPositionInExpression, int Length = 1, decimal? Value = null);
}