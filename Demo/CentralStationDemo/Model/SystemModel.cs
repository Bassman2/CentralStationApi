namespace CentralStationDemo.Model;

public class SystemModel
{
    [JsonPropertyName("cs3")]
    public List<DeviceModel> CentralStation3 { get; set; } = [];

    [JsonPropertyName("cs2")]
    public List<DeviceModel> CentralStation2 { get; set; } = [];

    [JsonPropertyName("tfp3")]
    public List<DeviceModel> TrackFormatProcessor3 { get; set; } = [];

    [JsonPropertyName("ms2")]
    public List<DeviceModel> MobileStation2 { get; set; } = [];

    [JsonPropertyName("ms1")]
    public List<DeviceModel> MobileStation1 { get; set; } = [];

    [JsonPropertyName("linkS88")]
    public List<DeviceModel> LinkS88 { get; set; } = [];

    [JsonPropertyName("connect6021")]
    public List<DeviceModel> Connect6021 { get; set; } = [];

    [JsonPropertyName("booster")]
    public List<DeviceModel> Booster { get; set; } = [];

    [JsonPropertyName("webApp")]
    public List<DeviceModel> WebApp { get; set; } = [];

    [JsonPropertyName("browser")]
    public List<DeviceModel> Browser { get; set; } = [];

    [JsonPropertyName("unknown")]
    public List<DeviceModel> Unknown { get; set; } = [];
}
