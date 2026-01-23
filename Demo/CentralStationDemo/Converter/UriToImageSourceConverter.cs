using CentralStationDemo.Misc;
using System.IO;
using System.Net.Http;
using System.Windows.Media;

namespace CentralStationDemo.Converter;


[ValueConversion(typeof(Uri), typeof(ImageSource))]
public class UriToImageSourceConverter : IValueConverter
{
    private readonly static string appPath = AppDomain.CurrentDomain.BaseDirectory;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && value is Uri uri )
        {

            return ImageCache.GetImage(uri);

            //string filePath = System.IO.Path.Combine(appPath.Trim('\\'), "ImageCache", uri.LocalPath.Replace('/', '\\').Trim('\\'));
            //string dir = System.IO.Path.GetDirectoryName(filePath)!;
            //System.IO.Directory.CreateDirectory(dir);

            //if (!System.IO.File.Exists(filePath))
            //{
            //    Task.Run(async () =>
            //    {
            //        var client = new HttpClient();
            //        using var stream = await client.GetStreamAsync(uri);
            //        using var writer = File.Create(filePath);
            //        await stream.CopyToAsync(writer);
            //    }).Wait();
            //}

            //uri = new Uri(filePath);

            //if (uri.AbsolutePath.EndsWith(".png"))
            //{
            //    return new System.Windows.Media.Imaging.BitmapImage(uri);
            //}
            //else if (uri.AbsolutePath.EndsWith(".svg"))
            //{
            //    return SvgConverter.ConvertSvg(uri);
            //}
            //else
            //{
            //    throw new InvalidOperationException();
            //}
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

