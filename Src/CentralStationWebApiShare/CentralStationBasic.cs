namespace CentralStationWebApi;

public partial class CentralStationBasic : IDisposable
{
    private readonly IProtocolHandler client;

    private readonly Task receiver;
    internal static string? Host;
        
    public const uint AllDevices = 0x0000;
    public const ushort MinVelocity = 0;
    public const ushort MaxVelocity = 1000;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    private readonly MessageQueue<CANMessage> messageReceivedQueue;
    
    private readonly uint hash = 0x4711;
    //private readonly MessageQueue<CSFileStream> fileReceivedQueue;

    public CentralStationBasic(string host, Protocol protocol = Protocol.UDP)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(host);
        ArgumentOutOfRangeException.ThrowIfZero((int)Uri.CheckHostName(host), host);

        Host = host;

        client = protocol switch
        {
            Protocol.TCP => new TcpHandler(),
            Protocol.UDP => new UdpHandler(),
            _ => throw new NotSupportedException($"Protocol {protocol} not supported"),
        };

        client.Connect(host);

        messageReceivedQueue = new MessageQueue<CANMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        this.receiver = Task.Run(async () => await ReceiveAsync());
    }

    public void Dispose()
    {
        client.Dispose();

        messageReceivedQueue.Dispose();
    }

    #region Send Message
    
    private void SendMessage(CANMessage msg)
    {
        messageReceivedQueue.Add(msg);
        client.Send(msg);
        Tracer.TraceMessage(msg);
    }

    #endregion

    #region Receive Message

    //private string fileName;
    //private uint streamLength;
    //private List<byte> stream;
    //private string streamText;

    protected virtual void ReceiveHandler(CANMessage msg)
    { }

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

                ReceiveHandler(msg);


                //HandleAsync(msg);
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

    #endregion

    #region 2 System Commands

    public void SystemStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Stop);
        SendMessage(message);
    }

    public void SystemGo(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Go);
        SendMessage(message);
    }

    public void SystemHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Halt);
        SendMessage(message);
    }

    public void SystemLocomotiveEmergencyHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoHalt);
        SendMessage(message);
    }

    public void SystemLocomotiveCycleStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoCycleStop);
        SendMessage(message);
    }

    public void SystemLocomotiveDataProtocol(uint device = AllDevices, byte protocoll = 0xff)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoDataProtocol).
            AddByte(protocoll);
        SendMessage(message);
    }

    public void SystemArticleSwitchingTimel(uint device, ushort time)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.SwitchingTime).
            AddUInt16(time);
        SendMessage(message);
    }

    public void SystemMfxFastRead(uint device, ushort mfxSid)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.FastRead).
            AddUInt16(mfxSid);
        SendMessage(message);
    }

    public void SystemTrackProtocolSwitch(uint device, TrackProtocol protocoll)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.TrackProtocol).
            AddByte((byte)protocoll);
        SendMessage(message);
    }

    public void SystemMfxNewRegistrationCounter(uint device, ushort newRegistrationCounter)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.NewRegistrationCounter).
            AddUInt16(newRegistrationCounter);
        SendMessage(message);
    }

    public void SystemOverload(uint device, byte channel)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Overload).
            AddByte(channel);
        SendMessage(message);
    }

    public void SystemStatus(uint device, byte channel, ushort? value = null)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Status).
            AddByte(channel).
            AddUInt16(value);   // optional
        SendMessage(message);
    }

    public void SystemIdentifier(uint device, byte identifier)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Identifier).
            AddUInt16(identifier);   // optional
        SendMessage(message);
    }

    public void SystemMfxSeek(uint device)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.MfxSeek);
        SendMessage(message);
    }

    public void SystemReset(uint device, byte target)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Reset).
            AddByte(target);
        SendMessage(message);
    }

    #endregion

    #region 3 Administration

    public void GetLocomotiveDiscovery(uint locoId)
    {
        var message = new CANMessage(Priority.Proirity1, Command.Discovery, hash).AddUInt32(locoId);
        SendMessage(message);
    }

    public void SetLocomotiveDiscovery(uint locoId, ushort velocity)
    {
        var message = new CANMessage(Priority.Proirity1, Command.Discovery, hash).AddUInt32(locoId).AddUInt16(velocity);
        SendMessage(message);
    }

    public void SetMfxBinding(uint mfxUID, ushort mfxSID)
    {
        var message = new CANMessage(Priority.Proirity1, Command.Bind, hash).
            AddUInt32(mfxUID).
            AddUInt16(mfxSID);
        SendMessage(message);
    }

    public void SetMfxVerify(uint mfxUID, ushort mfxSID)
    {
        var message = new CANMessage(Priority.Proirity1, Command.Bind, hash).
            AddUInt32(mfxUID).
            AddUInt16(mfxSID);
        SendMessage(message);
    }

    public void GetLocomotiveVelocity(uint locoId)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoVelocity, hash).AddUInt32(locoId);
        SendMessage(message);
    }

    public void SetLocomotiveVelocity(uint locoId, ushort velocity)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(velocity, MaxVelocity, nameof(velocity));

        var message = new CANMessage(Priority.Proirity1, Command.LocoVelocity, hash).AddUInt32(locoId).AddUInt16(velocity);
        SendMessage(message);
    }

    public void GetLocomotiveDirection(uint locoId)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash).AddUInt32(locoId);
        SendMessage(message);
    }

    public void SetLocomotiveDirection(uint locoId, DirectionChange direction)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte((byte)direction);
        SendMessage(message);
    }

    public void GetLocomotiveFunction(uint locoId, byte function)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte(function);
        SendMessage(message);
    }

    public void SetLocomotiveFunction(uint locoId, byte function, byte value)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte(function).AddByte(value);
        SendMessage(message);
    }

    public void SetLocomotiveFunction(uint locoId, byte function, byte value, ushort functionValue)
    {
        var message = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash).AddUInt32(locoId).AddByte(function).AddByte(value).AddUInt16(functionValue);
        SendMessage(message);
    }

    #endregion

    #region 4 Article Commands

    #endregion

    #region 5 Feedback

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    public void RequestParticipants()
    {
        var message = new CANMessage(Priority.Proirity1, Command.SoftwareVersion, hash);
        SendMessage(message);
    }

    public void RequestStatusData(uint device, byte index)
    {
        var message = new CANMessage(Priority.Proirity1, Command.StatusData, hash).
            AddUInt32(device).
            AddByte(index);
        SendMessage(message);
    }

    #endregion

    #region 7 GUI Information Transfer

    public void RequestConfigDataLocomotives()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("loks");
        SendMessage(message);
    }

    public void RequestConfigDataMagneticItems()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("mags");
        SendMessage(message);
    }
    public void RequestConfigDataRailwayRoute()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("fs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagram()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("gbs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagramPage(int page)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString($"gbs-{page}");
        SendMessage(message);
    }

    //Filename: mfxbver
    public void RequestConfigDataMfxBVverPage(int page)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("mfxbver");
        SendMessage(message);
    }

    #endregion

    #region 9 Automation

    public void AutomaticTransmission(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter)
    {
        var message = new CANMessage(Priority.Proirity1, Command.AutomaticTransmission, hash).
            AddUInt16(deviceExpert).
            AddUInt16(automaticFunction).
            AddByte(position).
            AddByte(parameter);
        SendMessage(message);
    }

    #endregion

    //#region Static

    //public static string DeviceDescription(uint device)
    //{
    //    string description = $"Invalid Controller Type {device:X4}";

    //    CheckRange(ref description, device, 0x0000, 0x0000, "All", false);
    //    CheckRange(ref description, device, 0x0001, 0x03FF, "MM2");
    //    CheckRange(ref description, device, 0x0400, 0x07FF, "Reserved");
    //    CheckRange(ref description, device, 0x0800, 0x0BFF, "SX1");
    //    CheckRange(ref description, device, 0x0C00, 0x0FFF, "Reserved");
    //    CheckRange(ref description, device, 0x1000, 0x13FF, "Res. MM functiondecoder");
    //    CheckRange(ref description, device, 0x1400, 0x17FF, "Reserved");
    //    CheckRange(ref description, device, 0x1800, 0x1BFF, "Private / Club");
    //    CheckRange(ref description, device, 0x1C00, 0x1FFF, "Company");
    //    CheckRange(ref description, device, 0x2000, 0x23FF, "Reserved");
    //    CheckRange(ref description, device, 0x2400, 0x27FF, "Reserved MM Loco decoder");
    //    CheckRange(ref description, device, 0x2800, 0x2BFF, "SX1 Accessories");
    //    CheckRange(ref description, device, 0x2C00, 0x2FFF, "Reserved traction");
    //    CheckRange(ref description, device, 0x3000, 0x33FF, "MM Accessories");
    //    CheckRange(ref description, device, 0x3400, 0x37FF, "Reserved");
    //    CheckRange(ref description, device, 0x3800, 0x3BFF, "DCC Accessories A");
    //    CheckRange(ref description, device, 0x3C00, 0x3FFF, "DCC Accessories B");
    //    CheckRange(ref description, device, 0x4000, 0x7FFF, "MFX SID");
    //    CheckRange(ref description, device, 0x8000, 0xBFFF, "SX2");
    //    CheckRange(ref description, device, 0xC000, 0xFFFF, "DCC Adr.");
        
    //    return description;
    //}

    //private static void CheckRange(ref string description, uint device, uint start, uint end, string descriptionPrefix, bool addNum = true)
    //{
    //    if (device >= start && device <= end)
    //    {
    //        description = addNum ? $"{descriptionPrefix} {device - start}" : descriptionPrefix;
    //    }
    //}

    //#endregion
}


