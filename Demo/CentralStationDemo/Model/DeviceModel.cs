namespace CentralStationDemo.Model;

public class DeviceModel
{
    public DeviceModel()
    { }

    public DeviceModel(Device device)
    {
        Id = device.DeviceId;
        Version = device.Version;
        Type = device.DeviceType;
        TypeName = device.DeviceTypeName;
        ImagePath = device.ImagePath;

    }

    // SoftwareVersion

    [JsonPropertyName("uid")]
    public uint Id { get; set; }

    [JsonPropertyName("version")]
    public Version Version { get; set; } = new Version(0, 0);

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<DeviceType>))]
    public DeviceType Type { get; set; }

    [JsonPropertyName("typeName")]
    public string TypeName { get; set; }

    [JsonPropertyName("imagePath")]
    public string? ImagePath { get; set; }


    [JsonPropertyName("serial")]
    public uint SerialNumber { get; set; }

    // StatusData

    [JsonPropertyName("article")]
    public string ArticleNumber { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;


    [JsonPropertyName("numOfMeasurements")]
    public int NumberOfMeasurements { get; set; }

    [JsonPropertyName("numOfChannels")]
    public int NumberOfChannels { get; set; }


    //public static implicit operator DeviceData(DeviceModel model) => new DeviceData(model.Id, model.Version, model.Type, model.SerialNumber, model.ArticleNumber, model.Name);
}
