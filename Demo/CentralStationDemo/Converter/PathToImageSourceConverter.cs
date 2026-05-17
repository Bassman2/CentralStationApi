using System.Windows.Media;

namespace CentralStationDemo.Converter;

[ValueConversion(typeof(Uri), typeof(ImageSource))]
public class PathToImageSourceConverter : IValueConverter
{
    private readonly static string appPath = AppDomain.CurrentDomain.BaseDirectory;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && value is string path)
        {

            return ImageCache.GetImageFromPath(path);
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
