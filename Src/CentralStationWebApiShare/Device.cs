namespace CentralStationWebApi;

public class Device
{
    internal Device(CANMessage msg)
    {
        DeviceId = msg.Device;
        MajorVersion = msg.GetDataByte(4);
        MinorVersion = msg.GetDataByte(5);
        DeviceType = (DeviceType)msg.GetDataUShort(6);
        DeviceTypeName = DeviceType.GetDescription();
        IconUri = DeviceType.GetFileNamePath(CentralStation.GuiUri); 
    }

    //internal Device(uint id, DeviceType deviceType)
    //{
    //    DeviceId = id;
    //    MajorVersion = 1;
    //    MinorVersion = 2;
    //    DeviceType = deviceType;
    //}

    internal void AddData(int index, DataCollector col)
    {
        col.SetPositionToStart();
        if (index == 0)
        {
            NumOfMeasuredValues = col.ReadByte();
            NumOfConfigurationChannels = col.ReadByte();
            col.ReadByte();
            col.ReadByte();
            SerialNumber = col.ReadUInt32();
            ArticleNumber = col.ReadString(8);
            DeviceName = col.ReadString();
        }
        else
        {
        }
    }

    // from SoftwareVersion
    public uint DeviceId { get; internal set; }
    public byte MajorVersion { get; internal set; }
    public byte MinorVersion { get; internal set; }
    public DeviceType DeviceType { get; internal set; }
    public string DeviceTypeName { get; internal set; }

    public Uri? IconUri { get; internal set; }
   
    // from Device Description (index 0)

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    public string ArticleNumber { get; internal set; } = String.Empty;

    public string DeviceName { get; internal set; } = String.Empty;


    /// <summary>
    /// ///
    /// </summary>

    //internal bool IsNeedData { get; set; } = true;

    //internal bool IsReady { get; set; } = false;

    //public byte Index { get; set; }
    //public byte NumOfPackages { get; set; }
}
