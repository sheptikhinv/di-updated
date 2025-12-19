using NUnit.Framework;
using TagsCloudContainer.Core.FileReaders;

namespace TagsCloudContainer.Core.Tests.FileReaders;

[TestFixture]
public class FileReaderFactoryTests
{
    [Test]
    public void GetReader_ForTxt_ReturnsBasicFileReader()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });

        var reader = factory.GetReader("sample.txt");

        Assert.That(reader, Is.TypeOf<BasicFileReader>());
    }

    [Test]
    public void GetReader_UnsupportedExtension_Throws()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });

        Assert.Throws<NotSupportedException>(() => factory.GetReader("file.ihopetheresnofileextensionslikethis"));
    }

    [Test]
    public void ReadFile_ReadsAsWritten_FromTxt()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"readfile_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFile, new[] { "Hello", "WORLD" });
            var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });

            var lines = factory.ReadFile(tempFile);

            CollectionAssert.AreEqual(new[] { "Hello", "WORLD" }, lines);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
