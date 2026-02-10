using System.Diagnostics.CodeAnalysis;

namespace CentralStationWebApi.Internal;

internal class CanMessageCollectorComparer : IEqualityComparer<CanMessage>
{
    public static CanMessageComparer Instance { get; } = new CanMessageComparer();

    public bool Equals(CanMessage? req, CanMessage? res)
    {
        DebugInfo($"Comparing req: {req?.ToTrace() ?? String.Empty} with res: {res?.ToTrace() ?? String.Empty}");

        bool f = 
            req is not null &&
            res is not null &&
            // config data 
            ((
                req.Command == Command.ConfigData &&
                res.Command == Command.ConfigDataStream
            ) 
            ||
            // status data
            (
                req.Command == Command.StatusData && 
                res.Command == Command.StatusData &&  res.IsResponse && 
                (
                    res.DataLength == 8 || ( res.DataLength == 6 && req.DeviceId == res.DeviceId)
                )
            ));
        DebugInfo($"Comparison result: {f}");
        return f;
    }

    public int GetHashCode([DisallowNull] CanMessage obj)
    {
        return 0;
    }

    [Conditional("DEBUG")]
    private static void DebugInfo(string text) => Debug.WriteLineIf(TraceSwitches.CanMessageHandlerSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} CanMessageCollectorComparer: {text}");

}
