namespace CentralStationDemo.Converter;

[ValueConversion(typeof(bool), typeof(string))]
public class ResponseToArrowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool flag)
        {
            return flag ? "◄" : "►";
        }
        throw new InvalidOperationException("The value must be a bool");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
