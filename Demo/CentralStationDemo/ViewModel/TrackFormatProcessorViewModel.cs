using CentralStationApi;
using System.Diagnostics.Metrics;
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
    private ObservableCollection<MeasurementViewModel> measurements = [];






    //########################
    //[ObservableProperty]
    //private string? name;

    //[ObservableProperty]
    //private string? article;

    //[ObservableProperty]
    //private Version? version;

    //[ObservableProperty]
    //private uint? serial;

    //[ObservableProperty]
    //private double? mainTrack;

    //[ObservableProperty]
    //private string? mainTrackUnit;

    //[ObservableProperty]
    //private double? programmingTrack;

    //[ObservableProperty]
    //private string? programmingTrackUnit;

    //[ObservableProperty]
    //private double? voltage;

    //[ObservableProperty]
    //private string? voltageUnit;

    //[ObservableProperty]
    //private double? temperature;

    //[ObservableProperty]
    //private string? temperatureUnit;

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
            var devices = await cs.GetAllDevicesAsync();
            gfpDevice = devices?.FirstOrDefault(d => d.DeviceType == DeviceType.GFP || d.DeviceType == DeviceType.GFP3);
            if (gfpDevice == null)
            {
                MessageBox.Show("No TFP/GFP found");
                return;
            }

            //Name = gfpDevice.D.DeviceTypeName;
            //Version = gfpDevice.Version;


            var gfpInfo = await cs.GetDeviceSystemDataAsync(gfpDevice.DeviceId);
            if (gfpInfo != null)
            {
                AddMeasurement(new MeasurementViewModel("Name", gfpInfo.DeviceName));
                AddMeasurement(new MeasurementViewModel("Version", gfpDevice.Version.ToString()));
                AddMeasurement(new MeasurementViewModel("Article", gfpInfo.ArticleNumber));
                AddMeasurement(new MeasurementViewModel("Serial", gfpInfo.SerialNumber.ToString()));

                //Name = ;
                //Article = ;
                //Serial = ;
            }

            deviceMeasurement1 = await cs.GetMeasurementSystemDataAsync(gfpDevice.DeviceId, 1);
            deviceMeasurement2 = await cs.GetMeasurementSystemDataAsync(gfpDevice.DeviceId, 2);
            deviceMeasurement3 = await cs.GetMeasurementSystemDataAsync(gfpDevice.DeviceId, 3);
            deviceMeasurement4 = await cs.GetMeasurementSystemDataAsync(gfpDevice.DeviceId, 4);
            AddMeasurement(new MeasurementViewModel(deviceMeasurement1!));
            AddMeasurement(new MeasurementViewModel(deviceMeasurement2!));
            AddMeasurement(new MeasurementViewModel(deviceMeasurement3!));
            AddMeasurement(new MeasurementViewModel(deviceMeasurement4!));
            //MainTrackUnit = deviceMeasurement1?.Unit;
            //ProgrammingTrackUnit = deviceMeasurement2?.Unit;
            //VoltageUnit = deviceMeasurement3?.Unit;
            //TemperatureUnit = deviceMeasurement4?.Unit;

        }

        GetChannel(1)?.SetValue(await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 1));
        GetChannel(2)?.SetValue(await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 2));
        GetChannel(3)?.SetValue(await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 3));
        GetChannel(4)?.SetValue(await cs.GetSystemStatusAsync(gfpDevice.DeviceId, 4));
        //GetChannel(2)?.SetValue
        //    GetChannel(3)?.SetValue
        //    GetChannel(4)?.SetValue

        // get system status
        //MainTrack = await centralStation.GetSystemStatusAsync(gfpDevice.DeviceId, 1) * Math.Pow(10.0, deviceMeasurement1?.ValuePower ?? -3.0);
        //ProgrammingTrack = await centralStation.GetSystemStatusAsync(gfpDevice.DeviceId, 2) * Math.Pow(10.0, deviceMeasurement2?.ValuePower ?? -3.0);
        //Voltage = await centralStation.GetSystemStatusAsync(gfpDevice.DeviceId, 3) * Math.Pow(10.0, deviceMeasurement3?.ValuePower ?? -3.0);
        //Temperature = await centralStation.GetSystemStatusAsync(gfpDevice.DeviceId, 4) * Math.Pow(10.0, deviceMeasurement4?.ValuePower ?? 0.0);
    }

    private void AddMeasurement(MeasurementViewModel measurementViewModel)
    {
        App.Current.Dispatcher.Invoke(() => Measurements.Add(measurementViewModel));
    }

    private MeasurementViewModel? GetChannel(int channel) => Measurements.FirstOrDefault(m => m.Channel == channel);
}
