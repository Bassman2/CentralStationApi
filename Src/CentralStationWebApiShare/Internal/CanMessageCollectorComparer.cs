using System.Diagnostics.CodeAnalysis;

namespace CentralStationWebApi.Internal;

internal class CanMessageCollectorComparer : IEqualityComparer<CanMessage>
{
    public static CanMessageComparer Instance { get; } = new CanMessageComparer();

    public bool Equals(CanMessage? req, CanMessage? res)
    {
        return
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
    }

    public int GetHashCode([DisallowNull] CanMessage obj)
    {
        return 0;
    }
}
