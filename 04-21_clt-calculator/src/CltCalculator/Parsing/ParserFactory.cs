using CltCalculator.Contracts.Parsing;
using CltCalculator.Parsing.Parts;

namespace CltCalculator.Parsing
{
    public static class ParserFactory
    {
        public static IParser GetParser()
        {
            var parts = new IParserPart[]
            {
                new ConstantParserPart(),
                new OperationParserPart()
            };
            return new Parser(parts);
        }
    }
}