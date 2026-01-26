namespace CentralStationDemo.ViewModel;

public partial class DeviceViewModel : ObservableObject
{
    //public DeviceViewModel(Controller controller)
    //{
    //    DeviceId = controller.DeviceId;
    //    SwVersion = $"{controller.MajorVersion}.{controller.MinorVersion}";
    //    Type = Enum.IsDefined<DeviceType>(controller.DeviceType) ? controller.DeviceType.ToString() : ((ushort)controller.DeviceType).ToString("X4");
    //    IconUri = controller.IconUri;

    //    //DeviceId = statusData.DeviceId;
    //    Index = controller.Index;
    //    NumOfPackages = controller.NumOfPackages;
    //    NumOfMeasuredValues = controller.NumOfMeasuredValues;
    //    NumOfConfigurationChannels = controller.NumOfConfigurationChannels;
    //    SerialNumber = controller.SerialNumber;
    //    ArticleNumber = controller.ArticleNumber ?? String.Empty;
    //    DeviceName = controller.DeviceName;
    //}

    public DeviceViewModel(Device device)
    {
        DeviceId = device.DeviceId;
        Version = new System.Version(device.MajorVersion, device.MinorVersion);
        DeviceType = device.DeviceType;
        DeviceTypeName = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
        IconUri = device.IconUri;
    }


    //public void UpdateStatusData(StatusDataDevice statusData)
    //{
    //    //DeviceId = statusData.DeviceId;
    //    Index = statusData.Index;
    //    NumOfPackages = statusData.NumOfPackages;
    //    NumOfMeasuredValues = statusData.NumOfMeasuredValues;
    //    NumOfConfigurationChannels = statusData.NumOfConfigurationChannels;
    //    SerialNumber = statusData.SerialNumber;
    //    ArticleNumber = statusData.ArticleNumber ?? String.Empty;
    //    DeviceName = statusData.DeviceName;
    //    HasDetails = true;
    //}

    public bool HasDetails = false;

    #region SoftwareVersion

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private System.Version version;
    
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
}