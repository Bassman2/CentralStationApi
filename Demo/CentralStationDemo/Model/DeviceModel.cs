namespace CentralStationDemo.Model;

public class DeviceModel
{
    [JsonPropertyName("uid")]
    public uint Id { get; set; }

    [JsonPropertyName("uid")]
    public Version Version { get; set; } = new Version(0, 0);

    [JsonPropertyName("type")]
    public DeviceType Type { get; set; }

    [JsonPropertyName("serial")]
    public string SerialNumber { get; set; } = null!;

    [JsonPropertyName("article")]
    public string ArticleNumber { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;


    [JsonPropertyName("numOfMeasurements")]
    public int NumberOfMeasurements { get; set; }

    [JsonPropertyName("numOfChannels")]
    public int NumberOfChannels { get; set; }

}
