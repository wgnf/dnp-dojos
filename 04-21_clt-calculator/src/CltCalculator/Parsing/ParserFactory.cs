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
                new OperationParserPart(),
                new ParenthesisParserPart()
            };
            return new Parser(parts);
        }
    }
}