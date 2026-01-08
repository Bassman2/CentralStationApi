namespace CentralStationWebApi;

public partial class CentralStationBasic : IDisposable
{
    private readonly IProtocolHandler client;

    private readonly Task receiver;
    internal static string? Host;
        
    public const uint AllDevices = 0x0000;  

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


                HandleAsync(msg);
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

    #region Static

    public static string DeviceDescription(uint device)
    {
        string description = $"Invalid Controller Type {device:X4}";

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


