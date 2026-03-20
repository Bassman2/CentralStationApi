namespace CentralStationWebApi;

/// <summary>
/// Represents the digital control protocols supported by the Märklin Central Station for track communication.
/// This is a flags enumeration allowing multiple protocols to be combined.
/// </summary>
[Flags]
public enum TrackProtocol : byte
{
    /// <summary>
    /// Märklin Motorola 2 (MM2) protocol.
    /// Traditional Märklin digital control protocol for locomotives and accessories.
    /// </summary>
    MM2 = 0x01,

    /// <summary>
    /// Märklin mfx (Motorola future extension) protocol.
    /// Advanced digital protocol with automatic locomotive registration and enhanced features.
    /// </summary>
    MFX = 0x02,

    /// <summary>
    /// Digital Command Control (DCC) protocol.
    /// Industry-standard NMRA/NRMA digital control protocol for model railways.
    /// </summary>
    DCC = 0x04
}
