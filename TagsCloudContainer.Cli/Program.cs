using System.Drawing;
using Autofac;
using CommandLine;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Cli;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options => Run(options))
            .WithNotParsed(errors => HandleParseErrors(errors));
    }

    private static void Run(Options options)
    {
        var container = BuildContainer(options);

        using var scope = container.BeginLifetimeScope();
        var reader = scope.Resolve<IFileReader>();
        var filter = scope.Resolve<IWordFilter>();
        var visualizator = scope.Resolve<IVisualizator>();

        var content = reader.GetWords(options.FilePath);
        var filtered = filter.ExcludeBoringWords(content);
        var count = WordCounter.CountWords(filtered);
        visualizator.DrawWordsToFile(count, options.OutputFilePath);

        foreach (var pair in count.OrderByDescending(pair => pair.Value))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.WriteLine($"Visualization saved to file {options.OutputFilePath}");
    }

    private static IContainer BuildContainer(Options options)
    {
        var builder = new ContainerBuilder();

        builder.RegisterInstance(options).AsSelf().SingleInstance();
        builder.Register(c =>
        {
            var opts = c.Resolve<Options>();
            var visualizationOptions = new VisualizationOptions
            {
                BackgroundColor = Color.FromName(opts.BackgroundColor).IsKnownColor
                    ? Color.FromName(opts.BackgroundColor)
                    : Color.Black,
                FontColor = Color.FromName(opts.TextColor).IsKnownColor ? Color.FromName(opts.TextColor) : null,
                FontSize = opts.FontSize,
                ImageSize = opts.ImageSize
            };
            return visualizationOptions;
        }).As<VisualizationOptions>();

        builder.RegisterType<BasicFileReader>().As<IFileReader>();
        
        builder.Register<IWordFilter>(c =>
        {
            var opts = c.Resolve<Options>();
            if (!string.IsNullOrWhiteSpace(opts.FilterFilePath))
            {
                var reader = c.Resolve<IFileReader>();
                return new FileWordFilter(reader, opts.FilterFilePath);
            }
            return new DummyWordFilter();
        }).As<IWordFilter>();

        builder.Register(c =>
        {
            var opts = c.Resolve<Options>();
            var center = new Point(opts.ImageSize / 2, opts.ImageSize / 2);
            return new SpiralCoordinateGenerator(center, opts.AngleStep);
        }).As<ICoordinateGenerator>();

        builder.RegisterType<BasicVisualizator>().As<IVisualizator>();

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