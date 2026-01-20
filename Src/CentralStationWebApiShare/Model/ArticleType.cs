namespace CentralStationWebApi.Model;

[EnumConverter]
public enum ArticleType
{
    Unknown = 0,

    #region Light

    [EnumMember(Value = "licht_str_lampe")]
    [Description("Park")]
    [FileName("magicon_a_033_01.svg")]
    LightStreetLamp,

    /// <summary>
    /// Light street neon / Bahn 
    /// </summary>
    [EnumMember(Value = "licht_str_neon")]
    [Description("Track")]
    [FileName("magicon_a_034_01.svg")]
    LightStreetNeon,
       
    [EnumMember(Value = "licht_mast")]
    [Description("Street")]
    [FileName("magicon_a_035_01.svg")]
    LightStreet,
       
    [EnumMember(Value = "licht_lampe")]
    [Description("House")]
    [FileName("magicon_a_036_01.svg")]
    LightLamp,
       
    [EnumMember(Value = "licht_neon")]
    [Description("Save")]
    [FileName("magicon_a_037_01.svg")]
    LightNeon,
        
    [EnumMember(Value = "licht_park")]
    [Description("Vintage Park")]
    [FileName("magicon_a_056_01.svg")]
    LightParkClassic,

    [EnumMember(Value = "licht_aussen")]
    [Description("Outside")]
    [FileName("magicon_a_062_01.svg")]
    LightOutside,

    [EnumMember(Value = "signallampe_ge")]
    [Description("Yellow")]
    [FileName("magicon_a_058_01.svg")]
    LightYellow,

    [EnumMember(Value = "signallampe_rt")]
    [Description("Red")]
    [FileName("magicon_a_059_01.svg")]
    LightRed,

    [EnumMember(Value = "signallampe_gn")]
    [Description("Green")]
    [FileName("magicon_a_060_01.svg")]
    LightGreen,

    [EnumMember(Value = "signallampe_bl")]
    [Description("Blue")]
    [FileName("magicon_a_061_01.svg")]
    LightBlue,

    /// <summary>
    /// Light color  
    /// <list type="bullet">
    /// <item>0: gray</item>
    /// <item>1: red</item>
    /// <item>2: green</item>
    /// <item>3: yellow</item>
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

    [EnumMember(Value = "rechtsweiche")]
    [Description("Right Turnout")]
    [FileName("magicon_a_005_01.svg")]
    RightTurnout,

    [EnumMember(Value = "linksweiche")]
    [Description("Left Turnout")]
    [FileName("magicon_a_006_01.svg")]
    LeftTurnout,

    [EnumMember(Value = "y_weiche")]
    [Description("Y Turnout")]
    [FileName("magicon_a_007_01.svg")]
    YSwitch,
    
    [EnumMember(Value = "dkw_2antriebe")]
    [Description("Double Slip Switch 2")]
    [FileName("magicon_a_011_01.svg")]
    DKW2Drive,

    [EnumMember(Value = "dkw_1antrieb")]
    [Description("Double Slip Switch")]
    [FileName("magicon_a_012_01.svg")]
    DKW1Drive,

    [EnumMember(Value = "dreiwegweiche")]
    [Description("Three Way")]
    [FileName("magicon_a_010_01.svg")]
    ThreeWaySwitch,

    [EnumMember(Value = "hosentraeger")]
    [Description("Scissors Crossing")]
    [FileName("magicon_a_070_01.svg")]
    ScissorsCrossing,

    [EnumMember(Value = "digitaldrehscheibe")]
    [Description("Turntable 7686/7")]
    [FileName("magicon_a_118_00.svg")]
    TurntableDigital,

    [EnumMember(Value = "drehscheibe_mfx")]
    [Description("Turntable 74861 66861 mfx")]
    [FileName("magicon_a_118_00.svg")]
    TurntableMfx,

    #endregion

    #region Light Signals

    /// <summary>
    /// Light Signal HP0/1  Code: 015
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP01")]
    [FileName("magicon_a_015_01.svg")]
    LightSignalHp01,

    [EnumMember(Value = "urc_lichtsignal_HP01")]
    [FileName("magicon_a_015_01.svg")]
    UrcLightSignalHp01,

    /// <summary>
    /// Light Signal HP0/2  Code: 016
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP02")]
    [FileName("magicon_a_016_01.svg")]
    LightSignalHp02,

    /// <summary>
    /// Light Signal HP0/1/2  Code: 017
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012")]
    [FileName("magicon_a_017_01.svg")]
    LightSignalHp012,

    [EnumMember(Value = "urc_lichtsignal_HP012")]
    [FileName("magicon_a_017_01.svg")]
    UrcLightSignalHp012,

    /// <summary>
    /// Light Signal HP0/1/2 + SH0/1  Code: 018
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012_SH01")]
    [FileName("magicon_a_018_01.svg")]
    LightSignalHp012Sh01,

    /// <summary>
    /// Light Signal SH0/1  Code: 019
    /// </summary>
    [EnumMember(Value = "lichtsignal_SH01")]
    [FileName("magicon_a_019_01.svg")]
    LightSignalSh01,

    [EnumMember(Value = "urc_lichtsignal_SH01")]
    [FileName("magicon_a_019_01.svg")]
    UrcLightSignalSh01,

    /// <summary>
    /// Light Signal HP0/1/2 + SH0/1  Code: 027
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_HP012_SH01")]
    [FileName("magicon_a_027_01.svg")]
    UrcLightSignalHp012Sh01,

    /// <summary>
    /// Light Signal VR0/1/2  Code: 029
    /// </summary>
    [EnumMember(Value = "lichtsignal_VR012")]
    [FileName("magicon_a_029_01.svg")]
    LightSignalVr012,

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
    /// Semaphore Signal HP0/1  Code: 020
    /// </summary>
    [EnumMember(Value = "formsignal_HP01")]
    [FileName("magicon_a_020_01.svg")]
    SemaphoreSignalHp01,

    [EnumMember(Value = "urc_formsignal_HP01")]
    [FileName("magicon_a_020_01.svg")]
    UrcSemaphoreSignalHp01,

    [EnumMember(Value = "profi_formsignal_HP01")]
    [FileName("magicon_a_020_01.svg")]
    ProfiSemaphoreSignalHp01,

    /// <summary>
    /// Semaphore Signal HP0/2  Code: 021
    /// </summary>
    [EnumMember(Value = "formsignal_HP02")]
    [FileName("magicon_a_021_01.svg")]
    SemaphoreSignalHp02,

    /// <summary>
    /// Semaphore Signal HP0/1/2  Code: 022
    /// </summary>
    [EnumMember(Value = "formsignal_HP012")]
    [FileName("magicon_a_022_01.svg")]
    SemaphoreSignalHp012,

    [EnumMember(Value = "urc_formsignal_HP012")]
    [FileName("magicon_a_022_01.svg")]
    UrcSemaphoreSignalHp012,

    [EnumMember(Value = "profi_formsignal_HP012")]
    [FileName("magicon_a_022_01.svg")]
    ProfiSemaphoreSignalHp021,

    /// <summary>
    /// Semaphore Signal HP0/1/2 + SH0/1  Code: 023
    /// </summary>
    [EnumMember(Value = "formsignal_HP012_SH01")]
    [FileName("magicon_a_023_01.svg")]
    SemaphoreSignalHp012Sh01,

    [EnumMember(Value = "urc_formsignal_HP012_SH01")]
    [FileName("magicon_a_023_01.svg")]
    UrcSemaphoreSignalHp012Sh01,

    /// <summary>
    /// Semaphore Signal HSH0/1  Code: 024
    /// </summary>
    [EnumMember(Value = "formsignal_SH01")]
    [FileName("magicon_a_024_01.svg")]
    SemaphoreSignalSh01,

    [EnumMember(Value = "urc_formsignal_SH01")]
    [FileName("magicon_a_024_01.svg")]
    UrcSemaphoreSignalSh01,

    [EnumMember(Value = "profi_formsignal_SH01")]
    [FileName("magicon_a_024_01.svg")]
    ProfiSemaphoreSignalSh01,

    // <summary>
    /// Profi vorsignal HP01
    /// </summary>
    [EnumMember(Value = "profi_vorsignal_HP01")]
    [Description("Prof vorsignal HP01")]
    [FileName("magicon_a_044_01.svg")]
    ProfiVorsignalHP01,

    // <summary>
    /// Profi vorsignal HP012
    /// </summary>
    [EnumMember(Value = "profi_vorsignal_HP012")]
    [Description("Prof vorsignal HP012")]
    [FileName("magicon_a_045_02.svg")]
    ProfiVorsignalHP012,

    /// <summary>
    /// Semaphore Signal VR0/1  
    /// </summary>
    [EnumMember(Value = "urc_formsignal_VR01")]
    [FileName("magicon_a_044_01.svg")]
    UrcSemaphoreSignalVr01,

    /// <summary>
    /// Semaphore Signal VR0/1/2 
    /// </summary>
    [EnumMember(Value = "urc_formsignal_VR012")]
    [FileName("magicon_a_045_01.svg")]
    UrcSemaphoreSignalVr012,

    /// <summary>
    /// Wait  
    /// </summary>
    [EnumMember(Value = "wartesignal")]
    [Description("Ra11")]
    [FileName("magicon_a_064_01.svg")]
    Wait,

    #endregion

    #region Misc

    [EnumMember(Value = "std_rot_gruen")]
    [Description("Standard")]
    [FileName("magicon_a_000_00.svg")]
    StdRedGreen,


    [EnumMember(Value = "std_rot")]
    [Description("Standard Red")]
    [FileName("magicon_a_001_01.svg")]
    StdRed,

    //[EnumMember(Value = "std_gruen")]
    //[FileName("magicon_a_002_01.svg")]
    //StdGreen,

    [EnumMember(Value = "entkupplungsgleis")]
    [Description("Coupling")]
    [FileName("magicon_a_003_01.svg")]
    Uncoupler,

    [EnumMember(Value = "entkupplungsgleis_1")]
    [Description("Coupling G1")]
    [FileName("magicon_a_004_01.svg")]
    Uncoupler1,

    [EnumMember(Value = "k84_ausgang")]
    [Description("K84")]
    [FileName("magicon_a_008_01.svg")]
    K84Exit,

    [EnumMember(Value = "k84_doppelausgang")]
    [Description("Double K84")]
    [FileName("magicon_a_009_01.svg")]
    K84DoubleExit,

    [EnumMember(Value = "schiebebuehne")]
    [Description("Transfer")]
    [FileName("magicon_a_030_01.svg")]
    K84Slideway,

    [EnumMember(Value = "drehscheibe_alt")]
    [Description("Turntable K84")]
    [FileName("magicon_a_031_00.svg")]
    K84Turntable,

    [EnumMember(Value = "andreas")]
    [Description("Crossing Signal")]
    [FileName("magicon_a_038_01.svg")]
    CrossingSignal,

    [EnumMember(Value = "schranke")]
    [Description("Barrier")]
    [FileName("magicon_a_039_01.svg")]
    Barrier,

    /// <summary>
    /// Sound operation / Betrieb 
    /// </summary>
    [EnumMember(Value = "sound_betrieb")]
    [Description("Sound Operating")]
    [FileName("magicon_a_048_01.svg")]
    SoundOperation,

    /// <summary>
    /// Sound warning / Warn 
    /// </summary>
    [EnumMember(Value = "sound_warn")]
    [Description("Sound Warn")]
    [FileName("magicon_a_049_01.svg")]
    SoundWarning,

    /// <summary>
    /// Sound miscellaneous / Sonstige  
    /// </summary>
    [EnumMember(Value = "sound_misc")]
    [Description("Sound Misc")]
    [FileName("magicon_a_050_01.svg")]
    SoundMisc,

    #endregion
}
