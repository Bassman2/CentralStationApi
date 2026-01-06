namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
    private readonly CentralStation cs; 
    public MainViewModel()
    {
        cs = new CentralStation(host);
        cs.MessageReceived += (s, e) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Insert(0, e.Message);

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

    

    //private const uint CS3 = 0x63736E38;
    //private const uint MS2 = 0x4D54E34D;

    //private readonly uint[] allDevices = [CS3, MS2];

    protected override void OnStartup()
    {
       

        //cs.RequestConfigDataLocomotives();
        //cs.RequestConfigDataMagneticItems();
        //cs.RequestConfigDataRailwayRoute();
        //cs.RequestConfigDataTrackDiagram();
        //cs.RequestConfigDataTrackDiagramPage(1);
        //cs.RequestConfigDataTrackDiagramPage(2);

        // Timestamp	Sender	Binary	Priority	Command	IsResponse	Hash	Description
        //        00:42:40.1337   CS3.fritz.box   00315F1F 8 63 73 6E 38 0C 71 00 50  Proirity1 SoftwareVersion True    5F1F    Software Version -Sender: 63736E38 Version: 12.113 DeviceId: GFP3 50
        //00:42:40.1315   CS3.fritz.box   00317319 8 4D 54 E3 4D 05 06 00 33  Proirity1 SoftwareVersion True    7319    Software Version -Sender: 4D54E34D Version: 5.6 DeviceId: MS2_3 33

        //Task.Run(() =>
        //{
        //    foreach (var device in allDevices)
        //    {
        //        do
        //        {
        //            cs.RequestStatusData(device, 0);
        //            statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
        //        } while (StatusData is null || StatusData.Count == 0 || !StatusData.Any(d => d.DeviceId == device));

        //        for (byte index = 1; index <= StatusData.First(d => d.DeviceId == device).NumOfMeasuredValues + 10; index++)
        //        {
        //            cs.RequestStatusData(device, index);
        //            statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
        //        }
        //    }
        //});

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
            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"OnCsPropertyChanged Devices # {cs.Devices.Count()}");
            ControlUnits = [.. cs.Devices.Select(i => new ControlUnitViewModel(i))];
            UpdateControlUnits();
            break;
        case "StatusData":
            UpdateControlUnitDetails(cs.StatusData);
            break;

        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
    private List<MagneticItemViewModel>? magneticItems;

    [RelayCommand]
    private void OnRequestArticles()
    {
        cs.RequestConfigDataMagneticItems();
    }


    #endregion

    #region Routes

    [ObservableProperty]
    private List<RailwayRouteViewModel>? railwayRoutes;

    [RelayCommand]
    private void OnRequestRoutes()
    {
        cs.RequestConfigDataRailwayRoute();
    }


    #endregion

    #region Tracks

    [ObservableProperty]
    private TrackDiagram? trackDiagram;

    [RelayCommand]
    private void OnRequestTracks()
    {
        cs.RequestConfigDataTrackDiagram();
        //cs.RequestConfigDataTrackDiagramPage(1);
        //cs.RequestConfigDataTrackDiagramPage(2);
    }

    #endregion

    #region Controllers

    private readonly AutoResetEvent statusDataEvent = new(false);

    [ObservableProperty]
    private List<ControlUnitViewModel> controlUnits = [];

    [RelayCommand]
    private void OnRequestControlUnits()
    {
        cs.RequestParticipants();
    }

    private readonly Lock lockItem = new();
    //private uint reqDeviceId = 0;
    private ControlUnitViewModel? GetControlUnit(uint deviceId) => ControlUnits?.FirstOrDefault(d => d.DeviceId == deviceId);


    private void UpdateControlUnits()
    {
        // store for parallel changes
        ControlUnitViewModel[] controlUnits = [.. ControlUnits.Where(c => !c.HasDetails).Reverse()];

        Task.Run(() =>
        {
            lock (lockItem)
            {
                foreach (var controlUnit in controlUnits)
                {
                    if (controlUnit.HasDetails == false)
                    {
                        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index 0");

                        cs.RequestStatusData(controlUnit.DeviceId, 0);
                        if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
                        {
                            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout {controlUnit.DeviceId:X8}");
                        }

                        for (byte index = 1; index <= controlUnit.NumOfMeasuredValues; index++)
                        {
                            Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Request {controlUnit.DeviceId:X8} Index {index}");

                            cs.RequestStatusData(controlUnit.DeviceId, index);
                            if (!statusDataEvent.WaitOne(new TimeSpan(0, 0, 10)))
                            {
                                Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnits Timeout Index {index} Device {controlUnit.DeviceId:X8}");
                            }
                        }
                    }

                    //for (byte index = 1; index <= StatusData.First(d => d.DeviceId == device).NumOfMeasuredValues + 10; index++)
                    //{
                    //    cs.RequestStatusData(device, index);
                    //    statusDataEvent.WaitOne(new TimeSpan(0, 0, 10));
                    //}
                }
            }
        });
    }

    private void UpdateControlUnitDetails(IEnumerable<StatusDataDevice> statusDataDevices)
    {
        foreach (var device in statusDataDevices)
        {
            //var cu = Devices?.FirstOrDefault(d => d.DeviceId == device.DeviceId);
            GetControlUnit(device.DeviceId)?.UpdateStatusData(device);
        }

        Debug.WriteLineIf(AppTraceSwitches.DevicesSwitch.TraceInfo, $"UpdateControlUnitDetails Set Event");
        statusDataEvent.Set();
    }


    #endregion

    #region Status

    [ObservableProperty]
    private SystemStatus systemStatus = SystemStatus.Default;


    [RelayCommand]
    private void OnSystemStop()
    {
        cs.SystemStop();
        cs.SystemGo();
    }


    #endregion

    #region Messages

    [ObservableProperty]
    private ObservableCollection<CANMessage> messages = [];

    #endregion
}
