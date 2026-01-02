namespace CentralStationWebApi;

public class StatusDataValue
{
    // Package 1

    public byte Channel { get; internal set; }

    public byte Potency { get; internal set; }

    public byte Color1 { get; internal set; }

    public byte Color2 { get; internal set; }

    public byte Color3 { get; internal set; }

    public byte Color4 { get; internal set; }

    public ushort ZeroPoint { get; internal set; }

    // Package 2

    public ushort End1 { get; internal set; }

    public ushort End2 { get; internal set; }

    public ushort End3 { get; internal set; }

    public ushort End4 { get; internal set; }

    // Package 3 ,4 , 5

    public string MeasurementDesignation { get; internal set; } = string.Empty;

    public string Start { get; internal set; } = string.Empty;

    public string End { get; internal set; } = string.Empty;

    public string Unit { get; internal set; } = string.Empty;
}
