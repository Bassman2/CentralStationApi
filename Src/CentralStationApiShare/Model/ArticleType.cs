namespace CentralStationApi.Model;

/// <summary>
/// Represents the type of article (accessory or device) in the Märklin Central Station system.
/// Includes various track accessories like turnouts, signals, lights, and other railway equipment.
/// </summary>
[EnumConverter]
public enum ArticleType
{
    /// <summary>
    /// Unknown or unspecified article type.
    /// </summary>
    Unknown = 0,

    #region Light

    /// <summary>
    /// Park street lamp lighting.
    /// </summary>
    [EnumMember(Value = "licht_str_lampe")]
    [Description("Park")]
    [FileName("magicon_a_033_01.svg")]
    LightStreetLamp,

    /// <summary>
    /// Track neon street lighting (Bahn).
    /// </summary>
    [EnumMember(Value = "licht_str_neon")]
    [Description("Track")]
    [FileName("magicon_a_034_01.svg")]
    LightStreetNeon,

    /// <summary>
    /// Street mast lighting.
    /// </summary>
    [EnumMember(Value = "licht_mast")]
    [Description("Street")]
    [FileName("magicon_a_035_01.svg")]
    LightStreet,

    /// <summary>
    /// House lamp lighting.
    /// </summary>
    [EnumMember(Value = "licht_lampe")]
    [Description("House")]
    [FileName("magicon_a_036_01.svg")]
    LightLamp,

    /// <summary>
    /// Neon lighting for buildings or scenes.
    /// </summary>
    [EnumMember(Value = "licht_neon")]
    [Description("Save")]
    [FileName("magicon_a_037_01.svg")]
    LightNeon,

    /// <summary>
    /// Vintage style park lighting.
    /// </summary>
    [EnumMember(Value = "licht_park")]
    [Description("Vintage Park")]
    [FileName("magicon_a_056_01.svg")]
    LightParkClassic,

    /// <summary>
    /// Outside/outdoor lighting.
    /// </summary>
    [EnumMember(Value = "licht_aussen")]
    [Description("Outside")]
    [FileName("magicon_a_062_01.svg")]
    LightOutside,

    /// <summary>
    /// Yellow signal lamp.
    /// </summary>
    [EnumMember(Value = "signallampe_ge")]
    [Description("Yellow")]
    [FileName("magicon_a_058_01.svg")]
    LightYellow,

    /// <summary>
    /// Red signal lamp.
    /// </summary>
    [EnumMember(Value = "signallampe_rt")]
    [Description("Red")]
    [FileName("magicon_a_059_01.svg")]
    LightRed,

    /// <summary>
    /// Green signal lamp.
    /// </summary>
    [EnumMember(Value = "signallampe_gn")]
    [Description("Green")]
    [FileName("magicon_a_060_01.svg")]
    LightGreen,

    /// <summary>
    /// Blue signal lamp.
    /// </summary>
    [EnumMember(Value = "signallampe_bl")]
    [Description("Blue")]
    [FileName("magicon_a_061_01.svg")]
    LightBlue,

    /// <summary>
    /// Quadcolor signal lamp with four color states.
    /// <list type="bullet">
    /// <item><description>0: gray</description></item>
    /// <item><description>1: red</description></item>
    /// <item><description>2: green</description></item>
    /// <item><description>3: yellow</description></item>
    /// </list>
    /// </summary>
    [EnumMember(Value = "profi_signallampe_4")]
    [Description("Quadcolor")]
    [FileName("magicon_a_057_01.svg")]
    LightQuadcolor,


    ///// <summary>
    ///// Light outdoor  
    ///// </summary>
    //[EnumMember(Value = "licht_aussen")]
    //[FileName("magicon_a_062_01.svg")]
    //LightOutdoor,

    #endregion

    #region Turnouts

    /// <summary>
    /// Right turnout switch.
    /// </summary>
    [EnumMember(Value = "rechtsweiche")]
    [Description("Right Turnout")]
    [FileName("magicon_a_005_01.svg")]
    RightTurnout,

    /// <summary>
    /// Left turnout switch.
    /// </summary>
    [EnumMember(Value = "linksweiche")]
    [Description("Left Turnout")]
    [FileName("magicon_a_006_01.svg")]
    LeftTurnout,

    /// <summary>
    /// Y-shaped turnout switch.
    /// </summary>
    [EnumMember(Value = "y_weiche")]
    [Description("Y Turnout")]
    [FileName("magicon_a_007_01.svg")]
    YSwitch,

    /// <summary>
    /// Double slip switch with 2 drives (DKW - Doppelkreuzweiche).
    /// </summary>
    [EnumMember(Value = "dkw_2antriebe")]
    [Description("Double Slip Switch 2")]
    [FileName("magicon_a_011_01.svg")]
    DKW2Drive,

    /// <summary>
    /// Double slip switch with 1 drive (DKW - Doppelkreuzweiche).
    /// </summary>
    [EnumMember(Value = "dkw_1antrieb")]
    [Description("Double Slip Switch")]
    [FileName("magicon_a_012_01.svg")]
    DKW1Drive,

    /// <summary>
    /// Three-way turnout switch.
    /// </summary>
    [EnumMember(Value = "dreiwegweiche")]
    [Description("Three Way")]
    [FileName("magicon_a_010_01.svg")]
    ThreeWaySwitch,

    /// <summary>
    /// Scissors crossing (Hosenträger).
    /// </summary>
    [EnumMember(Value = "hosentraeger")]
    [Description("Scissors Crossing")]
    [FileName("magicon_a_070_01.svg")]
    ScissorsCrossing,

    /// <summary>
    /// Digital turntable (model 7686/7).
    /// </summary>
    [EnumMember(Value = "digitaldrehscheibe")]
    [Description("Turntable 7686/7")]
    [FileName("magicon_a_118_00.svg")]
    TurntableDigital,

    /// <summary>
    /// Turntable with mfx support (models 74861/66861).
    /// </summary>
    [EnumMember(Value = "drehscheibe_mfx")]
    [Description("Turntable 74861 66861 mfx")]
    [FileName("magicon_a_118_00.svg")]
    TurntableMfx,

    #endregion

    #region Light Signals

    /// <summary>
    /// Light Signal HP0/1 
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP01")]
    [Description("Light Hp0/1")]
    [FileName("magicon_a_015_01.svg")]
    LightSignalHp01,
    
    /// <summary>
    /// Light Signal Hp0/2 
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP02")]
    [Description("Light Hp0/2")]
    [FileName("magicon_a_016_01.svg")]
    LightSignalHp02,

    /// <summary>
    /// Light Signal Hp0/1/2  
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012")]
    [Description("Light Hp0/1/2")]
    [FileName("magicon_a_017_01.svg")]
    LightSignalHp012,   

    /// <summary>
    /// Light Signal Hp0/1/2 + Sh0/1  
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012_SH01")]
    [Description("Light Hp0/1/2 + Sh0/1")]
    [FileName("magicon_a_018_01.svg")]
    LightSignalHp012Sh01,

    /// <summary>
    /// Light Signal Sh0/1  
    /// </summary>
    [EnumMember(Value = "lichtsignal_SH01")]
    [Description("Light Sh0/1")]
    [FileName("magicon_a_019_01.svg")]
    LightSignalSh01,

    /// <summary>
    /// Light Signal Vr0/1/2  
    /// </summary>
    [EnumMember(Value = "lichtvorsignal_VR012")]
    [Description("Light Vr0/1/2")]
    [FileName("magicon_a_029_01.svg")]
    LightSignalVr012,

    // Scale
    /// <summary>
    /// Scale Light Signal HP0/1 
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_HP01")]
    [Description("Light Hp0/1 Scale")]
    [FileName("magicon_a_015_01.svg")]
    ScaleLightSignalHp01,

    /// <summary>
    /// Scale Light Signal Hp0/1/2  
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_HP012")]
    [Description("Light Hp0/1/2 Scale")]
    [FileName("magicon_a_017_01.svg")]
    ScaleLightSignalHp012,
    
    /// <summary>
    /// Light Signal HP0/1/2 + SH0/1  
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_HP012_SH01")]
    [Description("Light Hp0/1/2 + SH0/1 Scale")]
    [FileName("magicon_a_027_01.svg")]
    ScaleLightSignalHp012Sh01,

    /// <summary>
    /// Light Signal SH0/1  
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_SH01")]
    [Description("Light Sh0/1 Scale")]
    [FileName("magicon_a_019_01.svg")]
    ScaleLightSignalSh01,

    /// <summary>
    /// Light Signal VR0/1/2  
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_VR012")]
    [Description("Light Vr0/1/2 Scale")]
    [FileName("magicon_a_029_01.svg")]
    ScaleLightSignalVr012,

    /// <summary>
    /// Monitoring Signal BÜ100/101  
    /// </summary>
    [EnumMember(Value = "ueberwachungs_signal")]
    [Description("Bü 100/101")]
    [FileName("magicon_a_040_01.svg")]
    MonitoringSignal,

    /// <summary>
    /// Monitoring Signal Zp9  
    /// </summary>
    [EnumMember(Value = "abfahrtsignal")]
    [Description("Zp9")]
    [FileName("magicon_a_063_01.svg")]
    DepartureSignal,
    
    #endregion

    #region Semaphore Signals

    /// <summary>
    /// Semaphore Signal Hp0/1 
    /// </summary>
    [EnumMember(Value = "formsignal_HP01")]
    [Description("Hp0/1")]
    [FileName("magicon_a_020_01.svg")]
    SemaphoreSignalHp01,

    /// <summary>
    /// Semaphore Signal Hp0/2 
    /// </summary>
    [EnumMember(Value = "formsignal_HP02")]
    [Description("Hp0/2")]
    [FileName("magicon_a_021_01.svg")]
    SemaphoreSignalHp02,


    /// <summary>
    /// Semaphore Signal Hp0/1/2 
    /// </summary>
    [EnumMember(Value = "formsignal_HP012")]
    [Description("Hp0/1/2")]
    [FileName("magicon_a_022_01.svg")]
    SemaphoreSignalHp012,

    /// <summary>
    /// Semaphore Signal Hp0/1/2 + Sh0/1  
    /// </summary>
    [EnumMember(Value = "formsignal_HP012_SH01")]
    [Description("Hp0/1/2+Sh0/1")]
    [FileName("magicon_a_023_01.svg")]
    SemaphoreSignalHp012Sh01,

    /// <summary>
    /// Semaphore Signal Sh0/1 
    /// </summary>
    [EnumMember(Value = "formsignal_SH01")]
    [Description("Sh0/1")]
    [FileName("magicon_a_024_01.svg")]
    SemaphoreSignalSh01,

    /// <summary>
    /// Profi Semaphore Signal Hp0/1 
    /// </summary>
    [EnumMember(Value = "profi_formsignal_HP01")]
    [Description("Hp0/1 Scale")]
    [FileName("magicon_a_020_01.svg")]
    ProfiSemaphoreSignalHp01,

    /// <summary>
    /// Profi Semaphore Signal Hp0/1/2 
    /// </summary>
    [EnumMember(Value = "profi_formsignal_HP012")]
    [Description("Hp0/1/2 Scale")]
    [FileName("magicon_a_022_01.svg")]
    ProfiSemaphoreSignalHp021,

    /// <summary>
    /// Profi Semaphore Signal Sh0/1 
    /// </summary>
    [EnumMember(Value = "profi_formsignal_SH01")]
    [Description("Sh0/1 Scale")]
    [FileName("magicon_a_024_01.svg")]
    ProfiSemaphoreSignalSh01,

    /// <summary>
    /// Profi Semaphore Signal Hp0/1
    /// </summary>
    [EnumMember(Value = "profi_vorsignal_HP01")]
    [Description("Vr0/1 Scale")]
    [FileName("magicon_a_044_01.svg")]
    ProfiSemaphoreSignalVr01,

    /// <summary>
    /// Profi Semaphore Signal Hp0/1/2
    /// </summary>
    [EnumMember(Value = "profi_vorsignal_HP012")]
    [Description("Vr0/1/2 Scale")]
    [FileName("magicon_a_045_02.svg")]
    ProfiSemaphoreSignalVr012,

    /// <summary>
    /// Wait signal Ra11 (Rangierhalttafel - shunting stop board).
    /// </summary>
    [EnumMember(Value = "wartesignal")]
    [Description("Ra11")]
    [FileName("magicon_a_064_01.svg")]
    ProfiSemaphoreSignalWait,


    //[EnumMember(Value = "urc_formsignal_HP01")]
    //[FileName("magicon_a_020_01.svg")]
    //UrcSemaphoreSignalHp01,

    //[EnumMember(Value = "urc_formsignal_HP012")]
    //[FileName("magicon_a_022_01.svg")]
    //UrcSemaphoreSignalHp012,

    //[EnumMember(Value = "urc_formsignal_HP012_SH01")]
    //[FileName("magicon_a_023_01.svg")]
    //UrcSemaphoreSignalHp012Sh01,

    //[EnumMember(Value = "urc_formsignal_SH01")]
    //[FileName("magicon_a_024_01.svg")]
    //UrcSemaphoreSignalSh01,

    ///// <summary>
    ///// Semaphore Signal VR0/1  
    ///// </summary>
    //[EnumMember(Value = "urc_formsignal_VR01")]
    //[FileName("magicon_a_044_01.svg")]
    //UrcSemaphoreSignalVr01,

    ///// <summary>
    ///// Semaphore Signal VR0/1/2 
    ///// </summary>
    //[EnumMember(Value = "urc_formsignal_VR012")]
    //[FileName("magicon_a_045_01.svg")]
    //UrcSemaphoreSignalVr012,

    #endregion

    #region Misc

    /// <summary>
    /// Standard red/green article (generic accessory).
    /// </summary>
    [EnumMember(Value = "std_rot_gruen")]
    [Description("Standard")]
    [FileName("magicon_a_000_00.svg")]
    StdRedGreen,

    /// <summary>
    /// Standard red article.
    /// </summary>
    [EnumMember(Value = "std_rot")]
    [Description("Standard Red")]
    [FileName("magicon_a_001_01.svg")]
    StdRed,

    //[EnumMember(Value = "std_gruen")]
    //[FileName("magicon_a_002_01.svg")]
    //StdGreen,

    /// <summary>
    /// Uncoupling track (Entkupplungsgleis) for automatic car uncoupling.
    /// </summary>
    [EnumMember(Value = "entkupplungsgleis")]
    [Description("Coupling")]
    [FileName("magicon_a_003_01.svg")]
    Uncoupler,

    /// <summary>
    /// Uncoupling track for G1 scale (gauge 1).
    /// </summary>
    [EnumMember(Value = "entkupplungsgleis_1")]
    [Description("Coupling G1")]
    [FileName("magicon_a_004_01.svg")]
    Uncoupler1,

    /// <summary>
    /// K84 decoder output.
    /// </summary>
    [EnumMember(Value = "k84_ausgang")]
    [Description("K84")]
    [FileName("magicon_a_008_01.svg")]
    K84Exit,

    /// <summary>
    /// K84 double output (two outputs on one decoder).
    /// </summary>
    [EnumMember(Value = "k84_doppelausgang")]
    [Description("Double K84")]
    [FileName("magicon_a_009_01.svg")]
    K84DoubleExit,

    /// <summary>
    /// Transfer table (Schiebebühne) for moving rolling stock between parallel tracks.
    /// </summary>
    [EnumMember(Value = "schiebebuehne")]
    [Description("Transfer")]
    [FileName("magicon_a_030_01.svg")]
    K84Slideway,

    /// <summary>
    /// K84-controlled turntable (older model).
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt")]
    [Description("Turntable K84")]
    [FileName("magicon_a_031_00.svg")]
    K84Turntable,

    /// <summary>
    /// Level crossing signal (Andreaskreuz).
    /// </summary>
    [EnumMember(Value = "andreas")]
    [Description("Crossing Signal")]
    [FileName("magicon_a_038_01.svg")]
    CrossingSignal,

    /// <summary>
    /// Level crossing barrier (Schranke).
    /// </summary>
    [EnumMember(Value = "schranke")]
    [Description("Barrier")]
    [FileName("magicon_a_039_01.svg")]
    Barrier,

    /// <summary>
    /// Sound module for operating sounds (Betrieb).
    /// </summary>
    [EnumMember(Value = "sound_betrieb")]
    [Description("Sound Operating")]
    [FileName("magicon_a_048_01.svg")]
    SoundOperation,

    /// <summary>
    /// Sound module for warning sounds (Warnung).
    /// </summary>
    [EnumMember(Value = "sound_warn")]
    [Description("Sound Warn")]
    [FileName("magicon_a_049_01.svg")]
    SoundWarning,

    /// <summary>
    /// Sound module for miscellaneous sounds (Sonstige).
    /// </summary>
    [EnumMember(Value = "sound_misc")]
    [Description("Sound Misc")]
    [FileName("magicon_a_050_01.svg")]
    SoundMisc,

    #endregion
}
