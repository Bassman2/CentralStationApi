using CentralStationWebApi.Serializer;

namespace CentralStationWebApi.Model;

public class CsLokomotiveFile : ICsSerialize
{
    public void Deserialize(StreamReader reader)
    {
        string? line = reader.ReadLine();
        if (line == null || !line.StartsWith("[lokomotive]"))
        {
            throw new InvalidDataException("Invalid lokomotive data");
        }   
        while ((line = reader.ReadLine()) != null)
        {
            // subclasses
            if (!line.Contains('='))
            {
                switch (line)
                {
                case "version":
                    Version = new CsVersion();
                    Version.Deserialize(reader);
                    break;
                case "lokomotive":
                    
                    var locomotive = new CsLocomotive();
                    locomotive.Deserialize(reader);
                    Locomotives ??= [];
                    Locomotives.Add(locomotive);
                    break;
                default:
                    throw new InvalidDataException($"Unknown lokomotive section {line}");
                }
            }
            else
            {
                var parts = line.Split('=', 2);
            }
        }
    }

    public CsVersion? Version { get; private set; }

    public List<CsLocomotive>? Locomotives { get; private set; }
}
