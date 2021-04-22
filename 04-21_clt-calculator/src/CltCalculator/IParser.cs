using System.Collections.Generic;

namespace CltCalculator
{
    public interface IParser
    {
        IEnumerable<Symbol> Parse(string expression);
    }
}