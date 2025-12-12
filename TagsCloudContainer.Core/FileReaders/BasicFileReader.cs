namespace TagsCloudContainer.Core.FileReaders;

public class BasicFileReader : IFileReader
{
    public List<string> GetWords(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            return lines.Select(w => w.ToLower()).ToList();
        }
        catch (IOException e)
        {
            Console.WriteLine($"File at {filePath} could not be read: {e.Message}");
            throw;
        }
    }
}