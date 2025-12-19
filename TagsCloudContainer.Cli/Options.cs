using CommandLine;
using System;

namespace TagsCloudContainer.Cli;

public class Options
{
    [Option('i', "inputFile", Required = true, HelpText = "Path to the file with words to be processed")]
    public string FilePath { get; set; }

    [Option('f', "filterFile", Required = false, HelpText = "Path to the file with boring words")]
    public string? FilterFilePath { get; set; }

    [Option('o', "outputFile", Required = false, HelpText = "Path to the output file")]
    public string OutputFilePath { get; set; } = Path.Combine(Environment.CurrentDirectory, $"TagsCloud_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.png");
    
    [Option('s', "imageSize", Required = false, HelpText = "Size of the image")]
    public int ImageSize { get; set; } = 300;
    
    [Option('b', "backgroundColor", Required = false, HelpText = "Background color of the image")]
    public string BackgroundColor { get; set; } = "white";
    
    [Option('c', "textColor", Required = false, HelpText = "Text color of the image")]
    public string TextColor { get; set; } = "black";
}