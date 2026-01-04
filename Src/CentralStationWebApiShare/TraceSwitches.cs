namespace CentralStationWebApi.Serializer;

public static class TraceSwitches
{
    public readonly static TraceSwitch CanSendSwitch = new TraceSwitch("CanSendSwitch", "CanSendSwitch", "Warning");

    public readonly static TraceSwitch CanReceiveSwitch = new TraceSwitch("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    public readonly static TraceSwitch SerializerSwitch = new TraceSwitch("SerializerSwitch", "SerializerSwitch", "Warning");


    public readonly static TraceSwitch StatusDataSwitch = new TraceSwitch("StatusDataSwitch", "StatusDataSwitch", "Info");
}