using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudContainer.Core.Utils;

public static class FileSaver
{
    public static void SaveFile(Bitmap bitmap, string path)
    {
        var extension = Path.GetExtension(path).ToLower();
        bitmap.Save(path, extension.GetImageFormat());
    }

    private static ImageFormat GetImageFormat(this string extension)
    {
        switch (extension)
        {
            case ".jpg":
                return ImageFormat.Jpeg;
            case ".png":
                return ImageFormat.Png;
            case ".bmp":
                return ImageFormat.Bmp;
            case ".gif":
                return ImageFormat.Gif;
            case ".tiff":
                return ImageFormat.Tiff;
            default:
                throw new ArgumentOutOfRangeException("extension", "Unsupported image format");
        }
    }
}