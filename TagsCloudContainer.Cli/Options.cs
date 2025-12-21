using CommandLine;

namespace TagsCloudContainer.Cli;

public class Options
{
    [Option('i', "inputFile", Required = true, HelpText = "Path to the file with words to be processed")]
    public string FilePath { get; set; }

    [Option('f', "filterFile", Required = false, HelpText = "Path to the file with boring words")]
    public string? FilterFilePath { get; set; }

    [Option('o', "outputFile", Required = false, HelpText = "Path to the output file")]
    public string? OutputFilePath { get; set; }

    [Option('a', "angleStep", Required = false, HelpText = "Angle step for the spiral algorithm")]
    public double AngleStepRadians { get; set; } = 0.1;

    [Option('w', "outputWidth", Required = false, HelpText = "Size of the image")]
    public int? OutputWidthPx { get; set; }

    [Option('l', "outputHeight", Required = false, HelpText = "Size of the image")]
    public int? OutputHeightPx { get; set; }

    [Option('g', "fontFamily", Required = false, HelpText = "Font family of the words")]
    public string? FontFamily { get; set; } = "Arial";

    [Option('t', "fontSize", Required = false, HelpText = "Font size of the words")]
    public float FontSize { get; set; } = 12;

    [Option('b', "backgroundColor", Required = false, HelpText = "Background color of the image")]
    public string BackgroundColor { get; set; } = "white";

    [Option('c', "textColor", Required = false, HelpText = "Text color of the image")]
    public string TextColor { get; set; } = "black";
}