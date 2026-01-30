namespace CentralStationWebApi.Internal;

internal static class DeviceCache
{
    private static readonly Dictionary<uint, string> deviceNames = new()
    {
        { 0x00000000, "All" },
        { 0xFFFFFFFF, "Broadcast" }
    };

    public static void AddDevice(uint id, ushort deviceType)
    {
        deviceNames[id] = ((DeviceType)deviceType).ToString();
    }

    public static void AddLocomotiveDevices(LocomotiveData locomotives)
    {
        foreach (var loco in locomotives.Locomotives ?? [])
        {
            if (!string.IsNullOrEmpty(loco.Name))
            {
                deviceNames[loco.Uid] = loco.Name;
            }
        }
    }

    public static string DeviceName(uint id)
    {
        return deviceNames.TryGetValue(id, out string? name) ? name : id.ToString("X8");
    }
}
