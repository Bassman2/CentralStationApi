namespace CentralStationWebApi;

public class Device
{
    internal Device(CANMessage msg)
    {
        DeviceId = msg.Device;
        MajorVersion = msg.GetDataByte(4);
        MinorVersion = msg.GetDataByte(5);
        DeviceType = (DeviceType)msg.GetDataUShort(6);
    }

    internal Device(uint id, DeviceType deviceType)
    {
        DeviceId = id;
        MajorVersion = 1;
        MinorVersion = 2;
        DeviceType = deviceType;
    }

    // from SoftwareVersion
    public uint DeviceId { get; internal set; }
    public byte MajorVersion { get; internal set; }
    public byte MinorVersion { get; internal set; }
    public DeviceType DeviceType { get; internal set; }

    public Uri? IconUri
    {
        get
        {
            string? fileName = FileNameAttribute.GetFilename(DeviceType);
            return fileName is null ? null : new Uri($"http://{CentralStationBasic.Host}/images/gui/{fileName}");
        }
    }

    //public Uri? IconUri
    //{
    //    get
    //    {

    //        var memberInfo = typeof(DeviceType).GetMember(DeviceType.ToString()).FirstOrDefault();
    //        if (memberInfo != null)
    //        {
    //            var attr = memberInfo.GetCustomAttributes(typeof(FileNameAttribute), false)
    //                                 .OfType<FileNameAttribute>()
    //                                 .FirstOrDefault();
    //            if (attr != null)
    //            {
    //                return new Uri($"http://{CentralStationBasic.Host}/{attr.FileName}");
    //            }
    //        }
    //        return null; // new Uri($"http://{CentralStationBasic.Host}/images/gui/dashboard_cs3.png");
    //    }
    //}
}
