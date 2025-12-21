

namespace CentralStationWebApi;

public sealed class CentralStation : IDisposable
{
    private const int PortSend = 15731;
    private const int PortReceive = 15730;

    private UdpClient sender;
    private UdpClient listener;
    private Task receiver;
    private string host;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    private readonly MessageQueue<CANMessage> messageReceivedQueue;

    public event EventHandler<FileReceivedEventArgs> FileReceived;
    private readonly MessageQueue<CSFile> fileReceivedQueue;

    private TimeSpan receiveTimeout = TimeSpan.FromSeconds(30);

    public CentralStation(string host = "CS3") 
    {
        this.host = host;

        messageReceivedQueue = new MessageQueue<CANMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        fileReceivedQueue = new MessageQueue<CSFile>((f) => FileReceived?.Invoke(this, new FileReceivedEventArgs(f)));

        this.listener = new UdpClient(PortReceive);
        this.receiver = Task.Run(async () => await ReceiveAsync());

        this.sender = new UdpClient();
        this.sender.Connect(host, PortSend);
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

    private List<MessageRequest> messageQueue = [];

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

    private string fileName;
    private uint streamLength;
    private List<byte> stream;
    private string streamText;

    private async Task ReceiveAsync()
    {         
        try
        {
            while (true)
            {
                var result = await listener.ReceiveAsync();
                var msg = new CANMessage(result);
                Debug.WriteLine($"Received: {msg}");
                //MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));
                messageReceivedQueue.Add(msg);

                if (msg.IsResponse && msg.Command == Command.SystemCommand && msg.SystemCommand == SystemCommand.Stop)
                {
                    //systemStopResMessage = msg;
                    //systemStopRespEvent.Set();
                }
                
                if (msg.Command == Command.ConfigDataStream)
                {
                    if (msg.DataLength != 8)
                    {
                        stream = [];
                        streamLength = msg.GetDataUInt(5);
                    }
                    else
                    {
                        stream.AddRange(msg.GetData());
                        Debug.WriteLine($"Streamed {stream.Count} / {streamLength} bytes");
                        if (stream.Count >= streamLength)
                        {
                            using var file = File.Create("CSFile.zlib");
                            file.Write(stream.ToArray(), 0, (int)streamLength);
                            file.Close();
                                
                            // Decompress zlib-compressed data
                            using var compressed = new MemoryStream(stream.ToArray(), 4, (int)streamLength - 4);
                            //using var zls = new Ionic.Zlib.ZlibStream(compressed, Ionic.Zlib.CompressionMode.Decompress);
                            using var zls = new ZLibStream(compressed, CompressionMode.Decompress);
                            using var outMs = new MemoryStream();
                            await zls.CopyToAsync(outMs);
                            var decompressedBytes = outMs.ToArray();

                            // Convert to string (assume UTF8); adjust if another encoding is used
                            streamText = Encoding.UTF8.GetString(decompressedBytes);

                            //FileReceived?.Invoke(this, new FileReceivedEventArgs(fileName, streamText));
                            autoResetEventConfigDataStream.Set();
                        }
                    }
                    // handle system command response if needed
                }

            }
        }
        catch (ObjectDisposedException)
        {
            // expected on disposal
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Receiver error: {ex}");
        }
    }

    #endregion

    #region System Commands

    private uint hash = 0x4711;

    //private CANMessage systemStopReqMessage;
    //private CANMessage systemStopResMessage;
    //private AutoResetEvent systemStopRespEvent = new AutoResetEvent(false);

    public void SystemStop(uint device = 0)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        SendMessage(message);
    }

    public async Task<CANMessage> SystemStopAsync(uint device = 0, CancellationToken cancellationToken = default)
    {   
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemGoAsync(uint device = 0, CancellationToken cancellationToken = default)                            
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemHaltAsync(uint device = 0, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemLocoHaltAsync(uint device, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemLocoCycleStopAsync(uint device, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemLocoDataProtocolAsync(uint device, byte protocoll, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        return await SendMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemSwitchingTimeAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    //{
    //    var message = new SystemMessage(SystemCommand.LocoDataProtocol, device, time);

    //    await SendMessageAsync(message, cancellationToken);

    //    return true;
    //}

    //public async Task<bool> SystemFastReadAsync(uint deviceUID, ushort mfxSID, CancellationToken cancellationToken = default)
    //{
    //    var message = new SystemMessage(SystemCommand.FastRead, deviceUID, mfxSID);

    //    await SendMessageAsync(message, cancellationToken);

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
    //    var message = new SystemMessage(SystemCommand.FastRead, deviceUID, counter);

    //    await SendMessageAsync(message, cancellationToken);

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
        var message = new CANMessage(Priority.Proirity1, Command.ConfigData, hash);
        message.DataLength = 8;
        message.SetData("lokinfo");
        await SendMessageAsync(message, cancellationToken);

        return "lokinfo";
    }

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);
    
    public async Task<string> ConfigDataLocos(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.ConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return streamText;
    }

}


