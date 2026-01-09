namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
    private readonly CentralStation cs; 

    public MainViewModel()
    {
        cs = new CentralStation(host, Protocol.TCP);
        cs.MessageReceived += (s, e) => App.Current.Dispatcher.Invoke(() => Messages.Insert(0, e.Message));
        cs.PropertyChanged += (s, e) => OnCsPropertyChanged(e.PropertyName);
    }

    public void Dispose() => cs.Dispose();

    protected override void OnStartup()
    {
        //http://cs3/app/assets/mag/magicon_a_005_01.svg

        //string name = "magicon_a_005_01";
        //var uri = new Uri($"http://{host}/app/assets/mag/{name}.svg");
        //return new BitmapImage(uri);

    }



    private void OnCsPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
        case "Status":
            Status = cs.Status;
            break;
        case "Locomotives":
            Locomotives = cs.Locomotives?.Locomotives?.ToViewModelList<LocomotiveViewModel>(host);
            break;
        case "Articles":
            //Articles = cs.Articles?.Articles?.Select(i => new ArticleViewModel(i))?.ToList();
            Articles = cs.Articles?.Articles?.ToViewModelList<ArticleViewModel>();
            break;
        case "Routes":
            Routes = cs.Routes?.Routes?.ToViewModelList<RouteViewModel>();
            break;
        case "Tracks":
            TrackPages = cs.Tracks?.Pages?.ToViewModelList<TrackPageViewModel>();
            break;
        case "Controllers":
            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"OnCsPropertyChanged Controllers # {cs.Controllers.Count()}");
            Controllers = [.. cs.Controllers.Select(i => new ControllerViewModel(i, host))];
            break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Status

    [ObservableProperty]
    private SystemStatus status = SystemStatus.Stop;


    [RelayCommand]
    private void OnStop()
    {
        switch (Status)
        {
        case SystemStatus.Default: cs.SystemStop(); break;
        case SystemStatus.Go: cs.SystemStop(); break;
        case SystemStatus.Stop: cs.SystemGo(); break;
        default: throw new InvalidEnumArgumentException(nameof(Status), (int)Status, typeof(SystemStatus));
        }
    }


    #endregion

    #region Locomotives

    [ObservableProperty]
    private List<LocomotiveViewModel>? locomotives;

    [RelayCommand]
    private void OnRequestLocomotioves()
    {
        cs.RequestConfigDataLocomotives();
    }

    [RelayCommand]
    private void OnSortLocomotives(string propertyName)
    {
        CollectionViewSource.GetDefaultView(Locomotives).SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
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

    #endregion

    #region Articles

    [ObservableProperty]
    private List<ArticleViewModel>? articles;

    [RelayCommand]
    private void OnRequestArticles()
    {
        cs.RequestConfigDataMagneticItems();
    }


    #endregion

    #region Routes

    [ObservableProperty]
    private List<RouteViewModel>? routes;

    [RelayCommand]
    private void OnRequestRoutes()
    {
        cs.RequestConfigDataRailwayRoute();
    }


    #endregion

    #region Tracks

    [ObservableProperty]
    private TrackData? trackData;

    [ObservableProperty]
    private List<TrackPageViewModel>? trackPages;

    [RelayCommand]
    private void OnRequestTracks()
    {
        cs.StartTrackCollector();
        //cs.RequestConfigDataTrackDiagram();
        //cs.RequestConfigDataTrackDiagramPage(1);
        //cs.RequestConfigDataTrackDiagramPage(2);
    }

    #endregion

    #region Controllers

    [ObservableProperty]
    private List<ControllerViewModel>? controllers;

    [RelayCommand]
    private void OnRequestControlUnits()
    {
        cs.RequestParticipants();
    }

    [RelayCommand]
    private void OnSortControllers(string propertyName)
    {
        CollectionViewSource.GetDefaultView(Controllers).SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
    }


    //private readonly AutoResetEvent statusDataEvent = new(false);

    //[ObservableProperty]
    //private List<ControllerViewModel> controlUnits = [];

    //[RelayCommand]
    //private void OnRequestControlUnits()
    //{
    //    cs.RequestParticipants();
    //}

    //private readonly Lock lockItem = new();
    ////private uint reqDeviceId = 0;
    //private ControllerViewModel? GetControlUnit(uint deviceId) => ControlUnits?.FirstOrDefault(d => d.DeviceId == deviceId);


    //private void UpdateControlUnits()
    //{
    //    // store for parallel changes
    //    ControllerViewModel[] controlUnits = [.. ControlUnits.Where(c => !c.HasDetails).Reverse()];

    //    Task.Run(() =>
    //    {
    //        lock (lockItem)
    //        {
    //            foreach (var controlUnit in controlUnits)
    //            {
    //                if (controlUnit.HasDetails == false)
    //                {
    //                    Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index 0");

    //                    cs.RequestStatusData(controlUnit.DeviceId, 0);
    //                    if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
    //                    {
    //                        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout {controlUnit.DeviceId:X8}");
    //                    }

    //                    for (byte index = 1; index <= controlUnit.NumOfMeasuredValues; index++)
    //                    {
    //                        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index {index}");

    //                        cs.RequestStatusData(controlUnit.DeviceId, index);
    //                        if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
    //                        {
    //                            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout Index {index} Device {controlUnit.DeviceId:X8}");
    //                        }
    //                    }
    //                }

    //                //for (byte index = 1; index <= StatusData.First(d => d.DeviceId == device).NumOfMeasuredValues + 10; index++)
    //                //{
    //                //    cs.RequestStatusData(device, index);
    //                //    statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
    //                //}
    //            }
    //        }
    //    });
    //}

    //private void UpdateControlUnitDetails(IEnumerable<StatusDataDevice> statusDataDevices)
    //{
    //    foreach (var device in statusDataDevices)
    //    {
    //        //var cu = Controllers?.FirstOrDefault(d => d.DeviceId == device.DeviceId);
    //        GetControlUnit(device.DeviceId)?.UpdateStatusData(device);
    //    }

    //    Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnitDetails Set Event");
    //    statusDataEvent.Set();
    //}


    #endregion


    #region Messages

    [ObservableProperty]
    private ObservableCollection<CANMessage> messages = [];

    #endregion
}
