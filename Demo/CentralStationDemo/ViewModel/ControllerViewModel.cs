namespace CentralStationDemo.ViewModel;

public partial class ControllerViewModel : ObservableObject
{
    public ControllerViewModel(Controller controller)
    {
        DeviceId = controller.DeviceId;
        SwVersion = $"{controller.MajorVersion}.{controller.MinorVersion}";
        Type = Enum.IsDefined<DeviceType>(controller.DeviceType) ? controller.DeviceType.ToString() : ((ushort)controller.DeviceType).ToString("X4");
        IconUri = controller.IconUri;

        //DeviceId = statusData.DeviceId;
        Index = controller.Index;
        NumOfPackages = controller.NumOfPackages;
        NumOfMeasuredValues = controller.NumOfMeasuredValues;
        NumOfConfigurationChannels = controller.NumOfConfigurationChannels;
        SerialNumber = controller.SerialNumber;
        ArticleNumber = controller.ArticleNumber ?? String.Empty;
        DeviceName = controller.DeviceName;
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

    [ObservableProperty]
    private Uri? iconUri;

    [ObservableProperty]
    private uint deviceId;

    [ObservableProperty]
    private string swVersion;
    
    [ObservableProperty]
    private string type;

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