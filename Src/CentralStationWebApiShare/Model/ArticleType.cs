namespace CentralStationWebApi.Model;

[EnumConverter]
public enum ArticleType
{
    Unknown = 0,

    [EnumMember(Value = "std_rot_gruen")]
    StdRedGreen,

    [EnumMember(Value = "std_rot")]
    [FileNameId(001)]
    StdRed,

    [EnumMember(Value = "std_gruen")]
    [FileNameId(002)]
    StdGreen,

    [EnumMember(Value = "entkupplungsgleis")]
    [FileNameId(003)]
    Uncoupler,

    [EnumMember(Value = "entkupplungsgleis_1")]
    [FileNameId(004)]
    Uncoupler1,

    [EnumMember(Value = "rechtsweiche")]
    [FileNameId(005)]
    RightTurnout,

    [EnumMember(Value = "linksweiche")]
    [FileNameId(006)]
    LeftTurnout,

    [EnumMember(Value = "y_weiche")]
    [FileNameId(007)]
    YSwitch,

    [EnumMember(Value = "k84_ausgang")]
    [FileNameId(008)]
    K84Exit,

    [EnumMember(Value = "k84_doppelausgang")]
    [FileNameId(009)]
    K84DoubleExit,

    [EnumMember(Value = "dreiwegweiche")]
    [FileNameId(010)]
    ThreeWaySwitch,

    [EnumMember(Value = "hosentraeger")]
    DoubleCrossing,    

    [EnumMember(Value = "DKW 2 Antriebe")]
    [FileNameId(011)]
    DKW2Drive,

    [EnumMember(Value = "DKW 1 Antrieb")]
    [FileNameId(012)]
    DKW1Drive,

    ///////////////////////////////////////////////////////////////////////////
    // Light Signal

    /// <summary>
    /// Light Signal HP0/1  Code: 015
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP01")]
    [FileNameId(015)]
    LightSignalHp01,

    [EnumMember(Value = "urc_lichtsignal_HP01")]
    [FileNameId(015)]
    UrcLightSignalHp01,
    
    /// <summary>
    /// Light Signal HP0/2  Code: 016
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP02")]
    [FileNameId(016)]
    LightSignalHp02,

    /// <summary>
    /// Light Signal HP0/1/2  Code: 017
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012")]
    [FileNameId(017)]
    LightSignalHp012,

    [EnumMember(Value = "urc_lichtsignal_HP012")]
    [FileNameId(017)]
    UrcLightSignalHp012,

    /// <summary>
    /// Light Signal HP0/1/2 + SH0/1  Code: 018
    /// </summary>
    [EnumMember(Value = "lichtsignal_HP012_SH01")]
    [FileNameId(018)]
    LightSignalHp012Sh01,

    /// <summary>
    /// Light Signal SH0/1  Code: 019
    /// </summary>
    [EnumMember(Value = "lichtsignal_SH01")]
    [FileNameId(019)]
    LightSignalSh01,

    [EnumMember(Value = "urc_lichtsignal_SH01")]
    [FileNameId(019)]
    UrcLightSignalSh01,

    ///////////////////////////////////////////////////////////////////////////
    // Semaphore Signal

    /// <summary>
    /// Semaphore Signal HP0/1  Code: 020
    /// </summary>
    [EnumMember(Value = "formsignal_HP01")]
    [FileNameId(020)]
    SemaphoreSignalHp01,

    [EnumMember(Value = "urc_formsignal_HP01")]
    [FileNameId(020)]
    UrcSemaphoreSignalHp01,

    /// <summary>
    /// Semaphore Signal HP0/2  Code: 021
    /// </summary>
    [EnumMember(Value = "formsignal_HP02")]
    [FileNameId(021)]
    SemaphoreSignalHp02,

    /// <summary>
    /// Semaphore Signal HP0/1/2  Code: 022
    /// </summary>
    [EnumMember(Value = "formsignal_HP012")]
    [FileNameId(022)]
    SemaphoreSignalHp012,

    [EnumMember(Value = "urc_formsignal_HP012")]
    [FileNameId(022)]
    UrcSemaphoreSignalHp012,

    /// <summary>
    /// Semaphore Signal HP0/1/2 + SH0/1  Code: 023
    /// </summary>
    [EnumMember(Value = "formsignal_HP012_SH01")]
    [FileNameId(023)]
    SemaphoreSignalHp012Sh01,

    [EnumMember(Value = "urc_formsignal_HP012_SH01")]
    [FileNameId(023)]
    UrcSemaphoreSignalHp012Sh01,

    /// <summary>
    /// Semaphore Signal HSH0/1  Code: 024
    /// </summary>
    [EnumMember(Value = "formsignal_SH01")]
    [FileNameId(024)]
    SemaphoreSignalSh01,

    [EnumMember(Value = "urc_formsignal_SH01")]
    [FileNameId(024)]
    UrcSemaphoreSignalSh01,


    ///////////////////////////////////////////////////////////////////////////
    // Light Signal additional

    /// <summary>
    /// Light Signal HP0/1/2 + SH0/1  Code: 027
    /// </summary>
    [EnumMember(Value = "urc_lichtsignal_HP012_SH01")]
    [FileNameId(027)]
    UrcLightSignalHp012Sh01,

    /// <summary>
    /// Light Signal VR0/1/2  Code: 029
    /// </summary>
    [EnumMember(Value = "lichtsignal_VR012")]
    [FileNameId(029)]
    LightSignalVr012,




    [EnumMember(Value = "schiebebuehne")]
    Slideway,

    /// <summary>
    /// data:image/png;base64,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
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt")]
    TurntableOld,

    [EnumMember(Value = "digitaldrehscheibe")]
    TurntableDigital,



    [EnumMember(Value = "drehscheibe_mfx")]
    TurntableMfx,

    ///////////////////////////////////////////////////////////////////////////
    // Light 

    /// <summary>
    /// Light park lamp / Park Code: 033
    /// </summary>
    [EnumMember(Value = "licht_str_lampe")]
    [FileNameId(033)]
    LightStreetLamp,

    /// <summary>
    /// Light street neon / Bahn Code: 034
    /// </summary>
    [EnumMember(Value = "licht_str_neon")]
    [FileNameId(034)]
    LightStreetNeon,

    /// <summary>
    /// Light street / Straße Bahn Code: 034
    /// </summary>
    [EnumMember(Value = "licht_str ?????")]
    [FileNameId(035)]
    LightStreet,

    /// <summary>  
    /// Light lamp / Haus Code: 036
    /// </summary>
    [EnumMember(Value = "licht_lampe")]
    [FileNameId(036)]
    LightLamp,

    /// <summary>
    /// Light neon / Spar Code: 037
    /// </summary>
    [EnumMember(Value = "licht_neon")]
    [FileNameId(037)]
    LightNeon,

    // <summary>
    /// Light outdoor  Code: 047
    /// </summary>
    [EnumMember(Value = "licht_aussen?????")]
    [FileNameId(047)]
    LightOutdoor2,

    ///////////////////////////////////////////////////////////////////////////
    // Sound 

    /// <summary>
    /// Sound operation / Betrieb  Code: 048
    /// </summary>
    [EnumMember(Value = "sound_betrieb")]
    [FileNameId(048)]
    SoundOperation,

    /// <summary>
    /// Sound warning / Warn  Code: 049
    /// </summary>
    [EnumMember(Value = "sound_warn")]
    [FileNameId(049)]
    SoundWarning,

    /// <summary>
    /// Sound miscellaneous / Sonstige  Code: 050
    /// </summary>
    [EnumMember(Value = "sound_misc")]
    [FileNameId(050)]
    SoundMisc,

    ///////////////////////////////////////////////////////////////////////////
    // 

    /// <summary>
    /// Light parc classic / Park klassisch  Code: 056
    /// </summary>
    [EnumMember(Value = "licht_?????1")]
    [FileNameId(056)]
    LightParkClassic,
        

    /// <summary>
    /// Light red / Rot  Code: 057 & 059
    /// </summary>
    [EnumMember(Value = "licht_?????2")]
    [FileNameId(057)]
    LightRed,

    /// <summary>
    /// Light yellow / Gelb  Code: 058
    /// </summary>
    [EnumMember(Value = "licht_?????3")]
    [FileNameId(058)]
    LightYellow,

    /// <summary>
    /// Light green / Grün  Code: 060
    /// </summary>
    [EnumMember(Value = "licht_?????4")]
    [FileNameId(060)]
    LightGreen,

    /// <summary>
    /// Light blue / Blau  Code: 061
    /// </summary>
    [EnumMember(Value = "licht_?????5")]
    [FileNameId(061)]
    LightBlue,


    /// <summary>
    /// Light outdoor  Code: 062
    /// </summary>
    [EnumMember(Value = "licht_aussen")]
    [FileNameId(062)]
    LightOutdoor,

    

   

   


   

    

    /// <summary>
    /// Monitoring Signal BÜ100/101  Code: 040
    /// </summary>
    [EnumMember(Value = "ueberwachungs_signal")]
    [FileNameId(040)]
    MonitoringSignal,


    /// <summary>
    /// Semaphore Signal VR0/1  Code: 044
    /// </summary>
    [EnumMember(Value = "urc_formsignal_VR01")]
    [FileNameId(044)]
    UrcSemaphoreSignalVr01,

    /// <summary>
    /// Semaphore Signal VR0/1/2  Code: 045
    /// </summary>
    [EnumMember(Value = "urc_formsignal_VR012")]
    [FileNameId(045)]
    UrcSemaphoreSignalVr012,

    /// <summary>
    /// Monitoring Signal Zp9  Code: 063
    /// </summary>
    [EnumMember(Value = "abfahrtsignal")]

    [FileNameId(063)]
    DepartureSignal,

    /// <summary>
    /// Wait  Code: 064
    /// </summary>
    [EnumMember(Value = "warten oder sowas ????")]
    [FileNameId(064)]
    Wait,



}
