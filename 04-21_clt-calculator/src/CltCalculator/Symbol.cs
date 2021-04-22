namespace CltCalculator
{
    public record Symbol(SymbolType SymbolType, int PositionInExpression, int Length = 1, decimal? Value = null);
}