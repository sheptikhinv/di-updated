using Autofac;
using TagsCloudContainer.Core.CloudRenderers;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.LayoutBuilders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordProcessing;

namespace TagsCloudContainer.Cli;

public class Client
{
    public void Run(Options options)
    {
        var container = Startup.ConfigureServices(options);

        using var scope = container.BeginLifetimeScope();
        var wordProcessor = scope.Resolve<WordsProcessor>();
        var fileReaderFactory = scope.Resolve<FileReaderFactory>();
        var layoutBuilder = scope.Resolve<ILayoutBuilder>();
        var cloudRenderer = scope.Resolve<ICloudRenderer>();
        var visualizationOptions = scope.Resolve<VisualizationOptions>();

        var rawWords = fileReaderFactory.GetReader(options.FilePath).ReadWords(options.FilePath);
        var count = wordProcessor.ReadProcessAndCountWords(rawWords);
        var layout = layoutBuilder.BuildLayout(count);
        var bitmap = cloudRenderer.RenderCloud(layout, visualizationOptions);
        var output = options.OutputFilePath ??
                     Path.Combine(Environment.CurrentDirectory, $"TagsCloud_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.png");

        FileSaver.SaveFile(bitmap, output);

        foreach (var pair in count.OrderByDescending(pair => pair.Value))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.WriteLine($"Visualization saved to file {options.OutputFilePath}");
    }
}