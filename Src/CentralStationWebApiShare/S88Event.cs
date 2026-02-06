namespace CentralStationWebApi;

public class S88Event
{
    public byte OldValue { get; internal set; }
    public byte NewValue { get; internal set; }

    public ushort Time { get; internal set; }
}
