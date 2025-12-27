using System.Runtime.Serialization;

namespace CentralStationWebApi.Model;

[EnumConverter]
public enum ElementType
{
    [EnumMember(Value = "leer")]
    Empty = 0,

    [EnumMember(Value = "gerade")]
    Straight,

    [EnumMember(Value = "kreuzung")]
    Crossing,

    [EnumMember(Value = "unterfuehrung")]
    Underpass,

    [EnumMember(Value = "prellbock")]
    BufferStop,

    [EnumMember(Value = "bogen")]
    Curve,

    [EnumMember(Value = "doppelbogen")]
    DoubleCurve,

    [EnumMember(Value = "tunnel")]
    Tunnel,

    [EnumMember(Value = "linksweiche")]
    LeftTurnout,

    [EnumMember(Value = "rechtsweiche")]
    RightTurnout,

    [EnumMember(Value = "dreiwegweiche")]
    ThreeWaySwitch,

    [EnumMember(Value = "yweiche")]
    YSwitch,

    [EnumMember(Value = "dkweiche")]
    DoubleSlipSwitch,

    [EnumMember(Value = "dkweiche_2")]
    DoubleSlipSwitch2,

    [EnumMember(Value = "signal")]
    Signal,

    [EnumMember(Value = "s88kontakt")]
    S88Contact,

    [EnumMember(Value = "s88bogen")]
    S88Curve,

    [EnumMember(Value = "pfeil")]
    Reference,

    [EnumMember(Value = "fahrstrasse")]
    RailwayLine,

    [EnumMember(Value = "text")]
    Text, 

    [EnumMember(Value = "signal_hp02")]
    SignalHp02,

    [EnumMember(Value = "signal_hp012")]
    SignalHp12,

    [EnumMember(Value = "signal_hp01s")]
    SignalHp01s,

    [EnumMember(Value = "signal_p_hp012s")]
    SignalPHp012s,

    [EnumMember(Value = "signal_f_hp012s")]
    SignalFHp012s,

    [EnumMember(Value = "signal_p_hp012")]
    SignalPHp012,

    [EnumMember(Value = "signal_f_hp01")]
    SignalFHp01,

    [EnumMember(Value = "signal_f_hp02")]
    SignalFHp02,

    [EnumMember(Value = "signal_f_hp012")]
    SignalFHp012,

    [EnumMember(Value = "signal_sh01")]
    SignalSh01,

    [EnumMember(Value = "k84_einfach")]
    K84Single,

    [EnumMember(Value = "k84_doppelt")]
    K84Double,

    [EnumMember(Value = "entkuppler")]
    Uncoupler,

    [EnumMember(Value = "entkuppler_1")]
    Uncoupler1,

    [EnumMember(Value = "std_rot")]
    StdRed,

    [EnumMember(Value = "std_gruen")]
    StdGreen,

    [EnumMember(Value = "std_rot_gruen_0")]
    StdRedGreen0,

    [EnumMember(Value = "std_rot_gruen_1")]
    StdRedGreen1,

    [EnumMember(Value = "schiebebuehne_0")]
    Slideway0, 

    [EnumMember(Value = "schiebebuehne_1")]
    Slideway1,

    [EnumMember(Value = "schiebebuehne_2")]
    Slideway2,

    [EnumMember(Value = "schiebebuehne_3")]
    Slideway3,

    [EnumMember(Value = "drehscheibe_alt_0")]
    TurntableOld0,

    [EnumMember(Value = "drehscheibe_alt_1")]
    TurntableOld1,

    [EnumMember(Value = "drehscheibe_alt_2")]
    TurntableOld2,

    [EnumMember(Value = "drehscheibe_alt_3")]
    TurntableOld3,

    [EnumMember(Value = "drehscheibe_dig_0")]
    TurntableDigital0,

    [EnumMember(Value = "drehscheibe_dig_1")]
    TurntableDigital1,

    [EnumMember(Value = "drehscheibe_dig_2")]
    TurntableDigital2,

    [EnumMember(Value = "drehscheibe_dig_3")]
    TurntableDigital3,

    [EnumMember(Value = "drehscheibe_dig_4")]
    TurntableDigital4,

    [EnumMember(Value = "drehscheibe_dig_5")]
    TurntableDigital5,

    [EnumMember(Value = "drehscheibe_dig_6")]
    TurntableDigital6,

    [EnumMember(Value = "drehscheibe_dig_7")]
    TurntableDigital7,

    [EnumMember(Value = "drehscheibe_dig_8")]
    TurntableDigital8,

    [EnumMember(Value = "drehscheibe_dig_9")]
    TurntableDigital9,

    [EnumMember(Value = "drehscheibe_dig_10")]
    TurntableDigital10,

    [EnumMember(Value = "drehscheibe_dig_11")]
    TurntableDigital11,

    [EnumMember(Value = "drehscheibe_dig_12")]
    TurntableDigital12,

    [EnumMember(Value = "drehscheibe_dig_13")]
    TurntableDigital13,

    [EnumMember(Value = "drehscheibe_dig_14")]
    TurntableDigital014,

    [EnumMember(Value = "drehscheibe_dig_15")]
    TurntableDigital15,

    [EnumMember(Value = "drehscheibe_dig_16")]
    TurntableDigital16,

    [EnumMember(Value = "drehscheibe_dig_17")]
    TurntableDigital17,

    [EnumMember(Value = "drehscheibe_dig_18")]
    TurntableDigital18,

    [EnumMember(Value = "drehscheibe_dig_19")]
    TurntableDigital19,

    [EnumMember(Value = "drehscheibe_dig_20")]
    TurntableDigital20,

    [EnumMember(Value = "drehscheibe_dig_21")]
    TurntableDigital21,

    [EnumMember(Value = "drehscheibe_dig_22")]
    TurntableDigital22,

    [EnumMember(Value = "drehscheibe_dig_23")]
    TurntableDigital23,

    [EnumMember(Value = "drehscheibe_dig_24")]
    TurntableDigital24,

    [EnumMember(Value = "drehscheibe_dig_25")]
    TurntableDigital25,

    [EnumMember(Value = "drehscheibe_dig_26")]
    TurntableDigital26,

    [EnumMember(Value = "drehscheibe_dig_27")]
    TurntableDigital27,

    [EnumMember(Value = "drehscheibe_dig_28")]
    TurntableDigital28,

    [EnumMember(Value = "drehscheibe_dig_29")]
    TurntableDigital29,

    [EnumMember(Value = "drehscheibe_dig_30")]
    TurntableDigital30,

    [EnumMember(Value = "drehscheibe_dig_31")]
    TurntableDigital31,



}
