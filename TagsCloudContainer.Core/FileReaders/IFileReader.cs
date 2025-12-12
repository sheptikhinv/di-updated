namespace TagsCloudContainer.Core.FileReaders;

public interface IFileReader
{
    List<string> GetWords(string filePath);
}