namespace CentralStationDemo.ViewModel;

public partial class MeasurementViewModel : ObservableObject
{
    private DeviceMeasurement? deviceMeasurement = null;

    private double power = 0.0;

    public MeasurementViewModel(DeviceMeasurement deviceMeasurement)
    {
        this.deviceMeasurement = deviceMeasurement;

        this.Channel = deviceMeasurement.Channel;
        this.Label = deviceMeasurement.Name;
        this.power = deviceMeasurement.ValuePower;
        //SetValue(value);
    }

    public MeasurementViewModel(string label, string value)
    {
        Label = label;
        Value = value;
    }

    public void SetValue(double value)
    {
        Value = (value * Math.Pow(10.0, power)).ToString();
    }

    public void SetValue(uint? value)
    {
        Value = value == null ? "err" : (value * Math.Pow(10.0, power)).ToString();
    }

    public int Channel { get; } = 0;

    [ObservableProperty]
    private string? label;

    [ObservableProperty]
    private string? value;

    [ObservableProperty]
    private string? unit;
}
