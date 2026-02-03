using CentralStationWebApi.Internal;
using System;
using System.Reflection;

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

    private readonly TimeSpan timeout = TimeSpan.FromSeconds(1000);
    //private readonly int retry = 3;

    internal static Uri GuiUri => new($"http://{Host}/images/gui/");
    internal static Uri MagUri => new($"http://{Host}/app/assets/mag/");
    internal static Uri LocoUri => new($"http://{Host}/app/assets/lok/");

    public TimeSpan MessageTimeout { get; set; } = new TimeSpan(0, 0, 0, 500);
    public TimeSpan DataTimeout { get; set; } = new TimeSpan(0, 0, 2);


    public CentralStation(string host, Protocol protocol = Protocol.TCP) : base(host, protocol)
    {
        statusDataEventQueue = new (tuple => StatusData(tuple.deviceId, tuple.index), TimeSpan.FromSeconds(10));
    }
        

    protected override void ReceiveHandler(CANMessage msg)
    {
        HandleSystem(msg);

        HandleSystemStatus(msg);

        HandleLocomotive(msg);
        HandleStatus(msg);
        //HandleStreams(msg);

        HandleConfigData(msg);
        //HandleController(msg);
        //HandleStatusData(msg);
        HandleDevices(msg);
        HandleDeviceInfo(msg);
    }

    #region System 

    private readonly Lock systemLock = new();
    private readonly AutoResetEvent systemEvent = new(false);
    private CANMessage? systemReqMsg;
    

    private void HandleSystem(CANMessage msg)
    {
        if (systemReqMsg is not null && 
            msg.Command == Command.SystemCommand &&
            msg.SubCommand == systemReqMsg!.SubCommand && 
            msg.DeviceId == systemReqMsg!.DeviceId && 
            msg.IsResponse)
        {
            systemEvent.Set();
        }
    }

    /// <summary>
    /// Stops the system asynchronously for the specified device.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to stop. Defaults to all devices.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> SystemStopAsync(uint deviceId = AllDevices)
        => await SystemCommandAsync(SubCommand.Stop, deviceId);
    
    public async Task<bool> SystemGoAsync(uint deviceId = AllDevices)
        => await SystemCommandAsync(SubCommand.Go, deviceId);

    public async Task<bool> SystemHaltAsync(uint deviceId = AllDevices)
        => await SystemCommandAsync(SubCommand.Halt, deviceId);

    public async Task<bool> SystemLocomotiveEmergencyHaltAsync(uint deviceId = AllDevices)
        => await SystemCommandAsync(SubCommand.LocoHalt, deviceId);

    public async Task<bool> SystemLocomotiveCycleStopAsync(uint deviceId = AllDevices)
        => await SystemCommandAsync(SubCommand.LocoCycleStop, deviceId);

    public async Task<bool> SystemLocomotiveDataProtocolAsync(uint deviceId = AllDevices, byte protocoll = 0xff)
        => await SystemCommandAsync(SubCommand.LocoDataProtocol, deviceId, protocoll);

    public async Task<bool> SystemArticleSwitchingTimeAsync(uint deviceId, ushort time = 0xff)
        => await SystemCommandAsync(SubCommand.SwitchingTime, deviceId, time);

    // SubCommand.FastRead not implemented

    public async Task<bool> SystemTrackProtocolSwitchAsync(uint deviceId, TrackProtocol protocoll)
        => await SystemCommandAsync(SubCommand.TrackProtocol, deviceId, (byte)protocoll);

    public async Task<bool> SystemMfxNewRegistrationCounterAsync(uint deviceId, TrackProtocol protocoll, ushort newRegistrationCounter)
        => await SystemCommandAsync(SubCommand.NewRegistrationCounter, deviceId, (byte)protocoll, newRegistrationCounter);

    // SubCommand.Overload not implemented only response from TFP/GFP

    // SubCommand.Status handled in HandleSystemStatus

    public async Task<bool> GetSystemIdentifierAsync(uint device)
         => await SystemCommandAsync(SubCommand.Identifier, deviceId);

    public async Task<bool> SetSystemIdentifierAsync(uint device, byte identifier)
        => await SystemCommandAsync(SubCommand.Identifier, deviceId);

    public async Task<bool> SystemResetAsync(uint deviceId, byte resetTarget)
        => await SystemCommandAsync(SubCommand.TrackProtocol, deviceId);


    private async Task<bool> SystemCommandAsync(SubCommand subCommand, uint deviceId)
    {
        return await Task.Run(() =>
        {
            lock (systemLock)
            {
                systemReqMsg = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
                    AddUInt32(deviceId).
                    AddSubCommand(subCommand);
                SendMessage(systemReqMsg);
                return systemEvent.WaitOne(MessageTimeout);
            }
        });
    }

    private async Task<bool> SystemCommandAsync(SubCommand subCommand, uint deviceId, byte para)
    {
        return await Task.Run(() =>
        {
            lock (systemLock)
            {
                systemReqMsg = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
                    AddUInt32(deviceId).
                    AddSubCommand(subCommand).
                    AddByte(para);
                SendMessage(systemReqMsg);
                return systemEvent.WaitOne(MessageTimeout);
            }
        });
    }

    private async Task<bool> SystemCommandAsync(SubCommand subCommand, uint deviceId, ushort para)
    {
        return await Task.Run(() =>
        {
            lock (systemLock)
            {
                systemReqMsg = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
                    AddUInt32(deviceId).
                    AddSubCommand(subCommand).
                    AddUInt16(para);
                SendMessage(systemReqMsg);
                return systemEvent.WaitOne(MessageTimeout);
            }
        });
    }

    private async Task<bool> SystemCommandAsync(SubCommand subCommand, uint deviceId, byte para1, ushort para2)
    {
        return await Task.Run(() =>
        {
            lock (systemLock)
            {
                systemReqMsg = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
                    AddUInt32(deviceId).
                    AddSubCommand(subCommand).
                    AddByte(para1).
                    AddUInt16(para2);
                SendMessage(systemReqMsg);
                return systemEvent.WaitOne(MessageTimeout);
            }
        });
    }

    #endregion

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
        if (message.Command == Command.SystemCommand && message.DeviceId == CentralStationBasic.AllDevices)
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

    #region SystemStatus

    private const int systemStatusTimeout = 500;
    private readonly Lock systemStatusLock = new();
    private readonly AutoResetEvent systemStatusEvent = new(false);
    private ushort? systemStatusValue = null;

    private void HandleSystemStatus(CANMessage msg)
    {
        if (msg.Command == Command.SystemCommand && msg.SubCommand == SubCommand.Status && msg.IsResponse)
        {
            switch (msg.DataLength)
            {
            // no result
            case 6: 
                systemStatusValue = null;
                break;
            // bool result
            case 7: 
                systemStatusValue = msg.GetDataByte(6);
                break;
            // ushort value result
            case 8:
                systemStatusValue = msg.GetDataUShort(6);
                break;
            default:
                throw new InvalidDataException();
            }
        }
    }
    public async Task<ushort?> GetSystemStatusAsync(uint device, byte channel, ushort? value = null)
    {
        return await Task.Run(() =>
        {
            lock (systemStatusLock)
            {
                systemStatusValue = null;
                SystemStatus(device, channel, value);
                systemStatusEvent.WaitOne(systemStatusTimeout);
                return systemStatusValue;
            }
        });
    }

    #endregion

    #region ConfigData

    private const int configDataTimeout = 2000;
    private readonly Lock configDataLock = new();
    private readonly AutoResetEvent configDataEvent = new(false);
    private string? configDataFileName = null;
    private FileCollector? configDataFileCollector = null;

    private void HandleConfigData(CANMessage msg)
    {
        if (msg.Command == Command.ConfigData && msg.IsResponse)
        {
            configDataFileName = msg.GetDataString().Trim('\0');
        }
        // hash compare: the message is for us 
        if (msg.Command == Command.ConfigDataStream && !msg.IsResponse && msg.Hash == hash)
        {
            if (msg.DataLength == 6)
            {
                configDataFileCollector = new FileCollector(CSFileStreamMode.Request, configDataFileName!, msg.GetDataUInt(0), msg.GetDataUShort(4));
            }
            else if (msg.DataLength == 7)
            {
                configDataFileCollector = new FileCollector(CSFileStreamMode.Broadcast, configDataFileName!, msg.GetDataUInt(0), msg.GetDataUShort(4), msg.GetDataByte(6));
            }
            else if (msg.DataLength == 8 )
            {
                configDataFileCollector!.AddData(msg.GetData());
                if (configDataFileCollector.IsReady())
                {
                    configDataEvent.Set();
                    //fileReceivedQueue.Add(fileStream);
                    //SetFile(configDataFileCollector.GetFileStream(), configDataFileCollector.FileKey, configDataFileCollector.FileName);
                }
            }
            else
            {
                Debug.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} ERROR: Invalid ConfigDataStream message length");
            }
        }
    }

    public async Task<Stream?> GetConfigDataAsync(string filename)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(filename.Length, 8, nameof(filename));

        return await Task.Run(() =>
        {
            lock (configDataLock)
            {
                DebugConfigData($"GetConfigDataAsync++");

                ConfigData(filename);

                bool success = configDataEvent.WaitOne(configDataTimeout);
                if (success)
                {
                    var mem = new MemoryStream();
                    //configDataFileCollector?.GetFileStream().CopyTo(mem);
                    configDataFileCollector?.CopyTo(mem);
                    DebugConfigData($"GetConfigDataAsync--");
                    mem.Position = 0;
                    return mem;
                }
                DebugConfigData($"GetConfigDataAsync-- NULL");
                return null; 
            }
        });
    }

    [Conditional("DEBUG")]
    private static void DebugConfigData(string text) => Debug.WriteLineIf(TraceSwitches.DevicesSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} Devices: {text}");


    #endregion

    #region Locomotives

    private void HandleLocomotive(CANMessage msg)
    {
        switch (msg.Command)
        {
        case Command.SystemCommand:
            if (msg.SubCommand == SubCommand.LocoHalt && msg.IsResponse && msg.DataLength == 5)
            {
                LocomotiveHalt?.Invoke(this, new LocomotiveEventArgs(msg.DeviceId));
            }
            break;
        case Command.LocoVelocity:
            if (msg.IsResponse && msg.DataLength == 6)
            {
                LocomotiveVelocity?.Invoke(this, new LocomotiveVelocityEventArgs(msg.DeviceId, Math.Min(MaxVelocity, msg.GetDataUShort(4))));
            }
            break;
        case Command.LocoDirection:
            if (msg.IsResponse && msg.DataLength == 5)
            {
                LocomotiveDirection?.Invoke(this, new LocomotiveDirectionEventArgs(msg.DeviceId, (DirectionChange)msg.GetDataByte(4)));
            }
            break;
        case Command.LocoFunction:
            if (msg.IsResponse && (msg.DataLength == 6 || msg.DataLength == 8))
            {
                LocomotiveFunction?.Invoke(this, new LocomotiveFunctionEventArgs(msg.DeviceId, msg.GetDataByte(4), msg.GetDataByte(5), msg.DataLength == 8 ? msg.GetDataUShort(6) : null));
            }
            break;
        }
    }

    //public LocomotiveData? Locomotives;

    //private void SetLocomotives(LocomotiveData locomotives)
    //{
    //    DeviceCache.AddLocomotiveDevices(locomotives);
    //    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Locomotives)));
    //    Locomotives = locomotives;
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locomotives)));
    //}

    #endregion

    #region Articles

    //public ArticleData? Articles;

    //private void SetArticles(ArticleData articles)
    //{
    //    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Articles)));
    //    Articles = articles;
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Articles)));
    //}


    #endregion

    #region Routes

    //public RouteData? Routes;

    //private void SetRoutes(RouteData routes)
    //{
    //    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Routes)));
    //    Routes = routes;
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Routes)));
    //}


    #endregion

    #region Tracks

    //public void StartTrackCollector()
    //{
    //    Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "Track Request Start");
    //    // clear all existing data
    //    trackCollector.Clear();

    //    // TODO

    //    // start collecting track data
    //    isTrackCollectorRunning = true;
    //    trackCollectorThread.Next();
    //}

    //public TrackData? Tracks;

    ////private void SetTracks(TrackData tracks)
    ////{
    ////    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Tracks)));
    ////    Tracks = tracks;
    ////    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tracks)));
    ////}

    //public TrackPageData? TrackPages;

    ////private void SetTrackPages(TrackPageData trackPages)
    ////{
    ////    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(TrackPages)));
    ////    TrackPages = trackPages;
    ////    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackPages)));
    ////}

    


    // track collector


    //private readonly BlockingCollection<T> queue = [];
    //private readonly AutoResetEvent autoResetEvent = new(false);
    //private readonly TimeSpan timeout;

    //public EventQueue(Action<T> action, TimeSpan timeout)
    //{
    //    this.action = action;
    //    this.timeout = timeout;
    //    thread = new Thread(WorkLoop) { Name = "EventQueueThread", IsBackground = true };

    //private bool isTrackCollectorRunning = false;
    ////private readonly DataCollector<TrackData> trackDataCollector = new();
    //private readonly TrackCollector trackCollector = new();

  

    //private void TrackCollectorWorkerLoop() 
    //{
    //    Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "  Track Loop");

    //    if (!isTrackCollectorRunning)
    //    {
    //        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
    //        return;
    //    }

    //    if (trackCollector.ShouldRequest(out var page))
    //    {
    //        if (page == 0)
    //        {
    //            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, "Track Request");
    //            ConfigDataTrackDiagram();
    //        } 
    //        else 
    //        {
    //            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Reqest Page: {page}");
    //            ConfigDataTrackDiagramPage((int)page);
    //        }
    //    }
    //    else
    //    {
    //        // check if finished 
    //        if (trackCollector.IsFinished)
    //        {
    //            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Request finished");
    //            isTrackCollectorRunning = false;
    //        }
    //        else
    //        {
    //            Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Nothing");
    //        }
    //    }
    //}

    //private void SetTrackData(TrackData trackData)
    //{
    //    if (!isTrackCollectorRunning)
    //    {
    //        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
    //        return;
    //    }

    //    Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Response");
    //    trackCollector.Add(trackData);
    //    trackCollectorThread.Next();
    //}

    //private void SetTrackPageData(TrackPageData trackPageData)
    //{
    //    if (!isTrackCollectorRunning)
    //    {
    //        Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"  Track not running");
    //        return;
    //    }

    //    Debug.WriteLineIf(TraceSwitches.TracksSwitch.TraceInfo, $"Track Response Page {trackPageData.Page}");
    //    trackCollector.Add(trackPageData);
    //    trackCollectorThread.Next();
    //}


    #endregion

    #region Controllers

    //public void StartControllerCollection()
    //{
    //    Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, "Controller Request Start");

    //    // clear all existing data
    //    controllerCollector.Clear();

    //    // start collecting track data
    //    isControllerCollectorRunning = true;
    //    SoftwareVersion();

    //    // wait on all Command.SoftwareVersion messages to be handled
    //    Task.Run(() => { Thread.Sleep(5000); contrCollectorThread.Next(); });
    //}

    //public IEnumerable<Controller>? Controllers { get; private set; } = null;

    //private void SetController(IEnumerable<Controller> controllers)
    //{
    //    //if (!controllersDictionary.TryGetValue(controller.DeviceId, out Controller? value) && !controller.Equals(value))
    //    {
    //        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Controllers)));
    //        Controllers = controllers;
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Controllers)));
    //    }
    //}




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

    //private bool isControllerCollectorRunning = false;
    //private readonly ControllerCollector controllerCollector = new();
    //private DataCollector controllerDataCollector = new();

    //private readonly Dictionary<uint, Controller> controllersDictionary = [];

    //private void ContrCollectorWorkerLoop()
    //{
    //    Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, "  Controller Loop");

    //    if (!isControllerCollectorRunning)
    //    {
    //        Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"  Controller not running");
    //        return;
    //    }

    //    if (controllerCollector.ShouldRequest(out var controller))
    //    {
    //        Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"Controller Reqest DeviceId: {controller.deviceId:X8} Page: {controller.index}");
    //        StatusData(controller.deviceId, (byte)controller.index);
    //    }
    //}

    //private void HandleController(CANMessage msg)
    //{
    //    if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
    //    {
    //        var controller = new Controller(msg, Host!);
    //        controllerCollector.Add(controller);

    //        // contrCollectorThread.Next() on timer 
    //    }
    //    if (msg.Command == Command.StatusData) // && msg.IsResponse)
    //    {
    //        switch (msg.DataLength)
    //        {
    //        case 5:
    //            Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 5 DeviceId {msg.DeviceId:X8} Index {msg.GetDataByte(4)}");
    //            break;
    //        case 6:
    //            Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 6 DeviceId {msg.DeviceId:X8} Index {msg.GetDataByte(4)} NumOfPackages {msg.GetDataByte(5)}");
    //            controllerCollector.Add(msg.DeviceId, msg.GetDataByte(4), controllerDataCollector);
    //            break;
    //        case 8:
    //            ushort packageIndex = (byte)(msg.Hash & 0xff);
    //            Debug.WriteLineIf(TraceSwitches.ControllerSwitch.TraceInfo, $"HandleStatusData Length 8 HashIndex {packageIndex}");
    //            if (packageIndex == 1)
    //            {
    //                controllerDataCollector = new();
    //            }
    //            controllerDataCollector.AddData(msg.GetData());
    //            break;
    //        default:
    //            throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
    //        }
    //    }
    //}

    //private Dictionary<uint, StatusDataDevice> statusData = [];
    //public List<StatusDataDevice> StatusData = [];
    //private StatusDataDevice? curStatusData;
    //private ushort nextStatusDataPackage = 1;



    //private void HandleStatusData(CANMessage msg)
    //{

    //}

    #endregion

    #region Devices


    private const int devicesTimeout = 500;
    private Dictionary<uint, Device>? devices = null;

    //    private AutoResetEvent devicesEvent = new(false);

    private void HandleDevices(CANMessage msg)
    {
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            if (devices != null && !devices.ContainsKey(msg.DeviceId))
            {
                var device = new Device(msg);
                DebugDevices($"--> SoftwareVersion {device.DeviceId:X8} {device.Version} {device.DeviceType}");
                devices?.Add(device.DeviceId, device);
            }
        }
    }

    public async Task<List<Device>?> GetDevicesAsync()
    {
        return await Task.Run(async () =>
        {
            DebugDevices($"GetDevicesAsync++");
         
            devices = [];
            SoftwareVersion();
            await Task.Delay(devicesTimeout);

            var res = devices.Values.ToList();
            devices = null;
            DebugDevices($"GetDevicesAsync--");
            return res; 
        });
    }

    #endregion

    #region DeviceInfo

    private const int deviceInfoTimeout = 500;
    private readonly AutoResetEvent deviceInfoEvent = new(false);
    private DeviceInfo? deviceInfo = null;

    private const int deviceMeasurementTimeout = 500;
    private readonly AutoResetEvent deviceMeasurementEvent = new(false);
    private DeviceMeasurement? deviceMeasurement = null;

    private DataCollector? deviceDataCollector = null;
    private readonly Lock deviceLock = new Lock();

    private void HandleDeviceInfo(CANMessage msg)
    {
        if (msg.Command == Command.StatusData && msg.IsResponse)
        {
            switch (msg.DataLength)
            {
            case 5:
                DebugDevices($"HandleStatusData Length 5 DeviceId {msg.DeviceId:X8} Index {msg.GetDataByte(4)}");
                break;
            case 6:
                DebugDevices($"HandleStatusData Length 6 DeviceId {msg.DeviceId:X8} Index {msg.GetDataByte(4)} NumOfPackages {msg.GetDataByte(5)}");
                int index = msg.GetDataByte(4);
                int packages = msg.GetDataByte(5);
                if (index == 0)
                {
                    deviceInfo = new(deviceDataCollector!);
                    deviceInfoEvent.Set();
                }
                else
                {
                    deviceMeasurement = new(deviceDataCollector!);
                    deviceMeasurementEvent.Set();
                }
                break;
            case 8:
                ushort packageIndex = (byte)(msg.Hash & 0xff);
                DebugDevices($"HandleStatusData Length 8 HashIndex {packageIndex}");
                if (packageIndex == 1)
                {
                    deviceDataCollector = new();
                }
                deviceDataCollector?.AddData(msg.GetData());
                break;
            default:
                throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
            }
        }
    }

    public async Task<DeviceInfo?> GetDeviceInfoAsync(uint deviceId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(deviceId, nameof(deviceId));

        return await Task.Run(() =>
        {
            lock (deviceLock)
            {
                DebugDevices($"GetDeviceInfoAsync++");

                deviceInfoEvent.Reset();
                StatusData(deviceId, 0);
                bool success = deviceInfoEvent.WaitOne(deviceInfoTimeout);
                DebugDevices($"deviceInfoEvent fired {success}");

                var res = deviceInfo;
                deviceInfo = null;
                DebugDevices($"GetDeviceInfoAsync--");
                return res;
            }
        });
    }

    public async Task<DeviceMeasurement?> GetDeviceMeasurementAsync(uint deviceId, byte index)
    {
        ArgumentOutOfRangeException.ThrowIfZero(deviceId, nameof(deviceId));
        ArgumentOutOfRangeException.ThrowIfZero(index, nameof(index));

        return await Task.Run(() =>
        {
            lock (deviceLock)
            {
                DebugDevices($"GetDeviceMeasurementAsync++");

                deviceMeasurementEvent.Reset();
                StatusData(deviceId, index);
                bool success = deviceMeasurementEvent.WaitOne(deviceMeasurementTimeout);
                DebugDevices($"deviceMeasurementEvent fired {success}");

                var res = deviceMeasurement;
                deviceMeasurement = null;
                DebugDevices($"GetDeviceMeasurementAsync--");
                return res;
            }
        });
    }



    [Conditional("DEBUG")]
    private static void DebugDevices(string text) => Debug.WriteLineIf(TraceSwitches.DevicesSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} Devices: {text}");

    #endregion
}
