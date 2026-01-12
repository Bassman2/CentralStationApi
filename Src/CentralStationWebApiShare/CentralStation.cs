namespace CentralStationWebApi;

public class CentralStation : CentralStationBasic, INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public event EventHandler<FileReceivedEventArgs>? FileReceived;

    public event EventHandler<LocomotiveEventArgs>? LocomotiveHalt;
    public event EventHandler<LocomotiveVelocityEventArgs>? LocomotiveVelocity;
    public event EventHandler<LocomotiveDirectionEventArgs>? LocomotiveDirection;
    public event EventHandler<LocomotiveFunctionEventArgs>? LocomotiveFunction;

    private readonly EventQueue<(uint deviceId, byte index)> statusDataEventQueue;

    private readonly CollectorThread trackCollectorThread;
    private readonly CollectorThread contrCollectorThread;

    private readonly TimeSpan timeout = TimeSpan.FromSeconds(1000);
    //private readonly int retry = 3;

    public CentralStation(string host, Protocol protocol = Protocol.TCP) : base(host, protocol)
    {
        statusDataEventQueue = new (tuple => RequestStatusData(tuple.deviceId, tuple.index), TimeSpan.FromSeconds(10));

        trackCollectorThread = new CollectorThread(TrackCollectorWorkerLoop, timeout);
        contrCollectorThread = new CollectorThread(ContrCollectorWorkerLoop, timeout);
    }
        

    protected override void ReceiveHandler(CANMessage msg)
    {
        HandleLocomotive(msg);
        HandleStatus(msg);
        HandleStreams(msg);
        HandleController(msg);
        //HandleStatusData(msg);
    }

    private readonly Dictionary<ushort, CSFileStream> fileDictionary = [];
    private string fileKey = "empty";

    private void HandleStreams(CANMessage msg)
    {

        if (msg.IsResponse && msg.Command == Command.RequestConfigData)
        {
            fileKey = msg.GetDataString().Trim('\0');
        }
        if (/*msg.IsResponse && */ msg.Command == Command.ConfigDataStream)
        {
            if (msg.DataLength == 6)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Request, fileKey, msg.GetDataUInt(0), msg.GetDataUShort(0));
            }
            else if (msg.DataLength == 7)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Broadcast, fileKey, msg.GetDataUInt(0), msg.GetDataUShort(4), msg.GetDataByte(6));
            }
            else if (msg.DataLength == 8 && fileDictionary.TryGetValue(msg.Hash, out var fileStream))
            {
                fileStream.AddData(msg.GetData());
                if (fileStream.IsReady())
                {
                    //fileReceivedQueue.Add(fileStream);
                    SetFile(fileStream.GetFileStream(), fileStream.FileKey, fileStream.FileName);
                    fileDictionary.Remove(msg.Hash);
                }
            }
            else
            {
                Debug.WriteLineIf(TraceSwitches.CanReceiveSwitch.TraceError, "Invalid ConfigDataStream message length");
                throw new InvalidOperationException("Invalid ConfigDataStream message length");
            }
        }
    }

    private void SetFile(Stream stream, string filerKey, string fileName)
    {
        FileReceived?.Invoke(this, new FileReceivedEventArgs(fileKey, fileName, stream));

        Tracer.TraceStream(stream, fileName);

        stream.Position = 0;
        var reader = new StreamReader(stream);
        string? line = reader.ReadLine();
        switch (line)
        {
        case "[lokomotive]":
            SetLocomotives(CsSerializer.Deserialize<LocomotiveData>(stream));
            break;
        case "[magnetartikel]":
            SetArticles(CsSerializer.Deserialize<ArticleData>(stream));
            break;
        case "[fahrstrassen]":
            SetRoutes(CsSerializer.Deserialize<RouteData>(stream));
            break;
        case "[gleisbild]":
            SetTrackData(CsSerializer.Deserialize<TrackData>(stream));
            break;
        case "[gleisbildseite]":
            SetTrackPageData(CsSerializer.Deserialize<TrackPageData>(stream));
            break;
        }
    }

    #region Status

    public SystemStatus Status { get; private set; } = CentralStationWebApi.SystemStatus.Default;

    private void SetStatus(SystemStatus status)
    {
        if (Status != status)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Status)));
            Status = status;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    private void HandleStatus(CANMessage message)
    {
        if (message.Command == Command.SystemCommand && message.Device == CentralStationBasic.AllDevices)
        {
            if (message.SubCommand == SubCommand.Stop)
            {
                SetStatus(CentralStationWebApi.SystemStatus.Stop);
            }
            else if (message.SubCommand == SubCommand.Go)
            {
                SetStatus(CentralStationWebApi.SystemStatus.Go);

            }
        }
    }

    #endregion

    #region Locomotives

    private void HandleLocomotive(CANMessage msg)
    {
        switch (msg.Command)
        {
        case Command.SystemCommand:
            if (msg.SubCommand == SubCommand.LocoHalt && msg.IsResponse && msg.DataLength == 5)
            {
                LocomotiveHalt?.Invoke(this, new LocomotiveEventArgs(msg.Device));
            }
            break;
        case Command.LocoVelocity:
            if (msg.IsResponse && msg.DataLength == 6)
            {
                LocomotiveVelocity?.Invoke(this, new LocomotiveVelocityEventArgs(msg.Device, Math.Min(MaxVelocity, msg.GetDataUShort(4))));
            }
            break;
        case Command.LocoDirection:
            if (msg.IsResponse && msg.DataLength == 5)
            {
                LocomotiveDirection?.Invoke(this, new LocomotiveDirectionEventArgs(msg.Device, (DirectionChange)msg.GetDataByte(4)));
            }
            break;
        case Command.LocoFunction:
            if (msg.IsResponse && (msg.DataLength == 6 || msg.DataLength == 8))
            {
                LocomotiveFunction?.Invoke(this, new LocomotiveFunctionEventArgs(msg.Device, msg.GetDataByte(4), msg.GetDataByte(5), msg.DataLength == 8 ? msg.GetDataUShort(6) : null));
            }
            break;
        }
    }

    public LocomotiveData? Locomotives;

    private void SetLocomotives(LocomotiveData locomotives)
    {
        DeviceCache.AddLocomotiveDevices(locomotives);
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Locomotives)));
        Locomotives = locomotives;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locomotives)));
    }

    #endregion

    #region Articles

    public ArticleData? Articles;

    private void SetArticles(ArticleData articles)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Articles)));
        Articles = articles;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Articles)));
    }


    #endregion

    #region Routes

    public RouteData? Routes;

    private void SetRoutes(RouteData routes)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Routes)));
        Routes = routes;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Routes)));
    }


    #endregion

    #region Tracks

    public void StartTrackCollector()
    {
        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "Track Request Start");
        // clear all existing data
        trackCollector.Clear();

        // TODO

        // start collecting track data
        isTrackCollectorRunning = true;
        trackCollectorThread.Next();
    }

    public TrackData? Tracks;

    //private void SetTracks(TrackData tracks)
    //{
    //    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Tracks)));
    //    Tracks = tracks;
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tracks)));
    //}

    public TrackPageData? TrackPages;

    //private void SetTrackPages(TrackPageData trackPages)
    //{
    //    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(TrackPages)));
    //    TrackPages = trackPages;
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackPages)));
    //}

    


    // track collector


    //private readonly BlockingCollection<T> queue = [];
    //private readonly AutoResetEvent autoResetEvent = new(false);
    //private readonly TimeSpan timeout;

    //public EventQueue(Action<T> action, TimeSpan timeout)
    //{
    //    this.action = action;
    //    this.timeout = timeout;
    //    thread = new Thread(WorkLoop) { Name = "EventQueueThread", IsBackground = true };

    private bool isTrackCollectorRunning = false;
    //private readonly DataCollector<TrackData> trackDataCollector = new();
    private readonly TrackCollector trackCollector = new();

  

    private void TrackCollectorWorkerLoop() 
    {
        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "  Track Loop");

        if (!isTrackCollectorRunning)
        {
            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
            return;
        }

        if (trackCollector.ShouldRequest(out var page))
        {
            if (page == 0)
            {
                Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "Track Request");
                RequestConfigDataTrackDiagram();
            } 
            else 
            {
                Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Reqest Page: {page}");
                RequestConfigDataTrackDiagramPage((int)page);
            }
        }
        else
        {
            // check if finished 
            if (trackCollector.IsFinished)
            {
                Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Request finished");
                isTrackCollectorRunning = false;
            }
            else
            {
                Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Nothing");
            }
        }
    }

    private void SetTrackData(TrackData trackData)
    {
        if (!isTrackCollectorRunning)
        {
            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
            return;
        }

        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Response");
        trackCollector.Add(trackData);
        trackCollectorThread.Next();
    }

    private void SetTrackPageData(TrackPageData trackPageData)
    {
        if (!isTrackCollectorRunning)
        {
            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
            return;
        }

        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Response Page {trackPageData.Page}");
        trackCollector.Add(trackPageData);
        trackCollectorThread.Next();
    }


    #endregion

    #region Controllers

    public void StartControllerCollection()
    {
        Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, "Controller Request Start");
        
        // clear all existing data
        controllerCollector.Clear();

        // start collecting track data
        isControllerCollectorRunning = true;
        RequestParticipants();

        // wait on all Command.SoftwareVersion messages to be handled
        Task.Run(() => { Thread.Sleep(5000); contrCollectorThread.Next(); });
    }

    public IEnumerable<Controller>? Controllers { get; private set; } = null;

    private void SetController(IEnumerable<Controller> controllers)
    {
        //if (!controllersDictionary.TryGetValue(controller.DeviceId, out Controller? value) && !controller.Equals(value))
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Controllers)));
            Controllers = controllers;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Controllers)));
        }
    }




    //private void UpdateController(Controller controller)
    //{
    //    if (!controllersDictionary.TryGetValue(controller.DeviceId, out Controller? value) && !controller.Equals(value))
    //    {
    //        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Controllers)));
    //        controllersDictionary[controller.DeviceId] = controller;
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Controllers)));
    //    }
    //}

    //private readonly BlockingCollection<T> queue = [];
    //private void StatusDataWorkerLoop()
    //{
    //    foreach (var item in queue.GetConsumingEnumerable())
    //    {
    //        try { action(item); }
    //        catch (Exception ex) { Debug.WriteLine(ex); }
    //    }
    //}

    private bool isControllerCollectorRunning = false;
    private readonly ControllerCollector controllerCollector = new();
    private DataCollector controllerDataCollector = new();

    //private readonly Dictionary<uint, Controller> controllersDictionary = [];

    private void ContrCollectorWorkerLoop()
    {
        Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, "  Controller Loop");

        if (!isControllerCollectorRunning)
        {
            Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"  Controller not running");
            return;
        }

        if (controllerCollector.ShouldRequest(out var controller))
        {
            Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"Controller Reqest Device: {controller.deviceId:X8} Page: {controller.index}");
            RequestStatusData(controller.deviceId, (byte)controller.index);
        }
    }

    private void HandleController(CANMessage msg)
    {
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            var controller = new Controller(msg, Host!);
            controllerCollector.Add(controller);

            // contrCollectorThread.Next() on timer 
        }
        if (msg.Command == Command.StatusData) // && msg.IsResponse)
        {
            switch (msg.DataLength)
            {
            case 5:
                Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 5 Device {msg.Device:X8} Index {msg.GetDataByte(4)}");
                break;
            case 6:
                Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 6 Device {msg.Device:X8} Index {msg.GetDataByte(4)} NumOfPackages {msg.GetDataByte(5)}");
                controllerCollector.Add(msg.Device, msg.GetDataByte(4), controllerDataCollector);
                break;
            case 8:
                ushort packageIndex = (byte)(msg.Hash & 0xff);
                Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 8 HashIndex {packageIndex}");
                if (packageIndex == 1)
                {
                    controllerDataCollector = new();
                }
                controllerDataCollector.AddData(msg.GetData());
                break;
            default:
                throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
            }
        }
    }

    //private Dictionary<uint, StatusDataDevice> statusData = [];
    //public List<StatusDataDevice> StatusData = [];
    //private StatusDataDevice? curStatusData;
    //private ushort nextStatusDataPackage = 1;

   

    //private void HandleStatusData(CANMessage msg)
    //{
       
    //}

    #endregion
}
