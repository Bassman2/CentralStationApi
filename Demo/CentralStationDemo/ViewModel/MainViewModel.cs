using CentralStationDemo.View;
using CentralStationWebApi;
using System.Linq;

namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
    private CentralStation cs; 
    public MainViewModel()
    {
        cs = new CentralStation(host);
        cs.MessageReceived += (s, e) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Insert(0, new MessageViewModel(e.Message));

                UpdateStatus(e.Message);
                UpdateLocomotive(e.Message);
            });
        };
        cs.PropertyChanged += (s, e) => OnCsPropertyChanged(e.PropertyName);
    }

    public void Dispose()
    {
        //cs.Close();
        cs.Dispose();
    }

    private AutoResetEvent statusDataEvent = new AutoResetEvent(false);

    private const uint CS3 = 0x63736E38;
    private const uint MS2 = 0x4D54E34D;

    private readonly uint[] allDevices = { CS3, MS2 };

    protected override void OnStartup()
    {
        //cs.RequestParticipants();

        //cs.RequestConfigDataLocomotives();
        //cs.RequestConfigDataMagneticItems();
        //cs.RequestConfigDataRailwayRoute();
        //cs.RequestConfigDataTrackDiagram();
        //cs.RequestConfigDataTrackDiagramPage(1);
        //cs.RequestConfigDataTrackDiagramPage(2);

        // Timestamp	Sender	Binary	Priority	Command	IsResponse	Hash	Description
        //        00:42:40.1337   CS3.fritz.box   00315F1F 8 63 73 6E 38 0C 71 00 50  Proirity1 SoftwareVersion True    5F1F    Software Version -Sender: 63736E38 Version: 12.113 DeviceId: GFP3 50
        //00:42:40.1315   CS3.fritz.box   00317319 8 4D 54 E3 4D 05 06 00 33  Proirity1 SoftwareVersion True    7319    Software Version -Sender: 4D54E34D Version: 5.6 DeviceId: MS2_3 33

        Task.Run(() =>
        {
            foreach (var device in allDevices)
            {
                do
                {
                    cs.RequestStatusData(device, 0);
                    statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
                } while (StatusData is null || StatusData.Count == 0 || !StatusData.Any(d => d.DeviceId == device));

                for (byte index = 1; index <= StatusData.First(d => d.DeviceId == device).NumOfMeasuredValues; index++)
                {
                    cs.RequestStatusData(device, index);
                    statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
                }
            }
        });

    }

    private void UpdateStatus(CANMessage message)
    { 
        if (message.Command == Command.SystemCommand && message.SubCommand == SubCommand.Stop && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.Stop;
        }
        if (message.Command == Command.SystemCommand && message.SubCommand == SubCommand.Go && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.Go;
        }
    }

    private void UpdateLocomotive(CANMessage message)
    {
        if (message.Command == Command.LocoVelocity ||
            message.Command == Command.LocoDirection ||
            message.Command == Command.LocoFunction)
        {
            var locomotiveViewModel = Locomotives?.FirstOrDefault(l => l.Uid == message.Device);
            locomotiveViewModel?.UpdateLocomotive(message);
        }

    }

    private void OnCsPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
        case "Locomotives":
            Locomotives = cs.Locomotives?.Locomotives_?.Select(i => new LocomotiveViewModel(host, i))?.ToList(); 
            break;
        case "MagneticItems":
            MagneticItems = cs.MagneticItems?.Articles?.Select(i => new MagneticItemViewModel(i))?.ToList();
            break;
        case "RailwayRoutes":
            RailwayRoutes = cs.RailwayRoutes?.RailwayRoutes_?.Select(i => new RailwayRouteViewModel(i))?.ToList();
            break;
        case "TrackDiagram":
            TrackDiagram = cs.TrackDiagram;
            break;
        case "Devices":
            Devices = cs.Devices.Select(i => new DeviceViewModel(i)).ToList();
            break;
        case "StatusDataDevice":
            StatusData = cs.StatusData.Select(i => new StatusDataViewModel(i)).ToList();
            statusDataEvent.Set();
            break;

        }
    }

    [ObservableProperty]
    private SystemStatus systemStatus = SystemStatus.Default;

    [ObservableProperty]
    private uint device = 0;

    //partial void OnDeviceChanged(uint value)
    //{ 
    //}

    [ObservableProperty]
    private List<LocomotiveViewModel>? locomotives;

    [ObservableProperty]
    private List<MagneticItemViewModel>? magneticItems;

    [ObservableProperty]
    private List<RailwayRouteViewModel>? railwayRoutes;

    [ObservableProperty]
    private TrackDiagram? trackDiagram;

    [ObservableProperty]
    private List<DeviceViewModel>? devices;

    [ObservableProperty]
    private List<StatusDataViewModel>? statusData;


    [ObservableProperty]
    private ObservableCollection<MessageViewModel> messages = [];

   



    #region System Sync Commands

    [RelayCommand]
    private void OnSystemStop() => cs.SystemStop();
    
    [RelayCommand]
    private void OnSystemGo() => cs.SystemGo();
    
    [RelayCommand]
    private void OnSystemHalt() => cs.SystemHalt();

    [RelayCommand]
    private void OnSystemLocoHalt() => cs.SystemLocoHalt();

    [RelayCommand]
    private void OnSystemLocoCycleStop() => cs.SystemLocoCycleStop();


    #endregion

    #region System Async Commands

    [RelayCommand]
    private async Task OnSystemStopAsyncAsync()
    {
        await cs.SystemStopAsync();
    }


    [RelayCommand]
    private async Task OnSystemGoAsyncAsync()
    {
        await cs.SystemGoAsync();
    }

    [RelayCommand]
    private async Task OnSystemHaltAsyncAsync()
    {
        await cs.SystemHaltAsync();
    }

    [RelayCommand]
    private async Task OnSystemLocoHaltAsyncAsync()
    {
        await cs.SystemLocoHaltAsync();
    }

    [RelayCommand]
    private async Task OnSystemLocoCycleStopAsyncAsync()
    {
        await cs.SystemLocoCycleStopAsync();
    }

    #endregion

    //[RelayCommand]
    //private async Task OnLocoInfo()
    //{
    //    await cs.ConfigDataLocomotivesInfo();
    //}

    //[RelayCommand]
    //private async Task OnLocos()
    //{
    //    string file = await cs.RequestConfigDataLocomotives();
    //}

    
}
