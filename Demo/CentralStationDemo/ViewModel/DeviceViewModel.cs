using CentralStationApi;

namespace CentralStationDemo.ViewModel;

public partial class DeviceViewModel : ObservableObject
{
    //public DeviceViewModel(Device device)
    //{
    //    DeviceId = device.DeviceId;
    //    Version = device.Version;
    //    DeviceType = device.DeviceType;
    //    DeviceTypeName = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
    //    IconUri = device.IconUri;

    //    //Measurements.Add(new DeviceMeasurementViewModel(new DeviceMeasurement() { Name = "Test A" }));
    //}

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
    public partial uint DeviceId { get; set; }

    [ObservableProperty]
    public partial Version Version { get; set; }

    [ObservableProperty]
    public partial DeviceType DeviceType { get; set; }

    [ObservableProperty]
    public partial string DeviceTypeName { get; set; }

    [ObservableProperty]
    public partial Uri? IconUri { get; set; }

    [ObservableProperty]
    public partial string? ImagePath { get; set; }

    #endregion

    #region DeviceDescription

    [ObservableProperty]
    public partial byte NumOfMeasuredValues { get; set; }

    [ObservableProperty]
    public partial byte NumOfConfigurationChannels { get; set; }

    [ObservableProperty]
    public partial uint SerialNumber { get; set; }

    [ObservableProperty]
    public partial string ArticleNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DeviceName { get; set; } = string.Empty;

    #endregion

    [ObservableProperty]
    public partial ObservableCollection<DeviceMeasurementViewModel> Measurements { get; set; } = [];
}