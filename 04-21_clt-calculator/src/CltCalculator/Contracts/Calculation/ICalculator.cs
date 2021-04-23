using System.Collections.Generic;
using CltCalculator.Models;

namespace CltCalculator.Contracts.Calculation
{
    public interface ICalculator
    {
        decimal Calculate(IEnumerable<Symbol> symbols);
    }
}