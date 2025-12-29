namespace CentralStationDemo.ViewModel;

public partial class DeviceViewModel : ObservableObject
{
    public DeviceViewModel(Device device)
    {
    
        DeviceId = device.DeviceId;
        SwVersion = $"{device.MajorVersion}.{device.MinorVersion}";
        DeviceType = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
    }

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private string swVersion;
    
    [ObservableProperty]
    private string deviceType;
}