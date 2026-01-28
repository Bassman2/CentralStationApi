namespace CentralStationDemo.ViewModel;

public partial class DeviceMeasurementViewModel(DeviceMeasurement deviceMeasurement) : ObservableObject
{
    [ObservableProperty]
    private byte channel = deviceMeasurement.Channel;

    [ObservableProperty]
    private byte valuePower = deviceMeasurement.ValuePower;

    [ObservableProperty]
    private byte colorRange1 = deviceMeasurement.ColorRange1;

    [ObservableProperty]
    private byte colorRange2 = deviceMeasurement.ColorRange2;

    [ObservableProperty]
    private byte colorRange3 = deviceMeasurement.ColorRange3;

    [ObservableProperty]
    private byte colorRange4 = deviceMeasurement.ColorRange4;

    [ObservableProperty]
    private byte zeroPoint = deviceMeasurement.ZeroPoint;

    [ObservableProperty]
    private byte endRange1 = deviceMeasurement.EndRange1;

    [ObservableProperty]
    private byte endRange2 = deviceMeasurement.EndRange2;

    [ObservableProperty]
    private byte endRange3 = deviceMeasurement.EndRange3;

    [ObservableProperty]
    private byte endRange4 = deviceMeasurement.EndRange4;

    [ObservableProperty]
    private string name = deviceMeasurement.Name;

    [ObservableProperty]
    private string start = deviceMeasurement.Start;

    [ObservableProperty]
    private string end = deviceMeasurement.End;

    [ObservableProperty]
    private string unit = deviceMeasurement.Unit;
}
