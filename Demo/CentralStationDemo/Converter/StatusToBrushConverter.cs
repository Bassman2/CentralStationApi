using System.Windows.Media;

namespace CentralStationDemo.Converter;

[ValueConversion(typeof(SystemStatus), typeof(Brush))]
public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SystemStatus status)
        {
            return status switch
            {
                SystemStatus.Stop => Brushes.Red,
                SystemStatus.Go => Brushes.Green,
                SystemStatus.Default => Brushes.Gray,
                _ => throw new InvalidEnumArgumentException(nameof(value), (int)status, typeof(SystemStatus))
            }; 
        }
        throw new InvalidOperationException("The value must be a SystemStatus");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

