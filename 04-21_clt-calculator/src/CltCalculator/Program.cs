using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Globalization;
using System.Threading.Tasks;

namespace CltCalculator
{
    public static class Program
    {
        public static Task<int> Main(string[] arguments)
        {
            // c.f.:
            // https://github.com/dotnet/command-line-api/blob/master/docs/Your-first-app-with-System-CommandLine.md
            var rootCommand = new RootCommand
            {
                new Argument<string>("expression", "The expression to calculate"),
                new Option<bool>("--diagnostic", () => false, "Whether or not diagnostic information should be printed")
            };
            
            rootCommand.Handler = CommandHandler.Create<string, bool, IConsole>(HandleExpressionCalculation);

            return rootCommand.InvokeAsync(arguments);
        }

        private static void HandleExpressionCalculation(string expression, bool diagnostic, IConsole console)
        {
            try
            {
                var expressionCalculator = ExpressionCalculatorFactory.GetExpressionCalculator();
                var result = expressionCalculator.Calculate(expression);
                console.Out.WriteLine($"Result: {result.ToString(CultureInfo.InvariantCulture)}");
            }
            catch (Exception e)
            {
                console.Error.WriteLine($"Error: {e.Message}");
                if (diagnostic)
                    console.Error.WriteLine($"\n\n{e.StackTrace}");
            }
        }
    }
}
