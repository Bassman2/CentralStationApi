namespace CentralStationApi;

/// <summary>
/// Specifies the type of device on the Märklin Central Station network.
/// Each device type has a unique identifier used in CAN bus communication.
/// </summary>
/// <remarks>
/// Device types identify various hardware components in the model railway system including
/// control stations, boosters, decoders, feedback modules, and client applications.
/// The device type is reported in response to <see cref="Command.SoftwareVersion"/> requests
/// and is used to identify device capabilities and compatibility.
/// </remarks>
public enum DeviceType : ushort
{
    /// <summary>
    /// Gleis Format Prozessor (Track Format Processor) / GFP booster.
    /// </summary>
    /// <remarks>
    /// Compatible devices:
    /// <list type="bullet">
    /// <item><description>60213 Central Station (2008 - 2009)</description></item>
    /// <item><description>60214 Central Station (2009 - 2011)</description></item>
    /// <item><description>60173 Booster (2008 - 2010) for H0 and 1 gauge</description></item>
    /// <item><description>60174 Booster (2010 - 2016)</description></item>
    /// </list>
    /// </remarks>
    [Description("GFP")]
    [FileName("dashboard_gfp3.png")]
    GFP = 0x0000,

    /// <summary>
    /// Gleis Format Prozessor 2 (Track Format Processor 2) / GFP2 from Central Station 2.
    /// </summary>
    /// <remarks>
    /// Specific device type details are unknown.
    /// </remarks>
    [Description("GFP2")]
    [FileName("dashboard_gfp3.png")]
    GFP2 = 0x0001, // ?????????

    /// <summary>
    /// Digital Connector Box (Gleisbox).
    /// </summary>
    /// <remarks>
    /// Compatible devices:
    /// <list type="bullet">
    /// <item><description>60112 for 1 Gauge</description></item>
    /// <item><description>60113 for Märklin H0, Trix H0 and Minitrix</description></item>
    /// </list>
    /// </remarks>
    [Description("Digital Connector Box")]
    [FileName("dashboard_cs1.png")]
    DCB = 0x0010,

    /// <summary>
    /// Digital Connector Box variant 1.
    /// </summary>
    [Description("Digital Connector Box")]
    [FileName("dashboard_cs2.png")]
    DCB1 = 0x0011,

    /// <summary>
    /// Connect 6021 adapter (60128).
    /// Adapter for connecting legacy 6021 control units to the digital system.
    /// </summary>
    [Description("Connect 6021")]
    [FileName("dashboard_cs2.png")]
    Connect = 0x0020,

    /// <summary>
    /// Mobile Station 2 (60653).
    /// Compact handheld control station.
    /// </summary>
    [Description("Mobile Station 2")]
    [FileName("dashboard_ms1.png")]
    MS2 = 0x0030,

    /// <summary>
    /// Mobile Station 2 variant 1 (60653).
    /// </summary>
    [Description("Mobile Station 2")]
    [FileName("dashboard_ms1.png")]
    MS2_1 = 0x0031,

    /// <summary>
    /// Mobile Station 2 variant 2 (60653).
    /// </summary>
    [Description("Mobile Station 2")]
    [FileName("dashboard_ms2.png")] 
    MS2_2 = 0x0032,

    /// <summary>
    /// Mobile Station 2 variant 3 (60653).
    /// Software version 5.6 confirmed on actual device.
    /// </summary>
    [Description("Mobile Station 2")]
    [FileName("dashboard_ms2.png")] 
    MS2_3 = 0x0033,

    /// <summary>
    /// Mobile Station 2 variant 4 (60653).
    /// </summary>
    [Description("Mobile Station 2")]
    [FileName("dashboard_ms2.png")] 
    MS2_4 = 0x0034,

    /// <summary>
    /// Mobile Station with WLAN capabilities (60653).
    /// Mobile Station 2 with integrated wireless network connectivity.
    /// </summary>
    [Description("Mobile Station WLAN")]
    [FileName("dashboard_ms2.png")]
    MS_WLAN = 0x0035,

    /// <summary>
    /// S88 Link module for S88 feedback bus connectivity.
    /// </summary>
    /// <remarks>
    /// SRSEII (non-Märklin product) compatible S88 feedback interface.
    /// </remarks>
    [Description("S88 Link")]
    [FileName("dashboard_links88.png")]
    LinkS88 = 0x0040,

    /// <summary>
    /// Gleis Format Prozessor 3 (Track Format Processor 3) / GFP3 from Central Station 3.
    /// </summary>
    /// <remarks>
    /// Compatible devices:
    /// <list type="bullet">
    /// <item><description>60226 Central Station 3 (confirmed on original device)</description></item>
    /// <item><description>60216 Central Station 3 plus</description></item>
    /// </list>
    /// </remarks>
    [Description("GFP3 from CS3")]
    [FileName("dashboard_gfp3.png")]
    GFP3 = 0x0050,

    /// <summary>
    /// Application / Client software.
    /// Generic identifier for computer applications connecting to the Central Station.
    /// </summary>
    Application = 0x1000,

    /// <summary>
    /// Central Station 2 software application.
    /// Software GUI application running on Central Station 2.
    /// </summary>
    [Description("Central Station 2")]
    [FileName("dashboard_cs2.png")]
    CS2 = 0xeeee,

    /// <summary>
    /// Wireless controllers (tablets, smartphones).
    /// Mobile applications connecting via WiFi to control the layout.
    /// </summary>
    [Description("Wireless Controllers")]
    [FileName("dashboard_tablet.png")]
    Wireless = 0xffe0,

    /// <summary>
    /// Wired controller / CS2 Slave.
    /// Physical wired controller connected to the Central Station.
    /// </summary>
    [Description("Wired")]
    [FileName("dashboard_cs3.png")]
    Wired = 0xfff0,

    /// <summary>
    /// Central Station Master GUI.
    /// Main graphical user interface for Central Station control.
    /// </summary>
    /// <remarks>
    /// Compatible devices:
    /// <list type="bullet">
    /// <item><description>Central Station 2 GUI</description></item>
    /// <item><description>60226 GUI Central Station 3 (confirmed on original device)</description></item>
    /// <item><description>60216 GUI Central Station 3 plus</description></item>
    /// </list>
    /// </remarks>
    [Description("GUI")]
    [FileName("dashboard_cs3.png")]
    GUI = 0xffff        
}
