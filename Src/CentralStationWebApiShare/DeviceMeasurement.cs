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
        ValuePower = col.ReadByte();
        ColorRange1 = col.ReadByte();
        ColorRange2 = col.ReadByte();
        ColorRange3 = col.ReadByte();
        ColorRange4 = col.ReadByte();
        ZeroPoint = col.ReadByte();
        EndRange1 = col.ReadByte();
        EndRange2 = col.ReadByte();
        EndRange3 = col.ReadByte();
        EndRange4 = col.ReadByte();
        Name = col.ReadString();
        Start = col.ReadString();
        End = col.ReadString();
        Unit = col.ReadString();

    }

    public byte Channel { get; internal set; }
    public byte ValuePower { get; internal set; }
    public byte ColorRange1 { get; internal set; }
    public byte ColorRange2 { get; internal set; }
    public byte ColorRange3 { get; internal set; }
    public byte ColorRange4 { get; internal set; }
    public byte ZeroPoint { get; internal set; }
    public byte EndRange1 { get; internal set; }
    public byte EndRange2 { get; internal set; }
    public byte EndRange3 { get; internal set; }
    public byte EndRange4 { get; internal set; }
    public string Name { get; set; } = String.Empty;
    public string Start { get; internal set; } = String.Empty;
    public string End { get; internal set; } = String.Empty;
    public string Unit { get; internal set; } = String.Empty;

}
