namespace CentralStationWebApi.Model;

[EnumConverter]
public enum DecoderType
{
    Unknown = 0,

    [EnumMember(Value = "mm2")]
    MM2,

    [EnumMember(Value = "mm2_prg")]
    MM2Prg,

    [EnumMember(Value = "mm2_dil8")]
    MM2Dil8,

    [EnumMember(Value = "dcc")]
    DCC,

    [EnumMember(Value = "mfx")]
    MFX,

    [EnumMember(Value = "sx1")]
    ESX1,

    [EnumMember(Value = "mm_prg")]
    MMPrg        
}
