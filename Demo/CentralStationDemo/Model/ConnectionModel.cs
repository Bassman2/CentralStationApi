using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationDemo.Model;

public class ConnectionModel
{
    [JsonPropertyName("host")]
    public string Host { get; set; } = "CS3";

    [JsonPropertyName("protocol")]
    [JsonConverter(typeof(JsonStringEnumConverter<Protocol>))]
    public Protocol Protocol { get; set; } = Protocol.TCP;

    [JsonPropertyName("device")]
    public DeviceModel Device { get; set; } = new DeviceModel() { Id = 0x6D554711, Version = new Version(1, 2), Type = DeviceType.Application, SerialNumber = 11111, ArticleNumber = "22222", Name = "DemoApp" };

}
