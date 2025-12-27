namespace CentralStationWebApi.Serializer;

public static class TraceSwitches
{
    public static TraceSwitch CanSendSwitch = new TraceSwitch("CanSendSwitch", "CanSendSwitch", "Warning");

    public static TraceSwitch CanReceiveSwitch = new TraceSwitch("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    public static TraceSwitch SerializerSwitch = new TraceSwitch("SerializerSwitch", "SerializerSwitch", "Warning");
}