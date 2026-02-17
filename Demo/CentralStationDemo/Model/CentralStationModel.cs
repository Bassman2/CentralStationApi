using System.IO;
using System.Text.Json.Serialization.Metadata;

namespace CentralStationDemo.Model;

public class CentralStationModel : IJsonOnDeserialized
{
    #region file handling

    private static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CentralStationDemo", "CentralStationModel.json");

    public static CentralStationModel? LoadOrCreate()
    {
        JsonTypeInfo<CentralStationModel> jsonTypeInfo = (JsonTypeInfo<CentralStationModel>)SourceGenerationContext.Default.GetTypeInfo(typeof(CentralStationModel))!;

        if (!File.Exists(path))
        {
            return new CentralStationModel();
        }

        using Stream stream = File.OpenRead(path);
        CentralStationModel? centralStationModel = JsonSerializer.Deserialize<CentralStationModel>(stream, jsonTypeInfo);
        return centralStationModel;
    }

    public void Save()
    {
        if (File.Exists(path))
        {
            int index = 1;
            string backupPath;
            // Loop until finding a backup filename that doesn't exist.
            do
            {
                backupPath = Path.ChangeExtension(path, $"{index:D3}.json");
                index++;
            } while (File.Exists(backupPath));

            File.Move(path, backupPath);
        }

        JsonTypeInfo<CentralStationModel> jsonTypeInfo = (JsonTypeInfo<CentralStationModel>)SourceGenerationContext.Default.GetTypeInfo(typeof(CentralStationModel))!;
        using Stream stream = File.Create(path);
        JsonSerializer.Serialize(stream, this, jsonTypeInfo);
    }

    public void OnDeserialized()
    { }

    #endregion

    [JsonPropertyName("locomotives")]
    public List<LocomotiveModel> Locomotives { get; set; } = [];

    [JsonPropertyName("articles")]
    public List<ArticleModel> Articles { get; set; } = [];

    [JsonPropertyName("system")]
    public SystemModel System { get; set; } = new SystemModel();


}



