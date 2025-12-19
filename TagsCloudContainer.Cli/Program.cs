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
        builder.RegisterType<DocxFileReader>().As<IFileReader>();
        
        builder.Register(c =>
        {
            var readers = c.Resolve<IEnumerable<IFileReader>>();
            return new FileReaderFactory(readers);
        }).AsSelf().SingleInstance();
        
        builder.Register<IBoringWordsFilter>(c =>
        {
            var opts = c.Resolve<Options>();
            if (!string.IsNullOrWhiteSpace(opts.FilterFilePath))
            {
                var factory = c.Resolve<FileReaderFactory>();
                return new FileBoringWordsFilter(factory, opts.FilterFilePath);
            }
            return new LengthBoringWordsFilter();
        }).As<IBoringWordsFilter>();

        builder.Register(c =>
        {
            var readerFactory = c.Resolve<FileReaderFactory>();
            var filter = c.Resolve<IBoringWordsFilter>();
            return new WordsProcessor(readerFactory, filter);
        }).AsSelf();

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