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

    [EnumMember(Value = "drehscheibe_alt")]
    TurntableOld,

    [EnumMember(Value = "digitaldrehscheibe")]
    TurntableDigital,



    [EnumMember(Value = "drehscheibe_mfx")]
    TurntableMfx,

    [EnumMember(Value = "licht_str_lampe")]
    LightStreetLamp,

    [EnumMember(Value = "licht_aussen")]
    LightOutdoor,

    [EnumMember(Value = "licht_str_neon")]
    LightStreetNeon,

    [EnumMember(Value = "licht_lampe")]
    LightLamp,


    [EnumMember(Value = "licht_neon")]
    LightNeon,

    [EnumMember(Value = "sound_misc")]
    SoundMisc,

    [EnumMember(Value = "sound_betrieb")]
    SoundOperation,

    [EnumMember(Value = "sound_warn")]
    SoundWarning,

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
