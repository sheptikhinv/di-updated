using Autofac;
using CommandLine;
using TagsCloudContainer.Core.DependencyInjection;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Cli;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(Run)
            .WithNotParsed(HandleParseErrors);
    }

    private static void Run(Options options)
    {
        var container = BuildContainer(options);

        using var scope = container.BeginLifetimeScope();
        var wordProcessor = scope.Resolve<WordsProcessor>();
        var visualizator = scope.Resolve<IVisualizator>();

        var count = wordProcessor.ProcessWords(options.FilePath);
        visualizator.DrawWordsToFile(count, options.OutputFilePath);

        foreach (var pair in count.OrderByDescending(pair => pair.Value))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.WriteLine($"Visualization saved to file {options.OutputFilePath}");
    }

    private static IContainer BuildContainer(Options options)
    {
        var builder = new ContainerBuilder()
            .AddVisualizationOptions(options.BackgroundColor, options.TextColor, options.FontSize, options.ImageSize)
            .AddFileReaders()
            .AddWordsFilter(options.FilterFilePath)
            .AddWordsProcessor()
            .AddCoordinateGenerators(options.ImageSize, options.AngleStep)
            .AddVisualizators();
        
        return builder.Build();
    }

    private static void HandleParseErrors(IEnumerable<Error> errors)
    {
        Console.WriteLine("Error while parsing arguments:");
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }
    }
}