using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CentralStationDemo.Misc;

public static class ImageCache
{
    private readonly static string appPath = AppDomain.CurrentDomain.BaseDirectory.Trim('\\');

    public static ImageSource GetImage(Uri uri)
    {
        string path = uri.LocalPath.Replace('/', '\\').Trim('\\'); 
        string fileName = Path.GetFileName(path);
        string fileExt  = Path.GetExtension(path);
        string folderName = Path.GetFileName(Path.GetDirectoryName(path))!;

        string filePath = Path.Combine(appPath, "ImageCache", folderName, fileName);

        if (!File.Exists(filePath))
        {
            DownloadFile(uri, filePath);
        }

        Uri local = new Uri(filePath);

        if (fileExt == ".png")
        {
            return new BitmapImage(local);
        }
        else if (fileExt == ".svg")
        {
            return SvgConverter.ConvertSvg(local);
        }
        else
        {
            throw new InvalidDataException();
        }

    }

   
    private static void DownloadFile(Uri uri, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        Task.Run(async () =>
        {
            var client = new HttpClient();
            using var stream = await client.GetStreamAsync(uri);
            using var writer = File.Create(filePath);
            await stream.CopyToAsync(writer);
        }).Wait();
    }

}
