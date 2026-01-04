namespace CentralStationDemo.ViewModel;

public partial class ControlUnitViewModel : ObservableObject
{
    public ControlUnitViewModel(Device device)
    {
        DeviceId = device.DeviceId;
        SwVersion = $"{device.MajorVersion}.{device.MinorVersion}";
        DeviceType = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
    }

    public void UpdateStatusData(StatusDataDevice statusData)
    {
        //DeviceId = statusData.DeviceId;
        Index = statusData.Index;
        NumOfPackages = statusData.NumOfPackages;
        NumOfMeasuredValues = statusData.NumOfMeasuredValues;
        NumOfConfigurationChannels = statusData.NumOfConfigurationChannels;
        SerialNumber = statusData.SerialNumber;
        ArticleNumber = statusData.ArticleNumber ?? String.Empty;
        DeviceName = statusData.DeviceName;
        HasDetails = true;
    }

    public bool HasDetails = false; 

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private string swVersion;
    
    [ObservableProperty]
    private string deviceType;

    [ObservableProperty]
    private byte index;

    [ObservableProperty]
    private byte numOfPackages;

    [ObservableProperty]
    private byte numOfMeasuredValues;

    [ObservableProperty]
    private byte numOfConfigurationChannels;

    [ObservableProperty]
    private uint serialNumber;

    [ObservableProperty]
    private string articleNumber = string.Empty;

    [ObservableProperty]
    private string deviceName = string.Empty;
}