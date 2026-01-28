using CentralStationWebApi.Serializer;
using System.IO;

namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
    private const string locomotivesFileName = "Lokomotive.cs2";
    private const string articlesFileName = "magnetartikel.cs2";
    private const string routesFileName = "fahrstrassen.cs2";
    private const string tracksFileName = "gleisbild.cs2";

    private readonly CentralStation cs;

    public string Host => host;

    public MainViewModel()
    {
        cs = new CentralStation(host, Protocol.TCP);
        cs.MessageReceived += (s, e) => App.Current.Dispatcher.Invoke(() => Messages.Insert(0, e.Message));
        cs.PropertyChanged += (s, e) => OnCsPropertyChanged(e.PropertyName);

        cs.LocomotiveHalt += (s, e) => OnLocomotiveHalt(e.LocomotiveId);
        cs.LocomotiveVelocity += (s,e) => OnLocomotiveVelocity(e.LocomotiveId, e.Velocity);
        cs.LocomotiveDirection += (s, e) => OnLocomotiveDirection(e.LocomotiveId, e.Direction); 
        cs.LocomotiveFunction += (s, e) => OnLocomotiveFunction(e.LocomotiveId, e.Function, e.Value);

        cs.FileReceived += (s, e) => OnFileReceived(e.FileName, e.Stream);

    }

    public void Dispose() => cs.Dispose();

    protected override void OnStartup()
    {
        //http://cs3/app/assets/mag/magicon_a_005_01.svg

        // load locomotive data 
        if (File.Exists(locomotivesFileName))
        {
            using var file = File.OpenRead(locomotivesFileName);
            var locomotives = CsSerializer.Deserialize<LocomotiveData>(file);
            Locomotives = locomotives.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }

        // load articles data 
        if (File.Exists(articlesFileName))
        {
            using var file = File.OpenRead(articlesFileName);
            var articles = CsSerializer.Deserialize<ArticleData>(file);
            Articles = articles.Articles?.ToViewModelList<ArticleViewModel>(cs);
        }

        // load routes data 
        if (File.Exists(routesFileName))
        {
            using var file = File.OpenRead(routesFileName);
            var routes = CsSerializer.Deserialize<RouteData>(file);
            Routes = routes.Routes?.ToViewModelList<RouteViewModel>();
        }

        // load tracks data 
        if (File.Exists(tracksFileName))
        {
            using (var file = File.OpenRead(tracksFileName))
            {
                var tracks = CsSerializer.Deserialize<TrackData>(file);
                TrackData = tracks;
            }
            TrackPages = [];
            foreach (var page in TrackData?.Pages ?? [])
            {
                string pageFileName = $"gleisbild-{page.Id}.cs2";
                if (File.Exists(pageFileName))
                {
                    using var file2 = File.OpenRead(pageFileName);
                    var trackPage = CsSerializer.Deserialize<TrackPageData>(file2);
                    TrackPages.Add(new TrackPageViewModel(page, trackPage));
                }
                else
                {
                    TrackPages.Add(new TrackPageViewModel(page, null));
                }
            }


        }



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
            Locomotives = cs.Locomotives?.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
            break;
        case "Articles":
            Articles = cs.Articles?.Articles?.ToViewModelList<ArticleViewModel>(cs);
            break;
        case "Routes":
            Routes = cs.Routes?.Routes?.ToViewModelList<RouteViewModel>();
            break;
        case "Tracks":
            TrackPages = cs.Tracks?.Pages?.ToViewModelList<TrackPageViewModel>();
            break;
        //case "Controllers":
        //    Controllers = cs.Controllers?.ToViewModelList<DeviceViewModel>();
        //    break;
        }
    }

    private void OnFileReceived(string fileName, System.IO.Stream stream)
    {
        stream.Position = 0;
        using (var file = File.Create(fileName))
        {
            stream.CopyTo(file);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [ObservableProperty]
    private double zoomValue = 1.0;


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

    //private void UpdateLocomotive(CANMessage message)
    //{
    //    if (message.Command == Command.LocoVelocity ||
    //        message.Command == Command.LocoDirection ||
    //        message.Command == Command.LocoFunction)
    //    {
    //        var locomotiveViewModel = Locomotives?.FirstOrDefault(l => l.Uid == message.Device);
    //        locomotiveViewModel?.UpdateLocomotive(message);
    //    }
    //}

    private void OnLocomotiveHalt(uint locomotiveId)
    {
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.Halt();
    }
    private void OnLocomotiveVelocity(uint locomotiveId, ushort velocity)
    {   
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetVelocity(velocity);
    }

    private void OnLocomotiveDirection(uint locomotiveId, DirectionChange direction)
    { 
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetDirection(direction);
    }

    private void OnLocomotiveFunction(uint locomotiveId, byte function, byte value)
    {
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetFunction(function, value);
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

    #region Devices

    [ObservableProperty]
    private List<DeviceViewModel>? devices;

    [RelayCommand]
    private void OnUpdateDevices()
    {
        Task.Run((Func<Task?>)(async () =>
        {
            var devices = await cs.GetDevicesAsync();

            var vms = devices?.Select(d => new DeviceViewModel(d)).ToList();
            App.Current.Dispatcher.Invoke(() => Devices = vms);

            foreach (var device in Devices ?? [])
            {
                var deviceInfo = await cs.GetDeviceInfoAsync(device.DeviceId);
                if (deviceInfo != null)
                {
                    device.AddDeviceInfo(deviceInfo);
                }
            }

            
        }));
    }

    [RelayCommand]
    private void OnSortControllers(string propertyName)
    {
        CollectionViewSource.GetDefaultView(Devices).SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
    }


    //private readonly AutoResetEvent statusDataEvent = new(false);

    //[ObservableProperty]
    //private List<DeviceViewModel> controlUnits = [];

    //[RelayCommand]
    //private void OnRequestControlUnits()
    //{
    //    cs.RequestParticipants();
    //}

    //private readonly Lock lockItem = new();
    ////private uint reqDeviceId = 0;
    //private DeviceViewModel? GetControlUnit(uint deviceId) => ControlUnits?.FirstOrDefault(d => d.DeviceId == deviceId);


    //private void UpdateControlUnits()
    //{
    //    // store for parallel changes
    //    DeviceViewModel[] controlUnits = [.. ControlUnits.Where(c => !c.HasDetails).Reverse()];

    //    Task.Run(() =>
    //    {
    //        lock (lockItem)
    //        {
    //            foreach (var controlUnit in controlUnits)
    //            {
    //                if (controlUnit.HasDetails == false)
    //                {
    //                    Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index 0");

    //                    cs.StatusData(controlUnit.DeviceId, 0);
    //                    if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
    //                    {
    //                        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout {controlUnit.DeviceId:X8}");
    //                    }

    //                    for (byte index = 1; index <= controlUnit.NumOfMeasuredValues; index++)
    //                    {
    //                        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index {index}");

    //                        cs.StatusData(controlUnit.DeviceId, index);
    //                        if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
    //                        {
    //                            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout Index {index} Device {controlUnit.DeviceId:X8}");
    //                        }
    //                    }
    //                }

    //                //for (byte index = 1; index <= StatusData.First(d => d.DeviceId == device).NumOfMeasuredValues + 10; index++)
    //                //{
    //                //    cs.StatusData(device, index);
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
