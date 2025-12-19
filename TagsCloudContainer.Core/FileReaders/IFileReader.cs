namespace TagsCloudContainer.Core.FileReaders;

public interface IFileReader
{
    bool CanReadFile(string filePath);
    List<string> GetWords(string filePath);
}