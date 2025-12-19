namespace TagsCloudContainer.Core.FileReaders;

public class FileReaderFactory
{
    private readonly IEnumerable<IFileReader> _readers;

    public FileReaderFactory(IEnumerable<IFileReader> readers)
    {
        _readers = readers;
    }

    public IFileReader GetReader(string filePath)
    {
        var reader = _readers.FirstOrDefault(r => r.CanReadFile(filePath));
        
        if (reader == null)
        {
            throw new NotSupportedException($"No reader found for file: {filePath}");
        }

        return reader;
    }

    public List<string> ReadFile(string filePath)
    {
        var reader = GetReader(filePath);
        return reader.GetWords(filePath);
    }
}