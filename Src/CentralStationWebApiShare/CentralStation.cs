using CentralStationWebApi.Internal;


namespace CentralStationWebApi;

public sealed class CentralStation : IDisposable
{
    private const int PortSend = 15731;
    private const int PortReceive = 15730;

    private UdpClient sender;
    private UdpClient listener;
    private Task receiver;
    private string host;

    private readonly Thread messageEventThread;
    private readonly BlockingCollection<CANMessage> messageEventQueue = [];

    public CentralStation(string host = "CS3") 
    {
        this.host = host;

        messageEventThread = new Thread(MessageEventWorkLoop) { Name = "MessageEventThread", IsBackground = true };
        messageEventThread.Start();

        this.listener = new UdpClient(PortReceive);
        
        //this.listener.Connect(host, PortReceive);

        this.receiver = Task.Run(async () => await ReceiveAsync());

        this.sender = new UdpClient();
        this.sender.Connect(host, PortSend);

    }

    public void Dispose()
    {
        messageEventQueue.CompleteAdding();
        messageEventThread.Join();
        messageEventQueue.Dispose();

        sender.Close();
        sender.Dispose();
        // stop listening
        listener?.Close();
        listener?.Dispose();
    }

    private void MessageEventWorkLoop()
    {
        foreach (var message in messageEventQueue.GetConsumingEnumerable())
        {
            try { MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message)); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<FileReceivedEventArgs>? FileReceived;

    private async Task SendMessageAsync(CANMessage message, CancellationToken cancellationToken = default)
    {
        Debug.WriteLine($"Send: {message}");
        //int x = await sender.SendAsync(message.Buffer, 13, host, 15731);
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        int x = await sender.SendAsync(message.Buffer, 13);
    }

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
                messageEventQueue.Add(msg);

                if (msg.Command == Command.ConfigDataStream)
                {
                    if (msg.DataLength != 8)
                    {
                        stream = [];
                        streamLength = msg.UInt0;
                    }
                    else
                    {
                        stream.AddRange(msg.Data);
                        Debug.WriteLine($"Streamed {stream.Count} / {streamLength} bytes");
                        if (stream.Count >= streamLength)
                        {
                            using var file = File.Create("File.zlib");
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

    #region System Commands

    public async Task<bool> SystemStopAsync(uint device = 0, CancellationToken cancellationToken = default)
    {   
        var message = new SystemMessage(SystemCommand.Stop, device);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemGoAsync(uint device = 0, CancellationToken cancellationToken = default)                            
    {
        var message = new SystemMessage(SystemCommand.Go, device);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemHaltAsync(uint device = 0, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.Halt, device);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemLocoHaltAsync(uint device, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.LocoHalt, device);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemLocoCycleStopAsync(uint device, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.LocoCycleStop, device);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemLocoDataProtocolAsync(uint device, byte protocoll, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.LocoDataProtocol, device, protocoll);

        await SendMessageAsync(message, cancellationToken);

        return true;
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

    public async Task<bool> SystemTrackProtocolAsync(uint deviceUID, byte param, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.TrackProtocol, deviceUID, param);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    //public async Task<bool> SystemNewRegistrationCounterAsync(uint deviceUID, ushort counter, CancellationToken cancellationToken = default)
    //{
    //    var message = new SystemMessage(SystemCommand.FastRead, deviceUID, counter);

    //    await SendMessageAsync(message, cancellationToken);

    //    return true;
    //}

    public async Task<bool> SystemOverloadAsync(uint deviceUID, byte channel, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.Overload, deviceUID, channel);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemResetAsync(uint deviceUID, byte target, CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.Overload, deviceUID, target);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    #endregion

    public async Task<string> ConfigDataLocoInfo(CancellationToken cancellationToken = default)
    {
        var message = new ConfigDataMessage("lokinfo");

        await SendMessageAsync(message, cancellationToken);

        return "lokinfo";
    }

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);
    
    public async Task<string> ConfigDataLocos(CancellationToken cancellationToken = default)
    {
        var message = new ConfigDataMessage("loks");

        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return streamText;
    }

}


