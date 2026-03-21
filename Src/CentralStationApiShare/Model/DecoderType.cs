namespace CentralStationApi.Model;

/// <summary>
/// Specifies the type of decoder protocol used by a locomotive or accessory.
/// Different decoder types support different features and control methods.
/// </summary>
/// <remarks>
/// The decoder type determines how the Central Station communicates with the device,
/// including the available speed steps, function capabilities, and programming methods.
/// Some decoder types are variants that support special programming or configuration modes.
/// </remarks>
[EnumConverter]
public enum DecoderType
{
    /// <summary>
    /// Unknown or unspecified decoder type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Märklin Motorola 2 (MM2) decoder.
    /// Digital protocol for Märklin locomotives with 14 or 28 speed steps and up to 4 functions.
    /// </summary>
    [EnumMember(Value = "mm2")]
    MM2,

    /// <summary>
    /// Märklin Motorola 2 decoder in programming mode.
    /// Special mode for programming MM2 decoder addresses and settings.
    /// </summary>
    [EnumMember(Value = "mm2_prg")]
    MM2Prg,

    /// <summary>
    /// Märklin Motorola 2 decoder with DIL8 switch configuration.
    /// MM2 decoder that uses an 8-position DIP switch for address and configuration settings.
    /// </summary>
    [EnumMember(Value = "mm2_dil8")]
    MM2Dil8,

    /// <summary>
    /// Digital Command Control (DCC) decoder.
    /// Industry-standard NMRA/NRMA protocol supporting 14, 28, or 128 speed steps
    /// and up to 28+ functions depending on decoder capabilities.
    /// </summary>
    [EnumMember(Value = "dcc")]
    DCC,

    /// <summary>
    /// Märklin mfx (Motorola future extension) decoder.
    /// Advanced digital protocol with automatic registration, up to 1000 speed steps,
    /// 32 functions, and bidirectional communication for feedback and status information.
    /// </summary>
    [EnumMember(Value = "mfx")]
    MFX,

    /// <summary>
    /// Selectrix 1 (SX1) decoder.
    /// Alternative digital protocol used primarily in European model railways.
    /// </summary>
    [EnumMember(Value = "sx1")]
    ESX1,

    /// <summary>
    /// Märklin Motorola (MM) decoder in programming mode.
    /// Special mode for programming original MM decoder addresses and settings.
    /// </summary>
    [EnumMember(Value = "mm_prg")]
    MMPrg        
}
