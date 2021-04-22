using System.Collections.Generic;
using CltCalculator.Models;

namespace CltCalculator.Contracts.Parsing
{
    public interface IParser
    {
        IEnumerable<Symbol> Parse(string expression);
    }
}