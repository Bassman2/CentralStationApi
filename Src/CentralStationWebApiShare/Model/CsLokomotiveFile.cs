namespace CentralStationWebApi.Model;

public class CsLokomotiveFile : ICsSerialize
{
    //public void Deserialize(StreamReader reader)
    //{
    //    string? line = reader.ReadLine();
    //    if (line == null || !line.StartsWith("[lokomotive]"))
    //    {
    //        throw new InvalidDataException("Invalid lokomotive data");
    //    }   
    //    while ((line = reader.ReadLine()) != null)
    //    {
    //        // subclasses
    //        if (!line.Contains('='))
    //        {
    //            switch (line)
    //            {
    //            case "version":
    //                Version = new CsVersion();
    //                Version.Deserialize(reader);
    //                break;
    //            case "lokomotive":
                    
    //                var locomotive = new CsLocomotive();
    //                locomotive.Deserialize(reader);
    //                Locomotives ??= [];
    //                Locomotives.Add(locomotive);
    //                break;
    //            default:
    //                throw new InvalidDataException($"Unknown lokomotive section {line}");
    //            }
    //        }
    //        else
    //        {
    //            var parts = line.Split('=', 2);
    //        }
    //    }
    //}

    public ICsSerialize DeserializeLeave(string line)
    {
        switch (line.Trim(' ', '.'))
        {
        case "version":
            return Version = new CsVersion();
        case "lokomotive":
            var leave = new CsLocomotive();
            Locomotives ??= [];
            Locomotives.Add(leave);
            return leave;
        default:
            throw new InvalidDataException($"Unknown lokomotive section {line}");
        }
    }

    public void DeserializeProperty(string name, string value)
    {
        // no properties
        throw new InvalidDataException($"Unknown property {name}");
    }

    public CsVersion? Version { get; private set; }

    public List<CsLocomotive>? Locomotives { get; private set; }
}
