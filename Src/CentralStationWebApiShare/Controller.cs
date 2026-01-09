namespace CentralStationWebApi;

public class Controller(CANMessage msg, string host) 
{
    // from SoftwareVersion
    public uint DeviceId => msg.Device;
    public byte MajorVersion => msg.GetDataByte(4);
    public byte MinorVersion => msg.GetDataByte(5);
    public DeviceType DeviceType => (DeviceType)msg.GetDataUShort(6);

    // from StatusData

    internal bool HasStatusData { get; set; } = false;

    public byte Index { get; internal set; }

    public byte NumOfPackages { get; internal set; }

    // Package 1

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    // Package 2

    public string? ArticleNumber { get; internal set; }

    // Package 3 - 5
    public string DeviceName { get; internal set; } = String.Empty;

    public Uri? IconUri
    {
        get
        {
            string? fileName = DeviceType switch
            {
                DeviceType.GFP => "dashboard_gfp3",             // http://cs3/images/gui/dashboard_gfp3.png  
                DeviceType.DCB => "dashboard_cs1",              // http://cs3/images/gui/dashboard_cs1.png
                DeviceType.DCB1 => "dashboard_cs1",             // http://cs3/images/gui/dashboard_cs1.png
                DeviceType.Connect => "dashboard_cs2",          // http://cs3/images/gui/dashboard_cs2.png
                DeviceType.MS2 => "dashboard_ms2",              // http://cs3/images/gui/dashboard_ms1.png
                DeviceType.MS2_1 => "dashboard_ms2",            // http://cs3/images/gui/dashboard_ms1.png
                DeviceType.MS2_2 => "dashboard_ms2",            // http://cs3/images/gui/dashboard_ms2.png
                DeviceType.MS2_3 => "dashboard_ms2",            // http://cs3/images/gui/dashboard_ms2.png
                DeviceType.MS2_4 => "dashboard_ms2",            // http://cs3/images/gui/dashboard_ms2.png
                DeviceType.LinkS88 => "dashboard_links88",      // http://cs3/images/gui/dashboard_links88.png
                DeviceType.GFP3 => "dashboard_gfp3",            // http://cs3/images/gui/dashboard_gfp3.png
                DeviceType.CS2 => "dashboard_cs2",              // http://cs3/images/gui/dashboard_cs2.png
                DeviceType.Wireless => "dashboard_tablet",      // http://cs3/images/gui/dashboard_tablet.png
                DeviceType.Wired => "dashboard_cs3",            // http://cs3/images/gui/dashboard_cs3.png
                DeviceType.GUI => "dashboard_cs3",              // http://cs3/images/gui/dashboard_cs3.png

                // http://cs3/images/gui/dashboard_desktop.png
                // http://cs3/images/gui/dashboard_booster.png
                // http://cs3/images/gui/dashboard_smartphone.png
                _ => null,
            };
            return fileName is not null ? new Uri($"http://{host}/images/gui/{fileName}.png") : null;
        }
    }

    internal void Add(int index, DataCollector col)
    {
        col.SetPositionToStart();
        if (index == 0)
        {
            // package 1
            NumOfMeasuredValues = col.ReadByte();
            NumOfConfigurationChannels = col.ReadByte();
            col.ReadByte();
            col.ReadByte();
            SerialNumber = col.ReadUInt32();

            // package 2
            ArticleNumber = col.ReadString(8);

            // package 3, ...
            DeviceName = col.ReadString();

            HasStatusData = true;
        }
        else
        {
            // for future use
        }
    }

    //public void Update(StatusDataDevice statusDataDevice)
    //{
    //    HasStatusData = true;
    //    Index = statusDataDevice.Index;
    //    NumOfPackages = statusDataDevice.NumOfPackages;
    //    NumOfMeasuredValues = statusDataDevice.NumOfMeasuredValues;
    //    NumOfConfigurationChannels = statusDataDevice.NumOfConfigurationChannels;
    //    SerialNumber = statusDataDevice.SerialNumber;
    //    ArticleNumber = statusDataDevice.ArticleNumber;
    //    DeviceName = statusDataDevice.DeviceName;
    //}

    public override int GetHashCode() => (DeviceId, MajorVersion, MinorVersion, DeviceType).GetHashCode();
    public override bool Equals(object? obj)
    {
        if (obj is Controller device)
        {
            return 
                DeviceId == device.DeviceId && 
                MajorVersion == device.MajorVersion && 
                MinorVersion == device.MinorVersion && 
                DeviceType == device.DeviceType;
        }
        return false;
    }
    
}
