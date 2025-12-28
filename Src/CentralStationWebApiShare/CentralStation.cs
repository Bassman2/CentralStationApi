using System.IO;

namespace CentralStationWebApi;

public sealed partial class CentralStation : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
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

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    //private readonly MessageQueue<CSFileStream> fileReceivedQueue;

    private TimeSpan receiveTimeout = TimeSpan.FromSeconds(30);

    public CentralStation(string host, SystemStatus systemStatus = SystemStatus.Default) 
    {
        this.host = host;

        messageReceivedQueue = new MessageQueue<CANMessage>((m) => MessageReceived?.Invoke(this, new MessageReceivedEventArgs(m)));
        //fileReceivedQueue = new MessageQueue<CSFileStream>((f) => FileReceived?.Invoke(this, new FileReceivedEventArgs(f)));

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
        //fileReceivedQueue.Dispose();
    }

    #region Properties

    public Locomotives? Locomotives;

    public MagneticItems? MagneticItems;

    public RailwayRoutes? RailwayRoutes;

    public TrackDiagram? TrackDiagram;

    public TrackDiagramPage? TrackDiagramPages;

    #endregion

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
                HandleParticipants(msg);
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
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Request, filename, msg.GetDataUInt(5), msg.GetDataUShort(9));
            }
            else if (msg.DataLength == 7)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Broadcast, filename, msg.GetDataUInt(5), msg.GetDataUShort(9), msg.GetDataByte(11));
            }
            else if(msg.DataLength == 8 && fileDictionary.TryGetValue(msg.Hash, out var fileStream))
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
        SaveStream(stream, fileName);

        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string? line = reader.ReadLine();
        switch (line)
        {
        case "[lokomotive]":
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Locomotives)));
            Locomotives = CsSerializer.Deserialize<Locomotives>(stream, "[lokomotive]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locomotives)));
            break;
        case "[magnetartikel]":
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(MagneticItems)));
            MagneticItems = CsSerializer.Deserialize<MagneticItems>(stream, "[magnetartikel]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MagneticItems)));
            break;
        case "[fahrstrassen]":
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(RailwayRoutes)));
            RailwayRoutes = CsSerializer.Deserialize<RailwayRoutes>(stream, "[fahrstrassen]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RailwayRoutes)));
            break;
        case "[gleisbild]":
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(TrackDiagram)));
            TrackDiagram = CsSerializer.Deserialize<TrackDiagram>(stream, "[gleisbild]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackDiagram)));
            break;
        case "[gleisbildseite]":
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(TrackDiagram)));
            TrackDiagramPages = CsSerializer.Deserialize<TrackDiagramPage>(stream, "[gleisbildseite]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackDiagram)));
            break;
        }
    }

    private const string dataPath = @"C:\MärklinData";

    private void SaveStream(Stream stream, string fileName)
    {
        if (Directory.Exists(dataPath))
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string? firstLine = reader.ReadLine();

            string fileId = firstLine?.Trim('[', ']') ?? "unknown";

            using var file = File.CreateText(Path.Combine(dataPath, $"{fileName}-{fileId}.txt"));
            file.WriteLine(firstLine);
            file.Write(reader.ReadToEnd());
        }
        stream.Position = 0;
    }

    private uint hash = 0x4711;

    public IEnumerable<Device> Devices = [];

    private readonly Dictionary<uint, Device> devices = [];

    private void HandleParticipants(CANMessage msg)
    {
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            uint device = msg.Device;
            devices[device] = new Device(device, msg.GetDataByte(9), msg.GetDataByte(10), msg.GetDataUShort(11));
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Devices)));
            Devices = devices.Values;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Devices)));
        }
    }

    #endregion

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


