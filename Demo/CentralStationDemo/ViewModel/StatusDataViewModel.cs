namespace CentralStationDemo.ViewModel;

public partial class StatusDataViewModel : ObservableObject
{
    public StatusDataViewModel(StatusDataDevice statusData)
    {
        DeviceId = statusData.DeviceId;
        Index = statusData.Index;
        NumOfPackages = statusData.NumOfPackages;
        NumOfMeasuredValues = statusData.NumOfMeasuredValues;
        NumOfConfigurationChannels = statusData.NumOfConfigurationChannels;
        SerialNumber = statusData.SerialNumber;
        ArticleNumber = statusData.ArticleNumber ?? String.Empty;
        DeviceName = statusData.DeviceName;        
    }

    [ObservableProperty]
    private uint deviceId;

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
    private string articleNumber;

    [ObservableProperty]
    private string deviceName;



}