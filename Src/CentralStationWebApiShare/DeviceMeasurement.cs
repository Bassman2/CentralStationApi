using System.Drawing;

namespace CentralStationWebApi;

public class DeviceMeasurement
{
    public DeviceMeasurement()
    { }

    internal DeviceMeasurement(DataCollector col)
    {
        col.SetPositionToStart();
        
        Channel = col.ReadByte();
        ValuePower = col.ReadSByte();
        ColorRange1 = col.ReadByte();
        ColorRange2 = col.ReadByte();
        ColorRange3 = col.ReadByte();
        ColorRange4 = col.ReadByte();
        ZeroPoint = col.ReadUInt16();
        EndRange1 = col.ReadUInt16();
        EndRange2 = col.ReadUInt16();
        EndRange3 = col.ReadUInt16();
        EndRange4 = col.ReadUInt16();
        Name = col.ReadString();
        Start = col.ReadString();
        End = col.ReadString();
        Unit = col.ReadString();
    }

    public byte Channel { get; internal set; }
    public sbyte ValuePower { get; internal set; }
    public byte ColorRange1 { get; internal set; }
    public byte ColorRange2 { get; internal set; }
    public byte ColorRange3 { get; internal set; }
    public byte ColorRange4 { get; internal set; }
    public ushort ZeroPoint { get; internal set; }
    public ushort EndRange1 { get; internal set; }
    public ushort EndRange2 { get; internal set; }
    public ushort EndRange3 { get; internal set; }
    public ushort EndRange4 { get; internal set; }
    public string Name { get; set; } = String.Empty;
    public string Start { get; internal set; } = String.Empty;
    public string End { get; internal set; } = String.Empty;
    public string Unit { get; internal set; } = String.Empty;

}
