using System.Reflection.Metadata;

namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Locomotive
{
    [CsProperty("name")]
    public string? Name { get; private set; }

    [CsProperty("vorname")]
    public string? FirstName { get; private set; }

    [CsProperty("uid")]
    public uint Uid { get; private set; }

    [CsProperty("mfxuid")]
    public uint MfxUid { get; private set; }

    [CsProperty("adresse")]
    public uint Address { get; private set; }

    [CsProperty("icon")]
    public string? IconName { get; private set; }

    public Uri? IconUri => IconName != null ? new Uri($"http://{CentralStationBasic.Host}/app/assets/lok/{IconName}.png") : null;

    [CsProperty("typ")]
    public DecoderType Type { get; private set; }

    /// <summary>
    /// Locomotive symbol for MS1 (0=electric locomotive, 1=diesel locomotive, 2=steam locomotive, 3=no icon)
    /// </summary>
    [CsProperty("symbol")]
    public Symbol Symbol { get; private set; }

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

    [CsProperty("volume")]
    public uint Volume { get; private set; }

    [CsProperty("spa")]
    public uint Spa { get; private set; }

    [CsProperty("spm")]
    public uint Spm { get; private set; }

    /// <summary>
    /// Internal marker for locomotive programming, values ​​to be programmed
    /// </summary>
    [CsProperty("progmask")]
    public uint ProgMask { get; private set; }

    [CsProperty("ft")]
    public uint Ft { get; private set; }

    [CsProperty("xprot")]
    public uint Xprot { get; private set; }

    [CsProperty("inTraktion")]
    public uint InTraction { get; private set; }

    [CsProperty("velocity")]
    public uint Velocity { get; private set; }

    [CsProperty("richtung")]
    public Direction Direction { get; private set; }

    //
    [CsProperty("mfxtyp")]
    public uint MfxType { get; private set; }

    [CsProperty("blocks")]
    public uint[]? Blocks { get; private set; }

    [CsProperty("funktionen")]
    public List<Function>? Functions { get; private set; }

    [CsProperty("funktionen_2")]
    public List<Function>? Functions2 { get; private set; }
}
