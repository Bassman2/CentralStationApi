namespace CentralStationWebApi.Serializer;

public static class TraceSwitches
{
    public readonly static TraceSwitch CanSendSwitch = new("CanSendSwitch", "CanSendSwitch", "Warning");

    public readonly static TraceSwitch CanReceiveSwitch = new("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    public readonly static TraceSwitch SerializerSwitch = new("SerializerSwitch", "SerializerSwitch", "Warning");


    //public readonly static TraceSwitch StatusDataSwitch = new("StatusDataSwitch", "StatusDataSwitch", "Warning");


    public readonly static TraceSwitch TracksSwitch = new("TracksSwitch", "TracksSwitch", "Warning");

    public readonly static TraceSwitch ControllerSwitch = new("ControllerSwitch", "ControllerSwitch", "Warning");

    public readonly static TraceSwitch DevicesSwitch = new("DevicesSwitch", "DevicesSwitch", "Info");

    public readonly static TraceSwitch ConfigDataSwitch = new("ConfigDataSwitch", "ConfigDataSwitch", "Info");

    public readonly static TraceSwitch CanMessageHandlerSwitch = new("CanMessageHandlerSwitch", "CanMessageHandlerSwitch", "Info");

}