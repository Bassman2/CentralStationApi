using System.Diagnostics.CodeAnalysis;

namespace CentralStationApi.Internal;

internal class CanStreamComparer : IEqualityComparer<CanMessage>
{
    public static CanStreamComparer Instance { get; } = new CanStreamComparer();

    public bool Equals(CanMessage? req, CanMessage? res)
    {
        DebugInfo($"Comparing req: {req?.ToTrace() ?? String.Empty} with res: {res?.ToTrace() ?? String.Empty}");

        bool f =
            req is not null &&
            res is not null &&
            req.Command == Command.ConfigDataRequest &&
            res.Command == Command.ConfigDataStream;
        DebugInfo($"Comparison result: {f}");
        return f;
    }

    public int GetHashCode([DisallowNull] CanMessage obj) => 0;

    [Conditional("DEBUG")]
    private static void DebugInfo(string text) => Debug.WriteLineIf(TraceSwitches.CanMessageHandlerSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} CanStreamComparer: {text}");
}
