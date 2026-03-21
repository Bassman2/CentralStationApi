namespace CentralStationApi;

/// <summary>
/// Provides access to a Märklin Central Station via network communication.
/// Implements the CAN bus protocol for controlling model railway systems.
/// </summary>
public sealed class CentralStation : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    private IProtocolHandler? client;
    private Task? receiver;
    private ushort hash;

    private readonly CanMessageHandler canMessageHandler;
    private readonly MessageQueue<CanMessage> messageReceivedQueue;

    /// <summary>
    /// Represents all devices in the system. Used for broadcasting commands.
    /// </summary>
    public const uint AllDevices = 0x0000;

    /// <summary>
    /// Minimum velocity value (stopped).
    /// </summary>
    public const ushort MinVelocity = 0;

    /// <summary>
    /// Maximum velocity value (full speed).
    /// </summary>
    public const ushort MaxVelocity = 1000;

    /// <summary>
    /// Occurs when a property value has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when a property value is changing.
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Occurs when a CAN message is received from the Central Station.
    /// </summary>
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Occurs when a locomotive emergency halt command is received.
    /// </summary>
    public event EventHandler<LocomotiveEventArgs>? LocomotiveHalt;

    /// <summary>
    /// Occurs when a locomotive velocity change is received.
    /// </summary>
    public event EventHandler<LocomotiveVelocityEventArgs>? LocomotiveVelocity;

    /// <summary>
    /// Occurs when a locomotive direction change is received.
    /// </summary>
    public event EventHandler<LocomotiveDirectionEventArgs>? LocomotiveDirection;

    /// <summary>
    /// Occurs when a locomotive function change is received.
    /// </summary>
    public event EventHandler<LocomotiveFunctionEventArgs>? LocomotiveFunction;

    internal static Uri GuiUri => new($"http://{Host}/images/gui/");
    internal static Uri MagUri => new($"http://{Host}/app/assets/mag/");
    internal static Uri LocoUri => new($"http://{Host}/app/assets/lok/");

    /// <summary>
    /// Gets the URI for a GUI image resource.
    /// </summary>
    /// <param name="name">The name of the GUI resource.</param>
    /// <returns>The URI to the GUI resource.</returns>
    public Uri GetGuiUri(string name) => new($"http://{Host}/images/gui/");

    /// <summary>
    /// Gets the URI for a magazine asset resource.
    /// </summary>
    /// <param name="name">The name of the magazine resource.</param>
    /// <returns>The URI to the magazine resource.</returns>
    public Uri GetMagUri(string name) => new($"http://{Host}/app/assets/mag/");

    /// <summary>
    /// Gets the URI for a locomotive image.
    /// </summary>
    /// <param name="name">The name of the locomotive image file (without extension).</param>
    /// <returns>The URI to the locomotive image.</returns>
    public Uri GetLocoUri(string name) => new($"http://{Host}/app/assets/lok/{name}.png");

    /// <summary>
    /// Gets the hostname or IP address of the Central Station.
    /// </summary>
    public static string? Host { get; private set; }

    /// <summary>
    /// Gets the device data for this client connection.
    /// </summary>
    public DeviceData Device { get; private set; } = new DeviceData(0x6D554711, new Version(1, 0));

    /// <summary>
    /// Gets or sets the timeout for waiting for a single message response.
    /// </summary>
    public TimeSpan MessageTimeout
    {   
        get => canMessageHandler.MessageTimeout;
        set => canMessageHandler.MessageTimeout = value; 
    }

    /// <summary>
    /// Gets or sets the timeout for waiting for a collection of message responses.
    /// </summary>
    public TimeSpan CollectionTimeout
    {
        get => canMessageHandler.CollectionTimeout;
        set => canMessageHandler.CollectionTimeout = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CentralStation"/> class.
    /// </summary>
    public CentralStation()
    {
        canMessageHandler = new CanMessageHandler(this);
        messageReceivedQueue = new MessageQueue<CanMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CentralStation"/> class and connects to the specified host.
    /// </summary>
    /// <param name="host">The hostname or IP address of the Central Station.</param>
    /// <param name="protocol">The protocol to use (TCP or UDP). Default is TCP.</param>
    /// <param name="device">Optional device data for the client. If null, uses default values.</param>
    public CentralStation(string host, Protocol protocol = Protocol.TCP, DeviceData? device = null) : this()
    {
        Connect(host, protocol, device);
    }

    /// <summary>
    /// Connects to the Central Station at the specified host.
    /// </summary>
    /// <param name="host">The hostname or IP address of the Central Station.</param>
    /// <param name="protocol">The protocol to use (TCP or UDP). Default is TCP.</param>
    /// <param name="device">Optional device data for the client. If null, uses default values.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="host"/> is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="host"/> is not a valid hostname.</exception>
    /// <exception cref="NotSupportedException">Thrown when the specified protocol is not supported.</exception>
    public void Connect(string host, Protocol protocol = Protocol.TCP, DeviceData? device = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(host);
        ArgumentOutOfRangeException.ThrowIfZero((int)Uri.CheckHostName(host), host);

        Host = host;
        Device = device ?? new DeviceData(0x6D554711, new Version(1, 0));
        hash = DeviceId2Hash(Device.DeviceId);

        HashCache.AddHash((ushort)hash, "PCApp");

        client = protocol switch
        {
            Protocol.TCP => new TcpHandler(),
            Protocol.UDP => new UdpHandler(),
            _ => throw new NotSupportedException($"Protocol {protocol} not supported"),
        };

        client.Connect(host);

        //messageReceivedQueue = new MessageQueue<CanMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        this.receiver = Task.Run(async () => await ReceiveAsync());
    }

    /// <summary>
    /// Releases all resources used by the <see cref="CentralStation"/>.
    /// </summary>
    public void Dispose()
    {
        if (client is not null)
        {
            client.Dispose();
            client = null;
        }

        messageReceivedQueue.Dispose();
    }

    #region Send Message

    internal void SendMessage(CanMessage msg)
    {
        CentralStationNotConnectedException.ThrowIfNotConnected(client);

        messageReceivedQueue.Add(msg);
        client.Send(msg);
        Tracer.TraceMessage(msg);
    }

    internal async Task SendMessageAsync(CanMessage msg)
    {
        CentralStationNotConnectedException.ThrowIfNotConnected(client);

        messageReceivedQueue.Add(msg);
        await client.SendAsync(msg);
        Tracer.TraceMessage(msg);
    }

    #endregion

    #region Receive Message

    private async Task ReceiveAsync()
    {
        CentralStationNotConnectedException.ThrowIfNotConnected(client);

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

    /// <summary>
    /// Gets the current system status (Go/Stop/Halt).
    /// </summary>
    public SystemStatus Status { get; private set; } = CentralStationApi.SystemStatus.Default;

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
                SetStatus(CentralStationApi.SystemStatus.Stop);
            }
            else if (message.SubCommand == SubCommand.Go)
            {
                SetStatus(CentralStationApi.SystemStatus.Go);

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

    /// <summary>
    /// Sends a system stop command to halt all track power.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemStopAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Stop);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sends a system go command to enable track power.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemGoAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Go);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sends a system halt command.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemHaltAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Halt);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sends an emergency halt command for all locomotives.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemLocomotiveEmergencyHaltAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoHalt);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sends a command to stop the locomotive cycle.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemLocomotiveCycleStopAsync(uint deviceId = AllDevices, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoCycleStop);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sets the locomotive data protocol.
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="protocoll">The protocol value (0xff for all protocols).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemLocomotiveDataProtocolAsync(uint deviceId = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.LocoDataProtocol).AddByte(protocoll);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sets the switching time for articles (accessories).
    /// </summary>
    /// <param name="deviceId">The device ID to target. Use <see cref="AllDevices"/> for all devices.</param>
    /// <param name="time">The switching time in milliseconds (0xff for default).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemArticleSwitchingTimeAsync(uint deviceId = AllDevices, ushort time = 0xff, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.SwitchingTime).AddUInt16(time);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    // SubCommand.FastRead not implemented

    /// <summary>
    /// Sets the track protocol.
    /// </summary>
    /// <param name="deviceId">The device ID to target.</param>
    /// <param name="protocoll">The track protocol to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemTrackProtocolAsync(uint deviceId, TrackProtocol protocoll, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.TrackProtocol).AddByte((byte)protocoll);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sets the MFX new registration counter.
    /// </summary>
    /// <param name="deviceId">The device ID to target.</param>
    /// <param name="protocoll">The track protocol.</param>
    /// <param name="newRegistrationCounter">The new registration counter value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemMfxNewRegistrationCounterAsync(uint deviceId, TrackProtocol protocoll, ushort newRegistrationCounter, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.NewRegistrationCounter).AddByte((byte)protocoll).AddUInt16(newRegistrationCounter);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    // SubCommand.Overload not implemented only response from TFP/GFP

    /// <summary>
    /// Gets system status information for a specific channel.
    /// </summary>
    /// <param name="deviceId">The device ID to query.</param>
    /// <param name="channel">The channel number.</param>
    /// <param name="configurationValue">Optional configuration value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The status value, or null if no data is returned.</returns>
    /// <exception cref="InvalidDataException">Thrown when the response has unexpected data length.</exception>
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

    /// <summary>
    /// Gets the system identifier for a device.
    /// </summary>
    /// <param name="deviceId">The device ID to query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> GetSystemIdentifierAsync(uint deviceId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Identifier);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Sets the system identifier for a device.
    /// </summary>
    /// <param name="deviceId">The device ID to set.</param>
    /// <param name="identifier">The identifier value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SetSystemIdentifierAsync(uint deviceId, byte identifier, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Identifier).AddByte(identifier);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    /// <summary>
    /// Resets a device or system component.
    /// </summary>
    /// <param name="deviceId">The device ID to reset.</param>
    /// <param name="resetTarge">The reset target specifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SystemResetAsync(uint deviceId, byte resetTarge, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SystemCommand, hash).AddUInt32(deviceId).AddSubCommand(SubCommand.Reset).AddByte(resetTarge);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is not null;
    }

    #endregion

    #region 3 Administration

    /// <summary>
    /// Gets the current direction of a locomotive.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current direction, or null if unavailable.</returns>
    public async Task<DirectionChange?> GetLocomotiveDirectionAsync(uint locoId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoDirection, hash).AddUInt32(locoId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return (DirectionChange?)res?.GetDataByte(4) ?? null;
    }

    /// <summary>
    /// Sets the direction of a locomotive.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="direction">The direction to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new direction, or null if the command failed.</returns>
    public async Task<DirectionChange?> SetLocomotiveDirectionAsync(uint locoId, DirectionChange direction, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte((byte)direction);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return (DirectionChange?)res?.GetDataByte(4) ?? null;
    }

    /// <summary>
    /// Gets the current value of a locomotive function.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="function">The function number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The function value, or null if unavailable.</returns>
    public async Task<byte?> GetLocomotiveFunctionAsync(uint locoId, byte function, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    /// <summary>
    /// Sets a locomotive function value.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="function">The function number.</param>
    /// <param name="value">The function value to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new function value, or null if the command failed.</returns>
    public async Task<byte?> SetLocomotiveFunctionAsync(uint locoId, byte function, byte value, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function).AddByte(value);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    /// <summary>
    /// Sets a locomotive function value with an additional function parameter.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="function">The function number.</param>
    /// <param name="value">The function value to set.</param>
    /// <param name="functionValue">Additional function parameter value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new function value, or null if the command failed.</returns>
    public async Task<byte?> SetLocomotiveFunctionAsync(uint locoId, byte function, byte value, ushort functionValue, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.LocoFunction, hash).AddUInt32(locoId).AddByte(function).AddByte(value).AddUInt16(functionValue);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    /// <summary>
    /// Gets the current velocity of a locomotive.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="velocity">The velocity value (unused in get operation).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current velocity, or null if unavailable.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when velocity exceeds <see cref="MaxVelocity"/>.</exception>
    public async Task<ushort?> GetLocomotiveVelocityAsync(uint locoId, ushort velocity, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(velocity, MaxVelocity, nameof(velocity));

        var req = new CanMessage(Priority.Prio1, Command.LocoVelocity, hash).AddUInt32(locoId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    /// <summary>
    /// Sets the velocity of a locomotive.
    /// </summary>
    /// <param name="locoId">The locomotive ID.</param>
    /// <param name="velocity">The velocity value (0-1000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new velocity, or null if the command failed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when velocity exceeds <see cref="MaxVelocity"/>.</exception>
    public async Task<ushort?> SetLocomotiveVelocityAsync(uint locoId, ushort velocity, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(velocity, MaxVelocity, nameof(velocity));

        var req = new CanMessage(Priority.Prio1, Command.LocoVelocity, hash).AddUInt32(locoId).AddUInt16(velocity);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataByte(5) ?? null;
    }

    #endregion

    #region 4 Article Commands

    /// <summary>
    /// Sets the position of an article (accessory) switch.
    /// </summary>
    /// <param name="localId">The local ID of the article.</param>
    /// <param name="standing">The position/state to set.</param>
    /// <param name="power">The power setting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SetArticleSwitchAsync(uint localId, byte standing, byte power, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SwitchAccessories, hash).AddUInt32(localId).AddByte(standing).AddByte(power);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res != null;
    }

    /// <summary>
    /// Sets the position of an article (accessory) switch with a specific switching time.
    /// </summary>
    /// <param name="localId">The local ID of the article.</param>
    /// <param name="standing">The position/state to set.</param>
    /// <param name="power">The power setting.</param>
    /// <param name="time">The switching time in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the command was successful; otherwise, false.</returns>
    public async Task<bool> SetArticleSwitchAsync(uint localId, byte standing, byte power, ushort time, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SwitchAccessories, hash).AddUInt32(localId).AddByte(standing).AddByte(power).AddUInt16(time);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res != null;
    }

    #endregion

    #region 5 Feedback

    /// <summary>
    /// Polls S88 feedback module data (obsolete).
    /// </summary>
    /// <param name="deviceId">The device ID of the S88 module.</param>
    /// <param name="module">The module number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The module data, or null if unavailable.</returns>
    // obsolete
    public async Task<ushort?> GetS88PollingAsync(uint deviceId, byte module, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Polling, hash).AddUInt32(deviceId).AddByte(module);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res?.GetDataUShort(5) ?? null; 
    }

    /// <summary>
    /// Gets the current state of an S88 event.
    /// </summary>
    /// <param name="deviceId">The device ID of the S88 module.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The S88 event data, or null if unavailable.</returns>
    public async Task<S88Event?> GetS88EventAsync(ushort deviceId, ushort contactId, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Event, hash).AddUInt16(deviceId).AddUInt16(contactId);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is null ? null : new S88Event() { OldValue = res.GetDataByte(4), NewValue = res.GetDataByte(5), Time = res.GetDataUShort(6) };
    }

    /// <summary>
    /// Registers an S88 event for monitoring.
    /// </summary>
    /// <param name="deviceId">The device ID of the S88 module.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="parameter">The registration parameter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The S88 event data after registration, or null if unavailable.</returns>
    public async Task<S88Event?> RegisterS88EventAsync(ushort deviceId, ushort contactId, byte parameter, CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.S88Event, hash).AddUInt16(deviceId).AddUInt16(contactId).AddByte(parameter);
        var res = await canMessageHandler.SendMessageAsync(req, cancellationToken);
        return res is null ? null : new S88Event() { OldValue = res.GetDataByte(4), NewValue = res.GetDataByte(5), Time = res.GetDataUShort(6) };
    }

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    /// <summary>
    /// Gets a list of all devices connected to the Central Station.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of devices, or null if unavailable.</returns>
    public async Task<List<Device>?> GetAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var req = new CanMessage(Priority.Prio1, Command.SoftwareVersion, hash);
        return await canMessageHandler.SendSoftwareVersionMessageAsync(req, cancellationToken);
    }

    /// <summary>
    /// Gets system data for a specific device.
    /// </summary>
    /// <param name="deviceId">The device ID to query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The device information, or null if unavailable.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when deviceId is zero.</exception>
    public async Task<DeviceInfo?> GetDeviceSystemDataAsync(uint deviceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfZero(deviceId, nameof(deviceId));
        
        var req = new CanMessage(Priority.Prio1, Command.StatusData, hash).AddUInt32(deviceId).AddByte(0);
        var res = await canMessageHandler.SendStatusMessageAsync(req, cancellationToken);
        return res is null ? null : new DeviceInfo(deviceId, res);
    }

    /// <summary>
    /// Gets measurement data from a device.
    /// </summary>
    /// <param name="deviceId">The device ID to query.</param>
    /// <param name="index">The measurement channel index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The measurement data, or null if unavailable.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when deviceId or index is zero.</exception>
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

    /// <summary>
    /// Downloads configuration data from the Central Station.
    /// </summary>
    /// <param name="filename">The name of the configuration file to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the configuration data, or null if unavailable.</returns>
    /// <exception cref="ArgumentNullException">Thrown when filename is null or whitespace.</exception>
    public async Task<Stream?> GetConfigDataAsync(string filename, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));
        
        var req = new CanMessage(Priority.Prio1, Command.ConfigDataRequest, hash).AddString(filename);
        var res = await canMessageHandler.SendStreamMessageAsync(req, cancellationToken);
        return res?.GetStream();
    }

    #endregion

    #region 9 Automation

    /// <summary>
    /// Sends an automation command to control automated functions.
    /// </summary>
    /// <param name="deviceExpert">The device expert identifier.</param>
    /// <param name="automaticFunction">The automatic function identifier.</param>
    /// <param name="position">The position parameter.</param>
    /// <param name="parameter">Additional parameter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response value, or null if the command failed.</returns>
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

    /// <summary>
    /// Converts a device ID to a hash value used in CAN messages.
    /// </summary>
    /// <param name="deviceId">The device ID to convert.</param>
    /// <returns>The hash value derived from the device ID.</returns>
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
