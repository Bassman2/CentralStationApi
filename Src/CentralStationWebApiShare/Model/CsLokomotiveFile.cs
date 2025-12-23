namespace CentralStationWebApi.Model;

public class CsLokomotiveFile : ICsSerialize
{
    public ICsSerialize DeserializeLeave(string line)
    {
        switch (line)
        {
        case "version":
            return Version = new CsVersion();
        case "lokomotive":
            var locomotive = new CsLocomotive();
            Locomotives ??= [];
            Locomotives.Add(locomotive);
            return locomotive;
        default:
            throw new InvalidDataException($"Unknown lokomotive file section {line}");
        }
    }

    public void DeserializeProperty(string name, string value)
    {
        // no properties
        throw new InvalidDataException($"Unknown lokomotive file property {name}");
    }

    public CsVersion? Version { get; private set; }

    public List<CsLocomotive>? Locomotives { get; private set; }
}
