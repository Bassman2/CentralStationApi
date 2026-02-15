namespace CentralStationWebApi;

public sealed class CentralStation : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    private readonly IProtocolHandler client;
    private readonly CanMessageHandler canMessageHandler;
    private readonly MessageQueue<CanMessage> messageReceivedQueue;
    private readonly Task receiver;
    private readonly ushort hash;

    public const uint AllDevices = 0x0000;
    public const ushort MinVelocity = 0;
    public const ushort MaxVelocity = 1000;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<LocomotiveEventArgs>? LocomotiveHalt;
    public event EventHandler<LocomotiveVelocityEventArgs>? LocomotiveVelocity;
    public event EventHandler<LocomotiveDirectionEventArgs>? LocomotiveDirection;
    public event EventHandler<LocomotiveFunctionEventArgs>? LocomotiveFunction;

    internal static Uri GuiUri => new($"http://{Host}/images/gui/");
    internal static Uri MagUri => new($"http://{Host}/app/assets/mag/");
    internal static Uri LocoUri => new($"http://{Host}/app/assets/lok/");

    public static string? Host { get; private set; }
    public DeviceData Device { get; }

    public TimeSpan MessageTimeout
    {   
        get => canMessageHandler.MessageTimeout;
        set => canMessageHandler.MessageTimeout = value; 
    }

    public TimeSpan CollectionTimeout
    {
        get => canMessageHandler.CollectionTimeout;
        set => canMessageHandler.CollectionTimeout = value;
    }
    
    public CentralStation(string host, Protocol protocol = Protocol.TCP, DeviceData? device = null) 
    {
        ArgumentNullException.ThrowIfNullOrEmpty(host);
        ArgumentOutOfRangeException.ThrowIfZero((int)Uri.CheckHostName(host), host);

        canMessageHandler = new CanMessageHandler(this);

        Host = host;
        Device = device ?? new DeviceData(0x6D554711, new Version(1, 0));
        hash = DeviceId2Hash(Device.DeviceId);

        client = protocol switch
        {
            Protocol.TCP => new TcpHandler(),
            Protocol.UDP => new UdpHandler(),
            _ => throw new NotSupportedException($"Protocol {protocol} not supported"),
        };

        client.Connect(host);

        messageReceivedQueue = new MessageQueue<CanMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        this.receiver = Task.Run(async () => await ReceiveAsync());
        HashCache.AddHash((ushort)hash, "PCApp");  
        //statusDataEventQueue = new (tuple => StatusData(tuple.deviceId, tuple.index), TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
        client.Dispose();

        messageReceivedQueue.Dispose();
    }

    #region Send Message

    internal void SendMessage(CanMessage msg)
    {
        messageReceivedQueue.Add(msg);
        client.Send(msg);
        Tracer.TraceMessage(msg);
    }

    internal async Task SendMessageAsync(CanMessage msg)
    {
        messageReceivedQueue.Add(msg);
        await client.SendAsync(msg);
        Tracer.TraceMessage(msg);
    }

    #endregion

    #region Receive Message

    private async Task ReceiveAsync()
    {
        try
        {
            while (true)
            {
                var msg = await client.ReceiveAsync();

                Tracer.TraceMessage(msg);
                Debug.WriteLineIf(TraceSwitches.CanReceiveSwitch.TraceInfo, $"Received: {msg}");
                messageReceivedQueue.Add(msg);

                if (msg.Command == Command.SoftwareVersion && msg.IsResponse == false)
                {
                    var res = new CanMessage(Priority.Prio1, Command.SoftwareVersion, hash, true).AddUInt32(Device.DeviceId).AddByte((byte)Device.Version.Major).AddByte((byte)Device.Version.Minor).AddUInt16((ushort)DeviceType.Application);
                    SendMessage(res);
                }
                if (msg.Command == Command.StatusData && msg.IsResponse == false && msg.DeviceId == Device.DeviceId && msg.IsResponse == false)
                {
                    byte num = 2;
                    var res = new CanMessage(Priority.Prio1, Command.StatusData, 0x0301, true).AddUInt32(0).AddUInt32(Device.SerialNumber);
                    SendMessage(res);

                    res = new CanMessage(Priority.Prio1, Command.StatusData, 0x0302, true).AddString(Device.ArticleNumber);
                    SendMessage(res);

                    string name = Device.DeviceName;
                    while (!string.IsNullOrEmpty(name))
                    {
                        res = new CanMessage(Priority.Prio1, Command.StatusData, 0x0300, true).AddString(name.Substring(0, Math.Min(name.Length, 8)));
                        SendMessage(res);
                        num++;
                        name = name.Length > 8 ? name.Substring(8) : string.Empty;
                    }

                    res = new CanMessage(Priority.Prio1, Command.StatusData, hash, true).AddUInt32(Device.DeviceId).AddByte(0).AddByte(num);
                    SendMessage(res);
                }

                ReceiveHandler(msg);
            }
        }
        catch (ObjectDisposedException)
        {
            // expected on disposal
        }
        catch (Exception ex)
        {
            Debug.WriteLineIf(TraceSwitches.CanReceiveSwitch.TraceError, $"Receiver error: {ex}");
        }
    }

    private void ReceiveHandler(CanMessage msg)
    {
        HandleStatus(msg);
        HandleLocomotive(msg);
        canMessageHandler.OnResponseReceived(msg);
    }

    #endregion

    #region Status Events

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
        if (message.Command == Command.SystemCommand && message.DeviceId == AllDevices)
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

    #region 2 System Commands

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


    public async Task<ushort?> GetSystemStatusAsync(uint deviceId, byte channel, ushort? configurationValue = null, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Status).AddByte(channel).AddUInt16(configurationValue);  
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.DataLength switch
        {
            6 => null,                  // no result
            7 => res.GetDataByte(6),    // bool result
            8 => res.GetDataUShort(6),  // ushort value result
            _ => throw new InvalidDataException($"GetSystemStatusAsync: unexpected DataLength {res?.DataLength} in response")
        };
    }

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

    #region 3 Administration

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

    #region 4 Article Commands

    public async Task<bool> SetArticleSwitchAsync(uint localId, byte standing, byte power, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SwitchAccessories, hash).AddUInt32(localId).AddByte(standing).AddByte(power);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res != null;
    }

    public async Task<bool> SetArticleSwitchAsync(uint localId, byte standing, byte power, ushort time, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SwitchAccessories, hash).AddUInt32(localId).AddByte(standing).AddByte(power).AddUInt16(time);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res != null;
    }

    #endregion

    #region 5 Feedback

    // obsolete
    public async Task<ushort?> GetS88PollingAsync(uint deviceId, byte module, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Polling, hash).AddUInt32(deviceId).AddByte(module);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataUShort(5) ?? null; 
    }

    public async Task<S88Event?> GetS88EventAsync(ushort deviceId, ushort contactId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Event, hash).AddUInt16(deviceId).AddUInt16(contactId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is null ? null : new S88Event() { OldValue = res.GetDataByte(4), NewValue = res.GetDataByte(5), Time = res.GetDataUShort(6) };
    }

    public async Task<S88Event?> RegisterS88EventAsync(ushort deviceId, ushort contactId, byte parameter, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Event, hash).AddUInt16(deviceId).AddUInt16(contactId).AddByte(parameter);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is null ? null : new S88Event() { OldValue = res.GetDataByte(4), NewValue = res.GetDataByte(5), Time = res.GetDataUShort(6) };
    }

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    public async Task<List<Device>?> GetAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SoftwareVersion, hash);
        return await canMessageHandler.SendSoftwareVersionMessageAsync(req, cancellationToken);
    }

    public async Task<DeviceInfo?> GetDeviceSystemDataAsync(uint deviceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfZero(deviceId, nameof(deviceId));
        
        var req = new CanMessage(Priority.Prio1, Command.StatusData, hash).AddUInt32(deviceId).AddByte(0);
        var res = await canMessageHandler.SendStatusMessageAsync(req, cancellationToken);
        return res is null ? null : new DeviceInfo(deviceId, res);
    }

    public async Task<DeviceMeasurement?> GetMeasurementSystemDataAsync(uint deviceId, byte index, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfZero(deviceId, nameof(deviceId));
        ArgumentOutOfRangeException.ThrowIfZero(index, nameof(index));

        var req = new CanMessage(Priority.Prio1, Command.StatusData, hash).AddUInt32(deviceId).AddByte(index);
        var res = await canMessageHandler.SendStatusMessageAsync(req, cancellationToken);
        return res is null ? null : new DeviceMeasurement(deviceId, index, res);
    }

    #endregion

    #region 7 GUI Information Transfer

    public async Task<Stream?> GetConfigDataAsync(string filename, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));
        
        var req = new CanMessage(Priority.Prio1, Command.ConfigDataRequest, hash).AddString(filename);
        var res = await canMessageHandler.SendStreamMessageAsync(req, cancellationToken);
        return res?.GetStream();
    }

    #endregion

    #region 9 Automation

    public async Task<byte?> SetAutomationAsync(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.AutomaticTransmission, hash).AddUInt16(deviceExpert).AddUInt16(automaticFunction).AddByte(position).AddByte(parameter);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    #endregion

    #region helper

    //private static ushort Index2Hash(byte index)
    //{
    //    ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 0x7f, nameof(index));

    //    return (ushort)(0x0300 | (index & 0x7f));
    //}

    internal static ushort DeviceId2Hash(uint deviceId)
    {
        ushort hd = (ushort)((deviceId & 0xffff0000) >> 16);
        ushort ld = (ushort)(deviceId & 0x0000ffff);
        ushort ro = (ushort)(hd ^ ld);
        ushort lx = (ushort)(ro & 0x007f);
        ushort hx = (ushort)((ro & 0x1f80) << 3);
        ushort ha = (ushort)(0x0300 | hx | lx);
        return ha;
    }

    #endregion
}
