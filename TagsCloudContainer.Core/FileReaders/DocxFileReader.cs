using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using Path = System.IO.Path;

namespace TagsCloudContainer.Core.FileReaders;

public class DocxFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".docx"];

    public bool CanReadFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return SupportedExtensions.Contains(extension);
    }

    public List<string> GetWords(string filePath)
    {
        try
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            if (body == null)
                return [];

            return body.Descendants<Paragraph>()
                .Select(p => p.InnerText.Trim().ToLower())
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