using Autofac;
using TagsCloudContainer.Core.DependencyInjection;

namespace TagsCloudContainer.Cli;

public class Startup
{
    public static IContainer ConfigureServices(Options options)
    {
        var builder = BuildContainer(options);
        return builder.Build();
    }

    private static ContainerBuilder BuildContainer(Options options)
    {
        return new ContainerBuilder()
            .AddVisualizationOptions(options.BackgroundColor, options.TextColor, options.FontSize,
                options.OutputWidthPx, options.OutputHeightPx, options.FontFamily)
            .AddFileReaders()
            .AddWordsFilter(options.FilterFilePath)
            .AddWordsProcessor()
            .AddCoordinateGenerators(options.OutputWidthPx ?? 100, options.OutputHeightPx ?? 100, options.AngleStepRadians)
            .AddLayoutBuilder()
            .AddCloudRenderer();
    }
}