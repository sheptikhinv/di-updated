namespace TagsCloudContainer.Core.FileReaders;

public interface IFileReader
{
    bool CanReadFile(string extension);
    List<string> ReadWords(string filePath);
}