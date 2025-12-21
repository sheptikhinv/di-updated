using CommandLine;

namespace TagsCloudContainer.Cli;

class Program
{
    static void Main(string[] args)
    {
        var client = new Client();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(client.Run)
            .WithNotParsed(HandleParseErrors);
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