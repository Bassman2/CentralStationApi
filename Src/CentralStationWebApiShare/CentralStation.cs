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

    private readonly CanMessageHandler canMessageHandler; 

    //private readonly int retry = 3;

    internal static Uri GuiUri => new($"http://{Host}/images/gui/");
    internal static Uri MagUri => new($"http://{Host}/app/assets/mag/");
    internal static Uri LocoUri => new($"http://{Host}/app/assets/lok/");

    public TimeSpan MessageTimeout { get; set; } = new TimeSpan(0, 0, 0, 500);
    public TimeSpan DataTimeout { get; set; } = new TimeSpan(0, 0, 2);


    public CentralStation(string host, Protocol protocol = Protocol.TCP) : base(host, protocol)
    {
        canMessageHandler = new CanMessageHandler(this);
        statusDataEventQueue = new (tuple => StatusData(tuple.deviceId, tuple.index), TimeSpan.FromSeconds(10));
    }
        

    protected override void ReceiveHandler(CanMessage msg)
    {
        //HandleSystem(msg);

        HandleSystemStatus(msg);

        HandleLocomotive(msg);
        HandleStatus(msg);
        //HandleStreams(msg);

        HandleConfigData(msg);
        //HandleController(msg);
        //HandleStatusData(msg);
        HandleDevices(msg);
        HandleDeviceInfo(msg);

        canMessageHandler.OnResponseReceived(msg);
    }

    #region System 

    public async Task<bool> SystemStopAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Stop);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemGoAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Go);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemHaltAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Halt);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemLocomotiveEmergencyHaltAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoHalt);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemLocomotiveCycleStopAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoCycleStop);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemLocomotiveDataProtocolAsync(uint deviceId = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoDataProtocol).AddByte(protocoll);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemArticleSwitchingTimeAsync(uint deviceId = AllDevices, ushort time = 0xff, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.SwitchingTime).AddUInt16(time);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    // SubCommand.FastRead not implemented

    public async Task<bool> SystemTrackProtocolAsync(uint deviceId, TrackProtocol protocoll, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.TrackProtocol).AddByte((byte)protocoll);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemMfxNewRegistrationCounterAsync(uint deviceId, TrackProtocol protocoll, ushort newRegistrationCounter, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.NewRegistrationCounter).AddByte((byte)protocoll).AddUInt16(newRegistrationCounter);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    // SubCommand.Overload not implemented only response from TFP/GFP

    // SubCommand.Status handled in HandleSystemStatus

    public async Task<bool> GetSystemIdentifierAsync(uint deviceId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Identifier);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SetSystemIdentifierAsync(uint deviceId, byte identifier, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Identifier).AddByte(identifier);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    public async Task<bool> SystemResetAsync(uint deviceId, byte resetTarge, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Reset).AddByte(resetTarge);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    #endregion

    #region Administration

    public async Task<DirectionChange?> GetLocomotiveDirectionAsync(uint locoId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoDirection, hash).AddUInt32(locoId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return (DirectionChange?)res?.GetDataByte(4) ?? null;
    }

    public async Task<DirectionChange?> SetLocomotiveDirectionAsync(uint locoId, DirectionChange direction, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte((byte)direction);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return (DirectionChange?)res?.GetDataByte(4) ?? null;
    }

    public async Task<byte?> GetLocomotiveFunctionAsync(uint locoId, byte function, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    public async Task<byte?> SetLocomotiveFunctionAsync(uint locoId, byte function, byte value, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function).AddByte(value);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    public async Task<byte?> SetLocomotiveFunctionAsync(uint locoId, byte function, byte value, ushort functionValue, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function).AddByte(value).AddUInt16(functionValue);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    public async Task<ushort?> GetLocomotiveVelocityAsync(uint locoId, ushort velocity, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(velocity, MaxVelocity, nameof(velocity));

        var req = new CanMessage(Priority.Prio1, Command.LocoVelocity, hash).AddUInt32(locoId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    public async Task<ushort?> SetLocomotiveVelocityAsync(uint locoId, ushort velocity, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(velocity, MaxVelocity, nameof(velocity));

        var req = new CanMessage(Priority.Prio1, Command.LocoVelocity, hash).AddUInt32(locoId).AddUInt16(velocity);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
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

    private void HandleStatus(CanMessage message)
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

    private void HandleSystemStatus(CanMessage msg)
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

    private void HandleConfigData(CanMessage msg)
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

    private void HandleLocomotive(CanMessage msg)
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

    #endregion

    #region Devices


    private const int devicesTimeout = 500;
    private Dictionary<uint, Device>? devices = null;

    //    private AutoResetEvent devicesEvent = new(false);

    private void HandleDevices(CanMessage msg)
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

    private void HandleDeviceInfo(CanMessage msg)
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
