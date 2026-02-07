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
            Command.SystemCommand => req.SubCommand == res.SubCommand && req.DeviceId == res.DeviceId,
            Command.Discovery => true,
            Command.Bind => req.DeviceId == res.DeviceId && req.GetDataUShort(4) == res.GetDataUShort(4),
            Command.Verify => req.DeviceId == res.DeviceId,
            Command.LocoVelocity => req.DeviceId == res.DeviceId,
            Command.LocoDirection => req.DeviceId == res.DeviceId,
            Command.LocoFunction => req.DeviceId == res.DeviceId && req.GetDataByte(4) == res.GetDataByte(4),
            Command.ReadConfig => req.DeviceId == res.DeviceId,
            Command.WriteConfig => req.DeviceId == res.DeviceId,
            Command.SwitchAccessories => req.DeviceId == res.DeviceId,
            Command.ACC_CONFIG => req.DeviceId == res.DeviceId,
            Command.S88Polling => req.DeviceId == res.DeviceId,
            Command.S88Event => req.DeviceId == res.DeviceId,
            Command.SX1Event => req.DeviceId == res.DeviceId,
            Command.SoftwareVersion => false,   // not used with single result message
            //Command.UpdateOffer => req.DeviceId == res.DeviceId,
            //Command.ReadConfigData => req.DeviceId == res.DeviceId,
            Command.BootloaderCANBound => req.DeviceId == res.DeviceId,
            Command.BootloaderRailBound => req.DeviceId == res.DeviceId,
            Command.StatusData => req.DeviceId == res.DeviceId,
            Command.ConfigData => req.DeviceId == res.DeviceId,
            Command.ConfigDataStream => req.DeviceId == res.DeviceId,
            Command.DataStream6021Adapter => req.DeviceId == res.DeviceId,
            Command.AutomaticTransmission => req.DeviceId == res.DeviceId && req.GetDataByte(4) == res.GetDataByte(4),
            Command.DebugMessage => req.DeviceId == res.DeviceId,
            _ => false,
        };
    }
}
