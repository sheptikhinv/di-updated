using System.Drawing;
using Autofac;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.DependencyInjection;

public static class AutofacDependencyInjection
{
    public static ContainerBuilder AddVisualizationOptions(this ContainerBuilder builder, string backgroundColor,
        string textColor, float fontSize, int imageSize)
    {
        var visualizationOptions = new VisualizationOptions
        {
            BackgroundColor = Color.FromName(backgroundColor).IsKnownColor
                ? Color.FromName(backgroundColor)
                : Color.Black,
            FontColor = Color.FromName(textColor).IsKnownColor ? Color.FromName(textColor) : null,
            FontSize = fontSize,
            ImageWidthPx = imageSize
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
        builder.RegisterType<BasicFileReader>().As<IFileReader>();
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
            return new FileBoringWordsFilter(factory, filterFilePath);
        }).As<IBoringWordsFilter>();

        return builder;
    }

    public static ContainerBuilder AddWordsProcessor(this ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var readerFactory = c.Resolve<FileReaderFactory>();
            var filter = c.Resolve<IBoringWordsFilter>();
            return new WordsProcessor(readerFactory, filter);
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

    public static ContainerBuilder AddVisualizators(this ContainerBuilder builder)
    {
        builder.RegisterType<BasicVisualizator>().As<IVisualizator>();

        return builder;
    }
}