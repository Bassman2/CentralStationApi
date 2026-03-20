namespace CentralStationWebApi.Model;

/// <summary>
/// Specifies the type of track element that can be placed on a Central Station track layout.
/// Each element type represents a different track piece, accessory, or decoration.
/// </summary>
/// <remarks>
/// Element types are used in the track layout editor to define what kind of track component
/// is placed at each position. The type determines the visual representation and connectivity
/// of the track element. Many types have variants for different positions or configurations,
/// particularly for turntables which have 32 position variants (0-31).
/// </remarks>
[EnumConverter]
public enum ElementType
{
    /// <summary>
    /// Empty grid position with no track element.
    /// </summary>
    [EnumMember(Value = "leer")]
    Empty = 0,

    /// <summary>
    /// Straight track section (Gerade).
    /// </summary>
    [EnumMember(Value = "gerade")]
    Straight,

    /// <summary>
    /// Track crossing where two tracks intersect (Kreuzung).
    /// </summary>
    [EnumMember(Value = "kreuzung")]
    Crossing,

    /// <summary>
    /// Underpass/bridge where one track passes under another (Unterführung).
    /// </summary>
    [EnumMember(Value = "unterfuehrung")]
    Underpass,

    /// <summary>
    /// Buffer stop / bumper at the end of a track (Prellbock).
    /// </summary>
    [EnumMember(Value = "prellbock")]
    BufferStop,

    /// <summary>
    /// Curved track section (Bogen).
    /// </summary>
    [EnumMember(Value = "bogen")]
    Curve,

    /// <summary>
    /// Double curve / S-curve track section (Doppelbogen).
    /// </summary>
    [EnumMember(Value = "doppelbogen")]
    DoubleCurve,

    /// <summary>
    /// Tunnel section.
    /// </summary>
    [EnumMember(Value = "tunnel")]
    Tunnel,

    /// <summary>
    /// Left turnout / switch (Linksweiche).
    /// </summary>
    [EnumMember(Value = "linksweiche")]
    LeftTurnout,

    /// <summary>
    /// Right turnout / switch (Rechtsweiche).
    /// </summary>
    [EnumMember(Value = "rechtsweiche")]
    RightTurnout,

    /// <summary>
    /// Three-way turnout / switch (Dreiwegweiche).
    /// </summary>
    [EnumMember(Value = "dreiwegweiche")]
    ThreeWaySwitch,

    /// <summary>
    /// Y-shaped turnout / switch (Y-Weiche).
    /// </summary>
    [EnumMember(Value = "yweiche")]
    YSwitch,

    /// <summary>
    /// Double slip switch (Doppelkreuzweiche).
    /// </summary>
    [EnumMember(Value = "dkweiche")]
    DoubleSlipSwitch,

    /// <summary>
    /// Double slip switch variant 2 (Doppelkreuzweiche).
    /// </summary>
    [EnumMember(Value = "dkweiche_2")]
    DoubleSlipSwitch2,

    /// <summary>
    /// Generic signal element.
    /// </summary>
    [EnumMember(Value = "signal")]
    Signal,

    /// <summary>
    /// S88 feedback contact for track occupancy detection.
    /// </summary>
    [EnumMember(Value = "s88kontakt")]
    S88Contact,

    /// <summary>
    /// S88 feedback contact on curved track section.
    /// </summary>
    [EnumMember(Value = "s88bogen")]
    S88Curve,

    /// <summary>
    /// Reference arrow / direction indicator (Pfeil).
    /// </summary>
    [EnumMember(Value = "pfeil")]
    Reference,

    /// <summary>
    /// Railway line / route indicator (Fahrstraße).
    /// </summary>
    [EnumMember(Value = "fahrstrasse")]
    RailwayLine,

    /// <summary>
    /// Text label or annotation.
    /// </summary>
    [EnumMember(Value = "text")]
    Text,

    /// <summary>
    /// Signal with Hp0/2 aspects (stop/clear).
    /// </summary>
    [EnumMember(Value = "signal_hp02")]
    SignalHp02,

    /// <summary>
    /// Signal with Hp0/1/2 aspects (stop/proceed/clear).
    /// </summary>
    [EnumMember(Value = "signal_hp012")]
    SignalHp12,

    /// <summary>
    /// Signal with Hp0/1 aspects and shunting capability (Hp01s).
    /// </summary>
    [EnumMember(Value = "signal_hp01s")]
    SignalHp01s,

    /// <summary>
    /// Position light signal with Hp0/1/2 aspects and shunting (P-Hp012s).
    /// </summary>
    [EnumMember(Value = "signal_p_hp012s")]
    SignalPHp012s,

    /// <summary>
    /// Form signal (semaphore) with Hp0/1/2 aspects and shunting (F-Hp012s).
    /// </summary>
    [EnumMember(Value = "signal_f_hp012s")]
    SignalFHp012s,

    /// <summary>
    /// Position light signal with Hp0/1/2 aspects (P-Hp012).
    /// </summary>
    [EnumMember(Value = "signal_p_hp012")]
    SignalPHp012,

    /// <summary>
    /// Form signal (semaphore) with Hp0/1 aspects (F-Hp01).
    /// </summary>
    [EnumMember(Value = "signal_f_hp01")]
    SignalFHp01,

    /// <summary>
    /// Form signal (semaphore) with Hp0/2 aspects (F-Hp02).
    /// </summary>
    [EnumMember(Value = "signal_f_hp02")]
    SignalFHp02,

    /// <summary>
    /// Form signal (semaphore) with Hp0/1/2 aspects (F-Hp012).
    /// </summary>
    [EnumMember(Value = "signal_f_hp012")]
    SignalFHp012,

    /// <summary>
    /// Shunting signal with Sh0/1 aspects.
    /// </summary>
    [EnumMember(Value = "signal_sh01")]
    SignalSh01,

    /// <summary>
    /// K84 decoder single output.
    /// </summary>
    [EnumMember(Value = "k84_einfach")]
    K84Single,

    /// <summary>
    /// K84 decoder double output.
    /// </summary>
    [EnumMember(Value = "k84_doppelt")]
    K84Double,

    /// <summary>
    /// Uncoupling track (Entkuppler) for automatic car uncoupling.
    /// </summary>
    [EnumMember(Value = "entkuppler")]
    Uncoupler,

    /// <summary>
    /// Uncoupling track variant 1 (Entkuppler).
    /// </summary>
    [EnumMember(Value = "entkuppler_1")]
    Uncoupler1,

    /// <summary>
    /// Standard red indicator/control element.
    /// </summary>
    [EnumMember(Value = "std_rot")]
    StdRed,

    /// <summary>
    /// Standard green indicator/control element.
    /// </summary>
    [EnumMember(Value = "std_gruen")]
    StdGreen,

    /// <summary>
    /// Standard red/green indicator/control element position 0.
    /// </summary>
    [EnumMember(Value = "std_rot_gruen_0")]
    StdRedGreen0,

    /// <summary>
    /// Standard red/green indicator/control element position 1.
    /// </summary>
    [EnumMember(Value = "std_rot_gruen_1")]
    StdRedGreen1,

    /// <summary>
    /// Transfer table (Schiebebühne) position 0.
    /// </summary>
    [EnumMember(Value = "schiebebuehne_0")]
    Slideway0,

    /// <summary>
    /// Transfer table (Schiebebühne) position 1.
    /// </summary>
    [EnumMember(Value = "schiebebuehne_1")]
    Slideway1,

    /// <summary>
    /// Transfer table (Schiebebühne) position 2.
    /// </summary>
    [EnumMember(Value = "schiebebuehne_2")]
    Slideway2,

    /// <summary>
    /// Transfer table (Schiebebühne) position 3.
    /// </summary>
    [EnumMember(Value = "schiebebuehne_3")]
    Slideway3,

    /// <summary>
    /// Old-style turntable (Drehscheibe alt) position 0.
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt_0")]
    TurntableOld0,

    /// <summary>
    /// Old-style turntable (Drehscheibe alt) position 1.
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt_1")]
    TurntableOld1,

    /// <summary>
    /// Old-style turntable (Drehscheibe alt) position 2.
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt_2")]
    TurntableOld2,

    /// <summary>
    /// Old-style turntable (Drehscheibe alt) position 3.
    /// </summary>
    [EnumMember(Value = "drehscheibe_alt_3")]
    TurntableOld3,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 0.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_0")]
    TurntableDigital0,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 1.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_1")]
    TurntableDigital1,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 2.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_2")]
    TurntableDigital2,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 3.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_3")]
    TurntableDigital3,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 4.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_4")]
    TurntableDigital4,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 5.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_5")]
    TurntableDigital5,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 6.
    /// </summary>
    /// [EnumMember(Value = "drehscheibe_dig_6")]
    TurntableDigital6,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 7.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_7")]
    TurntableDigital7,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 8.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_8")]
    TurntableDigital8,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 9.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_9")]
    TurntableDigital9,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 10.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_10")]
    TurntableDigital10,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 11.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_11")]
    TurntableDigital11,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 12.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_12")]
    TurntableDigital12,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 13.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_13")]
    TurntableDigital13,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 14.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_14")]
    TurntableDigital014,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 15.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_15")]
    TurntableDigital15,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 16.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_16")]
    TurntableDigital16,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 17.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_17")]
    TurntableDigital17,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 18.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_18")]
    TurntableDigital18,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 19.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_19")]
    TurntableDigital19,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 20.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_20")]
    TurntableDigital20,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 21.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_21")]
    TurntableDigital21,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 22.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_22")]
    TurntableDigital22,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 23.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_23")]
    TurntableDigital23,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 24.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_24")]
    TurntableDigital24,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 25.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_25")]
    TurntableDigital25,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 26.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_26")]
    TurntableDigital26,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 27.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_27")]
    TurntableDigital27,
    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 28.
    /// </summary>

    [EnumMember(Value = "drehscheibe_dig_28")]
    TurntableDigital28,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 29.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_29")]
    TurntableDigital29,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 30.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_30")]
    TurntableDigital30,

    /// <summary>
    /// Digital turntable (Drehscheibe digital) position 31.
    /// </summary>
    [EnumMember(Value = "drehscheibe_dig_31")]
    TurntableDigital31,
}
