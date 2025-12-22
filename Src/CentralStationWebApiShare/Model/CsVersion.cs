namespace CentralStationWebApi.Model;

public class CsVersion : ICsSerialize
{
    //public void Deserialize(StreamReader reader, int level)
    //{
    //    string? line;
    //    while ((line = reader.ReadLine()) != null)
    //    {
    //        // subclasses
    //        if (!line.Contains('='))
    //        {
    //            throw new InvalidDataException($"Unknown section {line}");
    //        }
    //        else
    //        {
    //            var parts = line.Split('=', 2);
    //            switch (parts[0])
    //            {
    //            case " .major":
    //                Major = int.Parse(parts[1]);
    //                break;
    //            case " .minor":
    //                Minor = int.Parse(parts[1]);
    //                break;
    //            default:
    //                throw new InvalidDataException($"Unknown property {parts[0]}");
    //            }
    //        }
    //    }
    //}

    public ICsSerialize DeserializeLeave(string line)
    {
        throw new InvalidDataException($"Unknown section {line}");
    }

    public void DeserializeProperty(string name, string value)
    {
        switch (name.Trim(' ', '.'))
        {
        case "major":
            Major = int.Parse(value);
            break;
        case "minor":
            Minor = int.Parse(value);
            break; 
        default:
            throw new InvalidDataException($"Unknown property {name}");
        }
        
    }

    public int Major { get; private set; } = 0;

    public int Minor { get; private set; } = 0;

}

