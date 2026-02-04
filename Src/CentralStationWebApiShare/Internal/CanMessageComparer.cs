using System.Diagnostics.CodeAnalysis;

namespace CentralStationWebApi.Internal;

internal class CanMessageComparer : IEqualityComparer<CanMessage>
{
    public static CanMessageComparer Instance { get; } = new CanMessageComparer();

    public bool Equals(CanMessage? req, CanMessage? res)
    {
        return
            // identical messages to remove after timeout or cancel 
            req == res || 
            // compare req and res message
            (
                req is not null && 
                res is not null &&
                res.IsResponse &&
                req.Command == res.Command &&

                Comparer(req, res)
            );        
    }

    public int GetHashCode([DisallowNull] CanMessage obj)
    {
        return 0;
    }

    public static bool Comparer(CanMessage req, CanMessage res)
    {
        return req.Command switch
        {
            Command.LocoVelocity => req.DeviceId == res.DeviceId,
            Command.LocoDirection => req.DeviceId == res.DeviceId,
            Command.LocoFunction => req.DeviceId == res.DeviceId,
            _ => false,
        };
    }
}
