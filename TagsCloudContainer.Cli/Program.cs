using CommandLine;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Cli;

class Program
{
    public class Options
    {
        [Option('i', "inputFile", Required = true, HelpText = "Path to the file with words")]
        public string FilePath { get; set; }
        
        [Option('f', "filterFile", Required = false, HelpText = "Path to the file with boring words")]
        public string? FilterFilePath { get; set; }

        [Option('o', "outputFile", Required = false, HelpText = "Path to the output file")]
        public string? OutputFilePath { get; set; } = System.Environment.CurrentDirectory;
    }
    
    static void Main(string[] args)
    {
        var reader = new BasicFileReader();
        var filter = new DummyWordFilter();
        var content = reader.GetWords("C:\\Users\\shept\\Downloads\\words.txt");
        var filtered = filter.ExcludeBoringWords(content);
        var count = WordCounter.CountWords(filtered);
        foreach (var pair in count)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
}