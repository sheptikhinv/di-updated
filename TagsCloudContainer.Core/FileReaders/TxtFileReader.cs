namespace TagsCloudContainer.Core.FileReaders;

public class TxtFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".txt"];

    public bool CanReadFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return SupportedExtensions.Contains(extension);
    }

    public List<string> GetWords(string filePath)
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