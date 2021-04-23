using CltCalculator.Models;

namespace CltCalculator.Contracts.Parsing
{
    public interface IParserPart
    {
        bool? ParsesOperations { get; }
        bool TryParse(string expression, int currentPosition, out Symbol symbol);
    }
}