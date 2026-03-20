namespace CentralStationWebApi.Model;

/// <summary>
/// Represents a model locomotive controlled by the Märklin Central Station.
/// Contains locomotive properties, decoder configuration, and current state information.
/// </summary>
[CsSerialize]
public partial class Locomotive
{
    /// <summary>
    /// Gets the name of the locomotive.
    /// </summary>
    [CsProperty("name")]
    public string? Name { get; private set; }

    /// <summary>
    /// Gets the first name (alternative name) of the locomotive.
    /// </summary>
    [CsProperty("vorname")]
    public string? FirstName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the locomotive.
    /// </summary>
    [CsProperty("uid")]
    public uint Uid { get; private set; }

    /// <summary>
    /// Gets the MFX unique identifier of the locomotive.
    /// </summary>
    [CsProperty("mfxuid")]
    public uint MfxUid { get; private set; }

    /// <summary>
    /// Gets the decoder address of the locomotive.
    /// </summary>
    [CsProperty("adresse")]
    public uint Address { get; private set; }

    /// <summary>
    /// Gets the icon file name for the locomotive.
    /// </summary>
    [CsProperty("icon")]
    public string? IconName { get; private set; }

    /// <summary>
    /// Gets the full URI to the locomotive's icon image.
    /// </summary>
    public Uri? IconUri => IconName != null ? new Uri(CentralStation.LocoUri, $"{IconName}.png") : null;

    /// <summary>
    /// Gets the decoder type of the locomotive.
    /// </summary>
    [CsProperty("typ")]
    public DecoderType Type { get; private set; }

    /// <summary>
    /// Locomotive symbol for MS1 (0=electric locomotive, 1=diesel locomotive, 2=steam locomotive, 3=no icon)
    /// </summary>
    [CsProperty("symbol")]
    public Symbol Symbol { get; private set; }

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    [CsProperty("sid")]
    public uint Sid { get; private set; }

    /// <summary>
    /// Final reading of the speedometer.
    /// </summary>
    [CsProperty("tachomax")]
    public uint MaxSpeed { get; private set; }

    /// <summary>
    /// Maximum speed of the locomotive
    /// </summary>
    [CsProperty("vmax")]
    public uint VMax { get; private set; }

    /// <summary>
    /// Minimum speed of the locomotive
    /// </summary>
    [CsProperty("vmin")]
    public uint VMin { get; private set; }

    /// <summary>
    /// Delay in starting.
    /// </summary>
    [CsProperty("av")]
    public uint Av { get; private set; }

    /// <summary>
    /// Braking deceleration
    /// </summary>
    [CsProperty("bv")]
    public uint Bv { get; private set; }

    /// <summary>
    /// Gets the volume setting of the locomotive.
    /// </summary>
    [CsProperty("volume")]
    public uint Volume { get; private set; }

    /// <summary>
    /// Gets the spa value.
    /// </summary>
    [CsProperty("spa")]
    public uint Spa { get; private set; }

    /// <summary>
    /// Gets the spm value.
    /// </summary>
    [CsProperty("spm")]
    public uint Spm { get; private set; }

    /// <summary>
    /// Internal marker for locomotive programming, values ​​to be programmed
    /// </summary>
    [CsProperty("progmask")]
    public uint ProgMask { get; private set; }

    /// <summary>
    /// Gets the function type value.
    /// </summary>
    [CsProperty("ft")]
    public uint Ft { get; private set; }

    /// <summary>
    /// Gets the extended protocol value.
    /// </summary>
    [CsProperty("xprot")]
    public uint Xprot { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the locomotive is part of a traction unit.
    /// </summary>
    [CsProperty("inTraktion")]
    public uint InTraction { get; private set; }

    /// <summary>
    /// Gets the current velocity of the locomotive.
    /// </summary>
    [CsProperty("velocity")]
    public ushort Velocity { get; private set; }

    /// <summary>
    /// Gets the current direction of travel.
    /// </summary>
    [CsProperty("richtung")]
    public Direction Direction { get; private set; }

    /// <summary>
    /// Gets the MFX locomotive type.
    /// </summary>
    [CsProperty("mfxtyp")]
    public uint MfxType { get; private set; }

    /// <summary>
    /// Gets the memory blocks for the locomotive configuration.
    /// </summary>
    [CsProperty("blocks")]
    public uint[]? Blocks { get; private set; }

    /// <summary>
    /// Gets the list of available functions for the locomotive.
    /// </summary>
    [CsProperty("funktionen")]
    public List<Function>? Functions { get; private set; }

    /// <summary>
    /// Gets the extended list of available functions for the locomotive.
    /// </summary>
    [CsProperty("funktionen_2")]
    public List<Function>? Functions2 { get; private set; }
}
