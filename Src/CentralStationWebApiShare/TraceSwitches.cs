namespace CentralStationWebApi.Serializer;

/// <summary>
/// Provides trace switches for controlling diagnostic output in the Central Station API.
/// Each switch controls tracing for a specific component or operation.
/// </summary>
/// <remarks>
/// Trace switches can be configured in the application's configuration file (app.config or appsettings.json)
/// to control the verbosity of diagnostic output. Default level is "Warning" unless otherwise specified.
/// Trace levels: Off, Error, Warning, Info, Verbose.
/// </remarks>
public static class TraceSwitches
{
    //public readonly static TraceSwitch CanSendSwitch = new("CanSendSwitch", "CanSendSwitch", "Warning");

    /// <summary>
    /// Trace switch for CAN message receiving operations.
    /// Controls diagnostic output for messages received from the Central Station.
    /// Default level: Warning.
    /// </summary>
    public readonly static TraceSwitch CanReceiveSwitch = new("CanReceiveSwitch", "CanReceiveSwitch", "Warning");

    /// <summary>
    /// Trace switch for serialization and deserialization operations.
    /// Controls diagnostic output for parsing Central Station configuration data (CS2/CS3 format).
    /// Default level: Warning.
    /// </summary>
    public readonly static TraceSwitch SerializerSwitch = new("SerializerSwitch", "SerializerSwitch", "Warning");


    ////public readonly static TraceSwitch StatusDataSwitch = new("StatusDataSwitch", "StatusDataSwitch", "Warning");


    //public readonly static TraceSwitch TracksSwitch = new("TracksSwitch", "TracksSwitch", "Warning");

    //public readonly static TraceSwitch ControllerSwitch = new("ControllerSwitch", "ControllerSwitch", "Warning");

    //public readonly static TraceSwitch DevicesSwitch = new("DevicesSwitch", "DevicesSwitch", "Info");

    //public readonly static TraceSwitch ConfigDataSwitch = new("ConfigDataSwitch", "ConfigDataSwitch", "Info");

    /// <summary>
    /// Trace switch for CAN message handler operations.
    /// Controls diagnostic output for message queueing, response matching, and timeout handling.
    /// Default level: Info.
    /// </summary>
    public readonly static TraceSwitch CanMessageHandlerSwitch = new("CanMessageHandlerSwitch", "CanMessageHandlerSwitch", "Info");

}