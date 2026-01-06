namespace CentralStationWebApi;

public class CentralStation(string host, Protocol protocol = Protocol.TCP) : CentralStationBasic(host, protocol), INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    protected override void ReceiveHandler(CANMessage msg)
    {
        HandleStatus(msg);
        //HandleStreams(msg);
        //HandleParticipants(msg);
        //HandleStatusData(msg);
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
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Request, filename, msg.GetDataUInt(0), msg.GetDataUShort(0));
            }
            else if (msg.DataLength == 7)
            {
                // overwrite existing
                fileDictionary[msg.Hash] = new CSFileStream(CSFileStreamMode.Broadcast, filename, msg.GetDataUInt(0), msg.GetDataUShort(4), msg.GetDataByte(6));
            }
            else if (msg.DataLength == 8 && fileDictionary.TryGetValue(msg.Hash, out var fileStream))
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

        Tracer.TraceStream(stream, fileName);

        stream.Position = 0;
        var reader = new StreamReader(stream);
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
        if (message.Command == Command.SystemCommand && message.Device == CentralStationBasic.AllDevices)
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

    #region Locomotives

    public Locomotives? Locomotives;

    #endregion

    #region Articles

    public MagneticItems? MagneticItems;


    #endregion

    #region Routes

    public RailwayRoutes? RailwayRoutes;


    #endregion

    #region Tracks

    public TrackDiagram? TrackDiagram;

    public TrackDiagramPage? TrackDiagramPages;

    #endregion

    #region Controllers

    public IEnumerable<Device> Devices = [];

    private readonly Dictionary<uint, Device> devices = [];

    private void HandleParticipants(CANMessage msg)
    {
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            var device = new Device(msg);
            if (!devices.TryGetValue(msg.Device, out var oldDevice) || !device.Equals(oldDevice))
            {
                devices[msg.Device] = new Device(msg);
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Devices)));
                Devices = devices.Values;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Devices)));
            }
        }
    }

    //private Dictionary<uint, StatusDataDevice> statusData = [];
    public List<StatusDataDevice> StatusData = [];
    //private StatusDataDevice? curStatusData;
    //private ushort nextStatusDataPackage = 1;

    private DataCollector statusDataCollector = new();

    private void HandleStatusData(CANMessage msg)
    {
        if (msg.Command == Command.StatusData) // && msg.IsResponse)
        {
            switch (msg.DataLength)
            {
            case 5:
                Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 5 Device {msg.Device:X8} Index {msg.GetDataByte(4)}");
                break;
            case 6:
                byte index = msg.GetDataByte(4);
                Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 6 Device {msg.Device:X8} Index {msg.GetDataByte(4)} NumOfPackages {msg.GetDataByte(5)}");

                if (index == 0)
                {

                    StatusDataDevice statusDataDevice = new(msg.Device, msg.GetDataByte(4), msg.GetDataByte(5), statusDataCollector);
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(StatusData)));
                    StatusData.Add(statusDataDevice);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusData)));
                }
                else
                {
                    StatusDataValue statusDataValue = new(msg.Device, msg.GetDataByte(4), msg.GetDataByte(5), statusDataCollector);
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(StatusData)));
                    StatusData.First(d => d.DeviceId == msg.Device).Values[index - 1] = statusDataValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusData)));
                }
                break;
            case 8:
                ushort packageIndex = (byte)(msg.Hash & 0xff);
                Debug.WriteLineIf(TraceSwitches.StatusDataSwitch.TraceInfo, $"HandleStatusData Length 8 HashIndex {packageIndex}");
                if (packageIndex == 1)
                {
                    statusDataCollector = new();
                }
                statusDataCollector.AddData(msg.GetData());
                break;
            default:
                throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
            }
        }
    }

    #endregion
}
