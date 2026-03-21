using CentralStationApi;

namespace CentralStationDemo.ViewModel;

public partial class DeviceViewModel : ObservableObject
{
    public DeviceViewModel(Device device)
    {
        DeviceId = device.DeviceId;
        Version = device.Version;
        DeviceType = device.DeviceType;
        DeviceTypeName = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
        IconUri = device.IconUri;

        //Measurements.Add(new DeviceMeasurementViewModel(new DeviceMeasurement() { Name = "Test A" }));
    }

    public void AddDeviceInfo(DeviceInfo deviceInfo)
    {
        NumOfMeasuredValues = deviceInfo.NumOfMeasuredValues;
        NumOfConfigurationChannels = deviceInfo.NumOfConfigurationChannels;
        SerialNumber = deviceInfo.SerialNumber;
        ArticleNumber = deviceInfo.ArticleNumber;
        DeviceName = deviceInfo.DeviceName;
    }

    public void AddDeviceMeasurement(DeviceMeasurement deviceMeasurement, byte index)
    {
        //Measurements.Add(new DeviceMeasurementViewModel(new DeviceMeasurement() { Name = "Test A" }));
        Measurements.Add(new DeviceMeasurementViewModel(deviceMeasurement));
    }

    public bool HasDetails = false;

    #region SoftwareVersion

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private Version version;
    
    [ObservableProperty]
    private DeviceType deviceType;

    [ObservableProperty]
    private string deviceTypeName;

    [ObservableProperty]
    private Uri? iconUri;

    #endregion

    #region DeviceDescription

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

    #endregion

    [ObservableProperty]
    private ObservableCollection<DeviceMeasurementViewModel> measurements = [];
}