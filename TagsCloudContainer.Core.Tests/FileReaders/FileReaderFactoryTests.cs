using TagsCloudContainer.Core.FileReaders;

namespace TagsCloudContainer.Core.Tests.FileReaders;

[TestFixture]
public class FileReaderFactoryTests
{
    [Test]
    public void GetReader_ForTxt_ReturnsBasicFileReader_Test()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });

        var reader = factory.GetReader("sample.txt");

        Assert.That(reader, Is.TypeOf<TxtFileReader>());
    }

    [Test]
    public void GetReader_UnsupportedExtension_Throws_Test()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });

        Assert.Throws<NotSupportedException>(() => factory.GetReader("file.ihopetheresnofileextensionslikethis"));
    }

    [Test]
    public void ReadFile_ReadsAsWritten_FromTxt_Test()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"readfile_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFile, new[] { "Hello", "WORLD" });
            var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });

            var lines = factory.GetReader(tempFile).ReadWords(tempFile);

            CollectionAssert.AreEqual(new[] { "Hello", "WORLD" }, lines);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
