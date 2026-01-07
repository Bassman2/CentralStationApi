using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CentralStationDemo.ViewModel;

public partial class ControllerViewModel : ObservableObject
{
    public ControllerViewModel(Controller controller, string host)
    {
        DeviceId = controller.DeviceId;
        SwVersion = $"{controller.MajorVersion}.{controller.MinorVersion}";
        Type = Enum.IsDefined<DeviceType>(controller.DeviceType) ? controller.DeviceType.ToString() : ((ushort)controller.DeviceType).ToString("X4");

        string iconName = controller.DeviceType switch
        {
            CentralStationWebApi.DeviceType.GFP => "dashboard_gfp3",
            CentralStationWebApi.DeviceType.DCB => "dashboard_cs1",
            CentralStationWebApi.DeviceType.DCB1 => "dashboard_cs1",
            CentralStationWebApi.DeviceType.Connect => "dashboard_cs2",
            CentralStationWebApi.DeviceType.MS2 => "dashboard_ms2",
            CentralStationWebApi.DeviceType.MS2_1 => "dashboard_ms2",
            CentralStationWebApi.DeviceType.MS2_2 => "dashboard_ms2",
            CentralStationWebApi.DeviceType.MS2_3 => "dashboard_ms2",
            CentralStationWebApi.DeviceType.MS2_4 => "dashboard_ms2",
            CentralStationWebApi.DeviceType.LinkS88 => "dashboard_links88",
            CentralStationWebApi.DeviceType.GFP3 => "dashboard_gfp3",
            CentralStationWebApi.DeviceType.CS2 => "dashboard_cs2",
            CentralStationWebApi.DeviceType.Wireless => "dashboard_tablet",
            CentralStationWebApi.DeviceType.Wired => "dashboard_cs3",
            CentralStationWebApi.DeviceType.GUI => "dashboard_cs3",
            _ => string.Empty,
        };

        if (!string.IsNullOrEmpty(iconName))
        {
            Icon = App.Current.Dispatcher.Invoke(() =>
            {
                var uri = new Uri($"http://{host}/images/gui/{iconName}.png");
                return new BitmapImage(uri);
            });
        }
    }

    // http://cs3/images/gui/dashboard_desktop.png
    // http://cs3/images/gui/dashboard_booster.png
    // http://cs3/images/gui/dashboard_ms1.png
    // http://cs3/images/gui/dashboard_ms2.png
    // http://cs3/images/gui/dashboard_cs1.png
    // http://cs3/images/gui/dashboard_cs2.png
    // http://cs3/images/gui/dashboard_cs3.png
    // http://cs3/images/gui/dashboard_gfp3.png
    // http://cs3/images/gui/dashboard_tablet.png
    // http://cs3/images/gui/dashboard_smartphone.png
    // http://cs3/images/gui/dashboard_links88.png

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
    private ImageSource? icon;

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