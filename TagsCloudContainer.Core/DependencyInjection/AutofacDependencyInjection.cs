using System.Drawing;
using Autofac;
using TagsCloudContainer.Core.CloudRenderers;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.LayoutBuilders;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordFilters;
using TagsCloudContainer.Core.WordProcessing;
using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.DependencyInjection;

public static class AutofacDependencyInjection
{
    public static ContainerBuilder AddVisualizationOptions(this ContainerBuilder builder, string backgroundColor,
        string textColor, float fontSize, int? imageWidthPx, int? imageHeightPx, string? fontFamily)
    {
        var visualizationOptions = new VisualizationOptions
        {
            BackgroundColor = Color.FromName(backgroundColor).IsKnownColor
                ? Color.FromName(backgroundColor)
                : Color.Black,
            FontColor = Color.FromName(textColor).IsKnownColor ? Color.FromName(textColor) : null,
            FontSize = fontSize,
            ImageWidthPx = imageWidthPx,
            ImageHeightPx = imageHeightPx,
            FontFamily = fontFamily
        };

        builder.RegisterInstance(visualizationOptions).AsSelf();

        return builder;
    }

    public static ContainerBuilder AddVisualizationOptions(this ContainerBuilder builder, VisualizationOptions options)
    {
        builder.RegisterInstance(options).AsSelf();

        return builder;
    }

    public static ContainerBuilder AddFileReaders(this ContainerBuilder builder)
    {
        builder.RegisterType<TxtFileReader>().As<IFileReader>();
        builder.RegisterType<DocxFileReader>().As<IFileReader>();

        builder.Register(c =>
        {
            var readers = c.Resolve<IEnumerable<IFileReader>>();
            return new FileReaderFactory(readers);
        }).AsSelf().SingleInstance();

        return builder;
    }

    public static ContainerBuilder AddWordsFilter(this ContainerBuilder builder, string? filterFilePath)
    {
        builder.Register<IBoringWordsFilter>(c =>
        {
            if (string.IsNullOrWhiteSpace(filterFilePath))
                return new LengthBoringWordsFilter();
            var factory = c.Resolve<FileReaderFactory>();
            var words = factory.GetReader(filterFilePath).ReadWords(filterFilePath);
            return new SpecifiedBoringWordsFilter(words);
        }).As<IBoringWordsFilter>();

        return builder;
    }

    public static ContainerBuilder AddWordsProcessor(this ContainerBuilder builder)
    {
        builder.RegisterType<LowerizeWordProcessingRule>().As<IWordProcessingRule>();
        builder.Register(c =>
        {
            var rules = c.Resolve<IEnumerable<IWordProcessingRule>>();
            var filter = c.Resolve<IBoringWordsFilter>();
            return new WordsProcessor(filter, rules);
        }).AsSelf();

        return builder;
    }

    public static ContainerBuilder AddCoordinateGenerators(this ContainerBuilder builder, int imageWidth = 2048,
        int imageHeight = 2048,
        double angleStep = 0.1f)
    {
        builder.Register(c =>
        {
            var center = new Point(imageWidth / 2, imageHeight / 2);
            return new SpiralCoordinateGenerator(center, angleStep);
        }).As<ICoordinateGenerator>();

        return builder;
    }

    public static ContainerBuilder AddCloudRenderer(this ContainerBuilder builder)
    {
        builder.RegisterType<BasicCloudRenderer>().As<ICloudRenderer>();

        return builder;
    }

    public static ContainerBuilder AddLayoutBuilder(this ContainerBuilder builder)
    {
        builder.RegisterType<BasicLayoutBuilder>().As<ILayoutBuilder>();

        return builder;
    }
}