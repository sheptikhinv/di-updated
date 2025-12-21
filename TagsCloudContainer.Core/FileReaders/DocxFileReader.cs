using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;

namespace TagsCloudContainer.Core.FileReaders;

public class DocxFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".docx"];

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
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            if (body == null)
                return [];

            return body.Descendants<Paragraph>()
                .Select(p => p.InnerText.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
        }
        catch (IOException e)
        {
            Console.WriteLine($"File at {filePath} could not be read: {e.Message}");
            throw;
        }
    }
}