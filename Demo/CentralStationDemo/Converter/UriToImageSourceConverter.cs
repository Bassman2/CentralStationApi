using System.Windows.Media;

namespace CentralStationDemo.Converter;


[ValueConversion(typeof(Uri), typeof(ImageSource))]
public class UriToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && value is Uri uri )
        {
            return new System.Windows.Media.Imaging.BitmapImage(uri);
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

