namespace CentralStationWebApi.Serializer;

public static class TraceSwitches
{
    public readonly static TraceSwitch CanSendSwitch = new("CanSendSwitch", "CanSendSwitch", "Warning");

    public readonly static TraceSwitch CanReceiveSwitch = new("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    public readonly static TraceSwitch SerializerSwitch = new("SerializerSwitch", "SerializerSwitch", "Warning");


    public readonly static TraceSwitch StatusDataSwitch = new("StatusDataSwitch", "StatusDataSwitch", "Warning");


    public readonly static TraceSwitch TracksSwitch = new("TracksSwitch", "TracksSwitch", "Info");
}