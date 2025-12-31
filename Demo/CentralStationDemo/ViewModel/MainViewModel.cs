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

    protected override void OnStartup()
    {
        cs.RequestParticipants();

        cs.RequestConfigDataLocomotives();
        cs.RequestConfigDataMagneticItems();
        cs.RequestConfigDataRailwayRoute();
        cs.RequestConfigDataTrackDiagram();
        cs.RequestConfigDataTrackDiagramPage(1);
        cs.RequestConfigDataTrackDiagramPage(2);

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
