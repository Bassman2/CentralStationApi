namespace CentralStationWebApi.Model;

[EnumConverter]
public enum ArticleType
{
    Unknown = 0,

    [EnumMember(Value = "std_rot_gruen")]
    StdRedGreen,

    [EnumMember(Value = "std_rot")]
    StdRed,

    [EnumMember(Value = "std_gruen")]
    StdGreen,

    [EnumMember(Value = "entkupplungsgleis")]
    Uncoupler,

    [EnumMember(Value = "entkupplungsgleis_1")]
    Uncoupler1,

    [EnumMember(Value = "rechtsweiche")]
    RightTurnout,

    [EnumMember(Value = "linksweiche")]
    LeftTurnout,

    [EnumMember(Value = "y_weiche")]
    YSwitch,

    [EnumMember(Value = "k84_ausgang")]
    K84Exit,

    [EnumMember(Value = "k84_doppelausgang")]
    K84DoubleExit,

    [EnumMember(Value = "dreiwegweiche")]
    ThreeWaySwitch,

    [EnumMember(Value = "DKW 2 Antriebe")]
    DKW2Drive,

    [EnumMember(Value = "DKW 1 Antrieb")]
    DKW1Drive,

    [EnumMember(Value = "lichtsignal_HP01")]
    LightSignalHp01,

    [EnumMember(Value = "lichtsignal_HP02")]
    LightSignalHp02,

    [EnumMember(Value = "lichtsignal_HP012")]
    LightSignalHp012,

    [EnumMember(Value = "lichtsignal_HP012_SH01")]
    LightSignalHp012Sh01,

    [EnumMember(Value = "lichtsignal_SH01")]
    LightSignalSh01,

    [EnumMember(Value = "formsignal_HP01")]
    SemaphoreSignalHp01,

    [EnumMember(Value = "formsignal_HP02")]
    SemaphoreSignalHp02,

    [EnumMember(Value = "formsignal_HP012")]
    SemaphoreSignalHp012,

    [EnumMember(Value = "formsignal_HP012_SH01")]
    SemaphoreSignalHp012Sh01,

    [EnumMember(Value = "formsignal_SH01")]
    SemaphoreSignalHp01Sh01,

    [EnumMember(Value = "urc_lichtsignal_HP01")]
    UrcLightSignalHp01,

    [EnumMember(Value = "urc_lichtsignal_HP012")]
    UrcLightSignalHp012,

    [EnumMember(Value = "urc_lichtsignal_HP012_SH01")]
    UrcLightSignalHp012Sh01,

    [EnumMember(Value = "urc_lichtsignal_SH01")]
    UrcLightSignalSh01,

    [EnumMember(Value = "schiebebuehne")]
    Slideway,

    [EnumMember(Value = "drehscheibe_alt")]
    TurntableOld,

    [EnumMember(Value = "digitaldrehscheibe")]
    TurntableDigital
}
