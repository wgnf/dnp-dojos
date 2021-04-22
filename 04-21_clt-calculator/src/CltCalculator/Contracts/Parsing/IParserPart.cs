using CltCalculator.Models;

namespace CltCalculator.Contracts.Parsing
{
    public interface IParserPart
    {
        bool TryParse(string expression, int currentPosition, out Symbol symbol);
    }
}