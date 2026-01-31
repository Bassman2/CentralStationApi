using CentralStationWebApi;
using System.Timers;

namespace CentralStationDemo.ViewModel;

public partial class TrackFormatProcessorViewModel: ObservableObject
{
    private CentralStation cs;
    private Device? gfpDevice = null;
    private System.Timers.Timer updateTimer; 

    public TrackFormatProcessorViewModel(MainViewModel mainViewModels)
    {
        this.cs = mainViewModels.CentralStation;
        updateTimer = new(new TimeSpan(0, 0, 5)) { AutoReset = true };
        updateTimer.Elapsed += new ElapsedEventHandler(async (_,_) => await UpdateAsync());
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? article;

    [ObservableProperty]
    private System.Version? version;

    [ObservableProperty]
    private uint? serial;

    [ObservableProperty]
    private double? mainTrack;

    [ObservableProperty]
    private string? mainTrackUnit;

    [ObservableProperty]
    private double? programmingTrack;

    [ObservableProperty]
    private string? programmingTrackUnit;

    [ObservableProperty]
    private double? voltage;

    [ObservableProperty]
    private string? voltageUnit;

    [ObservableProperty]
    private double? temperature;

    [ObservableProperty]
    private string? temperatureUnit;

    [ObservableProperty]
    private bool isSelected;

    private DeviceMeasurement? deviceMeasurement1;
    private DeviceMeasurement? deviceMeasurement2;
    private DeviceMeasurement? deviceMeasurement3;
    private DeviceMeasurement? deviceMeasurement4;
    
    partial void OnIsSelectedChanged(bool value)
    {
        if (value)
        {
            Task.Run(async () => await UpdateAsync()).ContinueWith((_) => updateTimer.Start());
        }
        else
        {
            updateTimer.Stop();
        }

    }

    private async Task UpdateAsync()
    {
        // get TGP/GFP device
        if (gfpDevice == null)
        {
            var devices = await cs.GetDevicesAsync();
            gfpDevice = devices?.FirstOrDefault(d => d.DeviceType == DeviceType.GFP || d.DeviceType == DeviceType.GFP3);
            if (gfpDevice == null)
            {
                MessageBox.Show("No TFP/GFP found");
                return;
            }

            //Name = gfpDevice.D.DeviceTypeName;
            Version = gfpDevice.Version;

            var gfpInfo = await cs.GetDeviceInfoAsync(gfpDevice.DeviceId);
            if (gfpInfo != null)
            {
                Name = gfpInfo.DeviceName;
                Article = gfpInfo.ArticleNumber;
                Serial = gfpInfo.SerialNumber;
            }

            deviceMeasurement1 = await cs.GetDeviceMeasurementAsync(gfpDevice.DeviceId, 1);
            deviceMeasurement2 = await cs.GetDeviceMeasurementAsync(gfpDevice.DeviceId, 2);
            deviceMeasurement3 = await cs.GetDeviceMeasurementAsync(gfpDevice.DeviceId, 3);
            deviceMeasurement4 = await cs.GetDeviceMeasurementAsync(gfpDevice.DeviceId, 4);

            MainTrackUnit = deviceMeasurement1?.Unit;
            ProgrammingTrackUnit = deviceMeasurement2?.Unit;
            VoltageUnit = deviceMeasurement3?.Unit;
            TemperatureUnit = deviceMeasurement4?.Unit;

        }

        // get system status
        MainTrack = await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 1) * Math.Pow(10.0, deviceMeasurement1?.ValuePower ?? -3.0);
        ProgrammingTrack = await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 2) * Math.Pow(10.0, deviceMeasurement2?.ValuePower ?? -3.0);
        Voltage = await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 3) * Math.Pow(10.0, deviceMeasurement3?.ValuePower ?? -3.0);
        Temperature = await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 4) * Math.Pow(10.0, deviceMeasurement4?.ValuePower ?? 0.0);
    }
}
