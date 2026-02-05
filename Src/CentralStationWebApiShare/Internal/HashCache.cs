namespace CentralStationWebApi.Internal;

internal static class HashCache
{
    private static readonly Dictionary<ushort, string> cache = [];

    public static string GetHash(ushort hash)
    {
        if ((hash & 0xff80) == 0x0300)
        {
            return $"Index{hash & 0x007f}";
        }

        if (cache.TryGetValue(hash, out string? name))
        {
            return name;
        }
        return string.Empty;
    }

    public static void AddHash(ushort hash, ushort deviceType)
    {
        cache[hash] = ((DeviceType)deviceType).ToString();
    }

    public static void AddHash(ushort hash, string name)
    {
        cache[hash] = name;
    }
}
