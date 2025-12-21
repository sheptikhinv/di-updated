using Autofac;
using TagsCloudContainer.Core.CloudRenderers;
using TagsCloudContainer.Core.LayoutBuilders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Cli;

public class Client
{
    public void Run(Options options)
    {
        var container = Startup.ConfigureServices(options);

        using var scope = container.BeginLifetimeScope();
        var wordProcessor = scope.Resolve<WordsProcessor>();
        var layoutBuilder = scope.Resolve<ILayoutBuilder>();
        var cloudRenderer = scope.Resolve<ICloudRenderer>();

        var count = wordProcessor.ProcessWords(options.FilePath);
        var layout = layoutBuilder.BuildLayout(count);
        var bitmap = cloudRenderer.RenderCloud(layout);
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