namespace TagsCloudContainer.Core.FileReaders;

public class TxtFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".txt"];

    public bool CanReadFile(string extension)
    {
        return !extension.StartsWith('.')
            ? throw new ArgumentException($"Invalid file extension: {extension}")
            : SupportedExtensions.Contains(extension);
    }

    public List<string> ReadWords(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            return lines.ToList();
        }
        catch (IOException e)
        {
            Console.WriteLine($"File at {filePath} could not be read: {e.Message}");
            throw;
        }
    }
}