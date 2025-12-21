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
            .AddVisualizationOptions(options.BackgroundColor, options.TextColor, options.FontSize, options.OutputWidthPx)
            .AddFileReaders()
            .AddWordsFilter(options.FilterFilePath)
            .AddWordsProcessor()
            .AddCoordinateGenerators(options.OutputWidthPx, options.OutputHeightPx, options.AngleStepRadians)
            .AddVisualizators();
    }
}