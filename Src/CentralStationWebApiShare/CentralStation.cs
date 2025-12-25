namespace CentralStationWebApi;

public sealed class CentralStation : IDisposable
{
    private const int PortSend = 15731;
    private const int PortReceive = 15730;

    private readonly UdpClient sender;
    private readonly UdpClient listener;
    private readonly Task receiver;
    private readonly string host;
        
    public const uint AllDevices = 0x0000;  

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    private readonly MessageQueue<CANMessage> messageReceivedQueue;

    public event EventHandler<FileReceivedEventArgs>? FileReceived;
    private readonly MessageQueue<CSFileStream> fileReceivedQueue;

    private TimeSpan receiveTimeout = TimeSpan.FromSeconds(30);

    public CentralStation(string host, SystemStatus systemStatus = SystemStatus.Default) 
    {
        this.host = host;

        messageReceivedQueue = new MessageQueue<CANMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        fileReceivedQueue = new MessageQueue<CSFileStream>((f) => FileReceived?.Invoke(this, new FileReceivedEventArgs(f)));

        this.listener = new UdpClient(PortReceive);
        this.receiver = Task.Run(async () => await ReceiveAsync());

        this.sender = new UdpClient();
        this.sender.Connect(host, PortSend);

        switch (systemStatus)
        {
            case SystemStatus.Stop:
                SystemStop();
                break;
            case SystemStatus.Go:
                SystemGo();
                break;
            default:
                break;  
        }
    }

    public void Dispose()
    {
        sender.Close();
        sender.Dispose();
        // stop listening
        listener?.Close();
        listener?.Dispose();

        messageReceivedQueue.Dispose();
        fileReceivedQueue.Dispose();
    }
    
    #region Send Message

    private readonly List<MessageRequest> messageQueue = [];

    private async Task<CANMessage> SendMessageAsync(CANMessage message, CancellationToken cancellationToken = default)
    {
        messageReceivedQueue.Add(message);
        int x = await sender.SendAsync(message.Buffer, 13);
        return message;
    }

    private void SendMessage(CANMessage message)
    {
        messageReceivedQueue.Add(message);
        sender.Send(message.Buffer, 13);
    }

    #endregion

    #region Receive Message

    //private string fileName;
    //private uint streamLength;
    //private List<byte> stream;
    //private string streamText;

    private async Task ReceiveAsync()
    {         
        try
        {
            while (true)
            {
                var result = await listener.ReceiveAsync();
                var msg = new CANMessage(result);
                Debug.WriteLineIf(TraceSwitches.CanReceiveSwitch.TraceInfo, $"Received: {msg}");
                messageReceivedQueue.Add(msg);
                HandleStreams(msg); 
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

    private readonly Dictionary<ushort, CSFileStream> fileDictionary = [];

    private void HandleStreams(CANMessage msg)
    {
        if (/*msg.IsResponse && */ msg.Command == Command.ConfigDataStream)
        {
            if (msg.DataLength == 6)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Request, msg.GetDataUInt(5), msg.GetDataUShort(9));
            }
            else if (msg.DataLength == 7)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Broadcast, msg.GetDataUInt(5), msg.GetDataUShort(9), msg.GetDataByte(11));
            }
            else if(msg.DataLength == 8 && fileDictionary.TryGetValue(msg.Hash, out var fileStream))
            {
                if (fileStream.AddData(msg.GetData()))
                {
                    fileReceivedQueue.Add(fileStream);
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

    #endregion

    #region System Commands

    private uint hash = 0x4711;

    //private CANMessage systemStopReqMessage;
    //private CANMessage systemStopResMessage;
    //private AutoResetEvent systemStopRespEvent = new AutoResetEvent(false);

    public void SystemStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {   
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        return await SendMessageAsync(message, cancellationToken);
    }

    public void SystemGo(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemGoAsync(uint device = AllDevices, CancellationToken cancellationToken = default)                            
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        return await SendMessageAsync(message, cancellationToken);
    }

    public void SystemHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        return await SendMessageAsync(message, cancellationToken);
    }

    public void SystemLocoHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemLocoHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        return await SendMessageAsync(message, cancellationToken);
    }

    public void SystemLocoCycleStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemLocoCycleStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        return await SendMessageAsync(message, cancellationToken);
    }

    public void SystemLocoDataProtocol(uint device = AllDevices, byte protocoll = 0xff)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemLocoDataProtocolAsync(uint device = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        return await SendMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemSwitchingTimeAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.LocoDataProtocol, device, time);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    //public async Task<bool> SystemFastReadAsync(uint deviceUID, ushort mfxSID, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, mfxSID);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<CANMessage> SystemTrackProtocolAsync(uint deviceUID, byte param, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.TrackProtocol);
        message.SetData(param, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemNewRegistrationCounterAsync(uint deviceUID, ushort counter, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, counter);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<CANMessage> SystemOverloadAsync(uint deviceUID, byte channel, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(channel, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemResetAsync(uint deviceUID, byte target, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(target, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    #endregion

    public async Task<string> ConfigDataLocoInfo(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("lokinfo");
        await SendMessageAsync(message, cancellationToken);

        return "lokinfo";
    }

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);

    public void RequestConfigDataLocos()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        SendMessage(message);
    }

    public async Task<string> ConfigDataLocosAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return ""; 
    }

    public void RequestConfigDataMagneticItems()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("mags");
        SendMessage(message);
    }

    public async Task<string> ConfigDataMagneticItemsAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("mags");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public void RequestConfigDataRailwayRoute()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("fs");
        SendMessage(message);
    }

    public async Task<string> ConfigDataRailwayRouteAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("fs");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public void RequestConfigDataTrackDiagramRoute()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("gbsstat“");
        SendMessage(message);
    }

    public async Task<string> ConfigDataTrackDiagramAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("gbsstat“");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    #region Static

    public static string DeviceDescription(uint device)
    {
        string description = $"Invalid Device Type {device:X4}";

        CheckRange(ref description, device, 0x0000, 0x0000, "All", false);
        CheckRange(ref description, device, 0x0001, 0x03FF, "MM2");
        CheckRange(ref description, device, 0x0400, 0x07FF, "Reserved");
        CheckRange(ref description, device, 0x0800, 0x0BFF, "SX1");
        CheckRange(ref description, device, 0x0C00, 0x0FFF, "Reserved");
        CheckRange(ref description, device, 0x1000, 0x13FF, "Res. MM functiondecoder");
        CheckRange(ref description, device, 0x1400, 0x17FF, "Reserved");
        CheckRange(ref description, device, 0x1800, 0x1BFF, "Private / Club");
        CheckRange(ref description, device, 0x1C00, 0x1FFF, "Company");
        CheckRange(ref description, device, 0x2000, 0x23FF, "Reserved");
        CheckRange(ref description, device, 0x2400, 0x27FF, "Reserved MM Loco decoder");
        CheckRange(ref description, device, 0x2800, 0x2BFF, "SX1 Accessories");
        CheckRange(ref description, device, 0x2C00, 0x2FFF, "Reserved traction");
        CheckRange(ref description, device, 0x3000, 0x33FF, "MM Accessories");
        CheckRange(ref description, device, 0x3400, 0x37FF, "Reserved");
        CheckRange(ref description, device, 0x3800, 0x3BFF, "DCC Accessories A");
        CheckRange(ref description, device, 0x3C00, 0x3FFF, "DCC Accessories B");
        CheckRange(ref description, device, 0x4000, 0x7FFF, "MFX SID");
        CheckRange(ref description, device, 0x8000, 0xBFFF, "SX2");
        CheckRange(ref description, device, 0xC000, 0xFFFF, "DCC Adr.");
        
        return description;
    }

    private static void CheckRange(ref string description, uint device, uint start, uint end, string descriptionPrefix, bool addNum = true)
    {
        if (device >= start && device <= end)
        {
            description = addNum ? $"{descriptionPrefix} {device - start}" : descriptionPrefix;
        }
    }

    #endregion
}


