namespace CentralStationDemo.ViewModel;

public partial class DeviceViewModel : ObservableObject
{
    public DeviceViewModel(Device device)
    {
    
        DeviceId = device.DeviceId;
        SwVersion = $"{device.MajorVersion}.{device.MinorVersion}";
        DeviceType = device.DeviceType switch
        {
            0x0033 => "MS2",
            0x0050 => "GFP3",
            0xffff => "GUI (Master)",
            _ => device.DeviceType.ToString("X4")
        };  
    }

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private string swVersion;
    
    [ObservableProperty]
    private string deviceType;
}