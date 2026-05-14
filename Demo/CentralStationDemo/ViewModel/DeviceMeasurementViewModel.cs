namespace CentralStationDemo.ViewModel;

public partial class DeviceMeasurementViewModel(DeviceMeasurement deviceMeasurement) : ObservableObject
{
    [ObservableProperty]
    public partial byte Channel { get; set; } = deviceMeasurement.Channel;

    [ObservableProperty]
    public partial sbyte ValuePower { get; set; } = deviceMeasurement.ValuePower;

    [ObservableProperty]
    public partial byte ColorRange1 { get; set; } = deviceMeasurement.ColorRange1;

    [ObservableProperty]
    public partial byte ColorRange2 { get; set; } = deviceMeasurement.ColorRange2;

    [ObservableProperty]
    public partial byte ColorRange3 { get; set; } = deviceMeasurement.ColorRange3;

    [ObservableProperty]
    public partial byte ColorRange4 { get; set; } = deviceMeasurement.ColorRange4;

    [ObservableProperty]
    public partial ushort ZeroPoint { get; set; } = deviceMeasurement.ZeroPoint;

    [ObservableProperty]
    public partial ushort EndRange1 { get; set; } = deviceMeasurement.EndRange1;

    [ObservableProperty]
    public partial ushort EndRange2 { get; set; } = deviceMeasurement.EndRange2;

    [ObservableProperty]
    public partial ushort EndRange3 { get; set; } = deviceMeasurement.EndRange3;

    [ObservableProperty]
    public partial ushort EndRange4 { get; set; } = deviceMeasurement.EndRange4;

    [ObservableProperty]
    public partial string Name { get; set; } = deviceMeasurement.Name;

    [ObservableProperty]
    public partial string Start { get; set; } = deviceMeasurement.Start;

    [ObservableProperty]
    public partial string End { get; set; } = deviceMeasurement.End;

    [ObservableProperty]
    public partial string Unit { get; set; } = deviceMeasurement.Unit;
}
