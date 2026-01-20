namespace CentralStationWebApi.Model;

[EnumConverter]
public enum ArticleType
{
    Unknown = 0,

    ///////////////////////////////////////////////////////////////////////////
    // Common

   

   
    [EnumMember(Value = "k84_ausgang")]
    [FileName("magicon_a_008_01.svg")]
    K84Exit,

    [EnumMember(Value = "k84_doppelausgang")]
    [FileName("magicon_a_009_01.svg")]
    K84DoubleExit,

   

    [EnumMember(Value = "schiebebuehne")]
    Slideway,

    /// <summary>
    /// data:image/png;base64,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
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt")]
    [FileName("magicon_a_118_00.svg")]
    TurntableOld,

    [EnumMember(Value = "digitaldrehscheibe")]
    [FileName("magicon_a_118_00.svg")]
    TurntableDigital,

    [EnumMember(Value = "drehscheibe_mfx")]
    [FileName("magicon_a_118_00.svg")]
    TurntableMfx,

    ///////////////////////////////////////////////////////////////////////////
    // Light

    /// <summary>
    /// Light parc classic / Park klassisch  
    /// </summary>
    [EnumMember(Value = "licht_?????1")]
    [Description("Vintage Park")]
    [FileName("magicon_a_056_01.svg")]
    LightParkClassic,

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
    ProfiLight4,

    /// <summary>
    /// Light yellow / Gelb 
    /// </summary>
    [EnumMember(Value = "signallampe_ge")]
    [Description("Yellow")]
    [FileName("magicon_a_058_01.svg")]
    LightYellow,

    /// <summary>
    /// Light red / Rot  
    /// </summary>
    [EnumMember(Value = "signallampe_rt")]
    [Description("Red")]
    [FileName("magicon_a_059_01.svg")]
    LightRed,

    /// <summary>
    /// Light green / Grün 
    /// </summary>
    [EnumMember(Value = "signallampe_gn")]
    [Description("Green")]
    [FileName("magicon_a_060_01.svg")]
    LightGreen,

    /// <summary>
    /// Light blue / Blau 
    /// </summary>
    [EnumMember(Value = "signallampe_bl")]
    [Description("Blue")]
    [FileName("magicon_a_061_01.svg")]
    LightBlue,


    /// <summary>
    /// Light outdoor  
    /// </summary>
    [EnumMember(Value = "licht_aussen")]
    [FileName("magicon_a_062_01.svg")]
    LightOutdoor,

    ///////////////////////////////////////////////////////////////////////////
    // Turnouts

    [EnumMember(Value = "rechtsweiche")]
    [Description("Right")]
    [FileName("magicon_a_005_01.svg")]
    RightTurnout,

    [EnumMember(Value = "linksweiche")]
    [Description("Left")]
    [FileName("magicon_a_006_01.svg")]
    LeftTurnout,

    [EnumMember(Value = "y_weiche")]
    [FileName("magicon_a_007_01.svg")]
    YSwitch,

    [EnumMember(Value = "dreiwegweiche")]
    [FileName("magicon_a_010_01.svg")]
    ThreeWaySwitch,

    [EnumMember(Value = "hosentraeger")]
    [FileName("magicon_a_070_01.svg")]
    ScissorsCrossing,

    [EnumMember(Value = "DKW 2 Antriebe")]
    [FileName("magicon_a_011_01.svg")]
    DKW2Drive,

    [EnumMember(Value = "DKW 1 Antrieb")]
    [FileName("magicon_a_012_01.svg")]
    DKW1Drive,



    ///////////////////////////////////////////////////////////////////////////
    // Light Signal

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

    ///////////////////////////////////////////////////////////////////////////
    // Semaphore Signal

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
    /// Monitoring Signal Zp9  
    /// </summary>
    [EnumMember(Value = "abfahrtsignal")]

    [FileName("magicon_a_063_01.svg")]
    DepartureSignal,

    /// <summary>
    /// Wait  
    /// </summary>
    [EnumMember(Value = "wartesignal")]
    [Description("Ra11")]
    [FileName("magicon_a_064_01.svg")]
    Wait,

    ///////////////////////////////////////////////////////////////////////////
    // Light 

    /// <summary>
    /// Light park lamp / Park 
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

    /// <summary>
    /// Light street / Straße Bahn
    /// </summary>
    [EnumMember(Value = "licht_str ?????")]
    [Description("Street")]
    [FileName("magicon_a_035_01.svg")]
    LightStreet,

    /// <summary>  
    /// Light lamp / Haus 
    /// </summary>
    [EnumMember(Value = "licht_lampe")]
    [Description("House")]
    [FileName("magicon_a_036_01.svg")]
    LightLamp,

    /// <summary>
    /// Light neon / Spar
    /// </summary>
    [EnumMember(Value = "licht_neon")]
    [Description("Save")]
    [FileName("magicon_a_037_01.svg")]
    LightNeon,
    
    // <summary>
    /// Light outdoor  
    /// </summary>
    [EnumMember(Value = "licht_aussen2")]
    [Description("Outside")]
    [FileName("magicon_a_047_01.svg")]
    LightOutdoor2,

    ///////////////////////////////////////////////////////////////////////////
    // Sound 

    /// <summary>
    /// Sound operation / Betrieb 
    /// </summary>
    [EnumMember(Value = "sound_betrieb")]
    [FileName("magicon_a_048_01.svg")]
    SoundOperation,

    /// <summary>
    /// Sound warning / Warn 
    /// </summary>
    [EnumMember(Value = "sound_warn")]
    [FileName("magicon_a_049_01.svg")]
    SoundWarning,

    /// <summary>
    /// Sound miscellaneous / Sonstige  
    /// </summary>
    [EnumMember(Value = "sound_misc")]
    [FileName("magicon_a_050_01.svg")]
    SoundMisc,






    ///////////////////////////////////////////////////////////////////////////
    // Misc


    [EnumMember(Value = "std_rot_gruen")]
    [Description("")]
    [FileName("magicon_a_000_00.svg")]
    StdRedGreen,

    [EnumMember(Value = "std_rot")]
    [FileName("magicon_a_001_01.svg")]
    StdRed,

    [EnumMember(Value = "std_gruen")]
    [FileName("magicon_a_002_01.svg")]
    StdGreen,

    [EnumMember(Value = "entkupplungsgleis")]
    [FileName("magicon_a_003_01.svg")]
    Uncoupler,

    [EnumMember(Value = "entkupplungsgleis_1")]
    [FileName("magicon_a_004_01.svg")]
    Uncoupler1,

    /// <summary>  
    /// Crossing Signal
    /// </summary>
    [EnumMember(Value = "andreas")]
    [Description("Crossing Signal")]
    [FileName("magicon_a_038_01.svg")]
    CrossingSignal,

    /// <summary>
    /// Monitoring Signal BÜ100/101  
    /// </summary>
    [EnumMember(Value = "ueberwachungs_signal")]
    [FileName("magicon_a_040_01.svg")]
    MonitoringSignal,


    



}
