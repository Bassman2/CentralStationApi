namespace CentralStationDemo;

public static class AppTraceSwitches
{
    public static readonly TraceSwitch DevicesSwitch = new("DevicesSwitch", "DevicesSwitch", "Info");

    //public static TraceSwitch CanReceiveSwitch = new TraceSwitch("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    //public static TraceSwitch SerializerSwitch = new TraceSwitch("SerializerSwitch", "SerializerSwitch", "Warning");
}
