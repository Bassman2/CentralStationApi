namespace CentralStationWebApi;

public class CentralStation : CentralStationBasic, INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    private readonly EventQueue<(uint deviceId, byte index)> statusDataEventQueue;

    private readonly CollectorThread trackCollectorThread;

    private readonly TimeSpan timeout = TimeSpan.FromSeconds(1000);
    private readonly int retry = 3;

    public CentralStation(string host, Protocol protocol = Protocol.TCP) : base(host, protocol)
    {
        statusDataEventQueue = new (tuple => RequestStatusData(tuple.deviceId, tuple.index), TimeSpan.FromSeconds(10));

        trackCollectorThread = new CollectorThread(TrackCollectorWorkerLoop, timeout);
    }
        

    protected override void ReceiveHandler(CANMessage msg)
    {
        HandleStatus(msg);
        HandleStreams(msg);
        HandleController(msg);
        //HandleStatusData(msg);
    }

    private readonly Dictionary<ushort, CSFileStream> fileDictionary = [];
    private string filename = "empty";

    private void HandleStreams(CANMessage msg)
    {

        if (msg.IsResponse && msg.Command == Command.RequestConfigData)
        {
            filename = msg.GetDataString().Trim('\0');
        }
        if (/*msg.IsResponse && */ msg.Command == Command.ConfigDataStream)
        {
            if (msg.DataLength == 6)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Request, filename, msg.GetDataUInt(0), msg.GetDataUShort(0));
            }
            else if (msg.DataLength == 7)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Broadcast, filename, msg.GetDataUInt(0), msg.GetDataUShort(4), msg.GetDataByte(6));
            }
            else if (msg.DataLength == 8 && fileDictionary.TryGetValue(msg.Hash, out var fileStream))
            {
                if (fileStream.AddData(msg.GetData()))
                {
                    //fileReceivedQueue.Add(fileStream);
                    SetFile(fileStream.GetFileStream(), fileStream.FileName);
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

    private void SetFile(Stream stream, string fileName)
    {

        Tracer.TraceStream(stream, fileName);

        stream.Position = 0;
        var reader = new StreamReader(stream);
        string? line = reader.ReadLine();
        switch (line)
        {
        case "[lokomotive]":
            SetLocomotives(CsSerializer.Deserialize<LocomotiveData>(stream, "[lokomotive]"));
            break;
        case "[magnetartikel]":
            SetArticles(CsSerializer.Deserialize<ArticleData>(stream, "[magnetartikel]"));
            break;
        case "[fahrstrassen]":
            SetRoutes(CsSerializer.Deserialize<RouteData>(stream, "[fahrstrassen]"));
            break;
        case "[gleisbild]":
            SetTrackData(CsSerializer.Deserialize<TrackData>(stream, "[gleisbild]"));
            break;
        case "[gleisbildseite]":
            SetTrackPageData(CsSerializer.Deserialize<TrackPageData>(stream, "[gleisbildseite]"));
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
    private DataCollector<TrackData> trackDataCollector = new();
    private DictionaryCollector<uint, TrackPageData> trackPagesDataCollector = new();

  

    public void StartTrackCollector()
    {
        // clear all existing data
        trackDataCollector.Clear();
        trackPagesDataCollector.Clear();

        // TODO

        // start collecting track data
        isTrackCollectorRunning = true;
        trackCollectorThread.Next();
    }

    private void TrackCollectorWorkerLoop() //object? obj)
    {
        if (!isTrackCollectorRunning) return;

        if (trackDataCollector.ShouldRequest)
        {            
            RequestConfigDataTrackDiagram();
        }
        else if (trackPagesDataCollector.ShouldRequest(out var page))
        {
            RequestConfigDataTrackDiagramPage((int)page);
        }

        // check if finished 
        if (trackDataCollector.IsFinished && trackPagesDataCollector.IsFinished)
        {
            isTrackCollectorRunning = false;
        }
    }

    private void SetTrackData(TrackData trackData)
    { 
        if (!isTrackCollectorRunning) return;

        intTrackData = trackData;
        intTrackPagesData = [];
        intTrackPagesData = (intTrackData.Pages ?? []).ToDictionary(page => page.Id, _ => (TrackPageData?)null);


        //foreach (var page in intTrackData.Pages!)
        //{
        //    intTrackPagesData[page!.Id] = null;
        //}
        trackCollectorThread.Next();
    }

    private void SetTrackPageData(TrackPageData trackPageData)
    {
        if (!isTrackCollectorRunning) return;

        if (intTrackPagesData != null)
        {
            intTrackPagesData[trackPageData.Page] = trackPageData;
        }
        trackCollectorThread.Next();
    }


    #endregion

    #region Controllers

    public IEnumerable<Controller> Controllers => controllersDictionary.Values;

    private readonly Dictionary<uint, Controller> controllersDictionary = [];

    private void SetController(Controller controller)
    {
        if (!controllersDictionary.TryGetValue(controller.DeviceId, out Controller? value) && !controller.Equals(value))
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Controllers)));
            controllersDictionary[controller.DeviceId] = controller;
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

    private DataCollector statusDataCollector = new();

    private void HandleController(CANMessage msg)
    {
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            var controller = new Controller(msg, Host!);
            SetController(controller);

            // request additional data if not already happened
            //if (controllersDictionary.TryGetValue(msg.Device, out var existingController))
            //{
            //    statusDataEventQueue.Add((msg.Device, 0));
            //}
        }
        //if (msg.Command == Command.StatusData) // && msg.IsResponse)
        //{
        //    switch (msg.DataLength)
        //    {
        //    case 5:
        //        Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 5 Device {msg.Device:X8} Index {msg.GetDataByte(4)}");
        //        break;
        //    case 6:
        //        byte index = msg.GetDataByte(4);
        //        Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 6 Device {msg.Device:X8} Index {msg.GetDataByte(4)} NumOfPackages {msg.GetDataByte(5)}");

        //        if (index == 0)
        //        {
        //            if (controllersDictionary.TryGetValue(msg.Device, out var existingController))
        //            {
        //                StatusDataDevice statusDataDevice = new(msg.Device, msg.GetDataByte(4), msg.GetDataByte(5), statusDataCollector);

        //                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Controllers)));
        //                existingController.Update(statusDataDevice);
        //                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Controllers)));
        //                statusDataEventQueue.Continue();
        //            }
        //        }
        //        else
        //        {
        //            //StatusDataValue statusDataValue = new(msg.Device, msg.GetDataByte(4), msg.GetDataByte(5), statusDataCollector);
        //            //PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(StatusData)));
        //            //StatusData.First(d => d.DeviceId == msg.Device).Values[index - 1] = statusDataValue;
        //            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusData)));
        //        }
        //        break;
        //    case 8:
        //        ushort packageIndex = (byte)(msg.Hash & 0xff);
        //        Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 8 HashIndex {packageIndex}");
        //        if (packageIndex == 1)
        //        {
        //            statusDataCollector = new();
        //        }
        //        statusDataCollector.AddData(msg.GetData());
        //        break;
        //    default:
        //        throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
        //    }
        //}
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
