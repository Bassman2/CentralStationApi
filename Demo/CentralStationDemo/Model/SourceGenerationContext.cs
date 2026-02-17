namespace CentralStationDemo.Model;

[JsonSourceGenerationOptions(
    //JsonSerializerDefaults.Web,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip)]
[JsonSerializable(typeof(CentralStationModel))]
internal partial class SourceGenerationContext : JsonSerializerContext
{ }

