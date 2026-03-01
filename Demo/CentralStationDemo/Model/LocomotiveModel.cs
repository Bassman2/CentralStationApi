namespace CentralStationDemo.Model;

public class LocomotiveModel
{
    public LocomotiveModel()
    { }

    public LocomotiveModel(Locomotive loco)
    {
        Name = loco.Name;
        Uid = loco.Uid;
        MfxUid = loco.MfxUid;
        Address = loco.Address;
        IconName = loco.IconName;
        Type = loco.Type;
        MfxType = loco.MfxType;
        Symbol = loco.Symbol;

        Sid = loco.Sid;
        MaxSpeed = loco.MaxSpeed;
        VMax = loco.VMax;
        VMin = loco.VMin;
        Av = loco.Av;
        Bv = loco.Bv;
        Volume = loco.Volume;
        Spa = loco.Spa;
        Spm = loco.Spm;
        Velocity = loco.Velocity;
        Direction = loco.Direction;

        Functions = (loco.Functions?.Select(f => new FunctionModel(f)) ?? []).Concat(loco.Functions2?.Select(f => new FunctionModel(f)) ?? []).ToList() ?? [];

        //[..loco.Functions!.Select(f => new FunctionViewModel(this, f)).
        //        Concat(loco.Functions2?.Select(f => new FunctionViewModel(this, f)) ?? [])];

    }


    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uid")]
    public uint Uid { get; set; }

    [JsonPropertyName("mfxUid")]
    public uint MfxUid { get; set; }

    [JsonPropertyName("address")]
    public uint Address { get; set; }

    [JsonPropertyName("iconName")]
    public string? IconName { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<DecoderType>))]
    public DecoderType Type { get; set; }

    [JsonPropertyName("mfxType")]
    public uint MfxType { get; set; }

    [JsonPropertyName("symbol")]
    [JsonConverter(typeof(JsonStringEnumConverter<Symbol>))]
    public Symbol Symbol { get; set; }

    [JsonPropertyName("sid")]
    public uint Sid { get; set; }

    [JsonPropertyName("maxSpeed")]
    public uint MaxSpeed { get; set; }

    [JsonPropertyName("vMax")]
    public uint VMax { get; set; }

    [JsonPropertyName("vMin")]
    public uint VMin { get; set; }

    [JsonPropertyName("av")]
    public uint Av { get; set; }

    [JsonPropertyName("bv")]
    public uint Bv { get; set; }

    [JsonPropertyName("volume")]
    public uint Volume { get; set; }

    [JsonPropertyName("spa")]
    public uint Spa { get; set; }

    [JsonPropertyName("spm")]
    public uint Spm { get; set; }

    [JsonPropertyName("velocity")]
    public ushort Velocity { get; set; }

    [JsonPropertyName("direction")]
    [JsonConverter(typeof(JsonStringEnumConverter<Direction>))]
    public Direction Direction { get; set; }

    [JsonPropertyName("functions")]
    public List<FunctionModel> Functions { get; set; } = [];
}
