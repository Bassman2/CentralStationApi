namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsVersion 
{
    //public ICsSerialize DeserializeLeave(string line)
    //{
    //    throw new InvalidDataException($"Unknown version section {line}");
    //}

    //public void DeserializeProperty(string name, string value)
    //{
    //    switch (name)
    //    {
    //    case "major":
    //        Major = int.Parse(value);
    //        break;
    //    case "minor":
    //        Minor = int.Parse(value);
    //        break; 
    //    default:
    //        throw new InvalidDataException($"Unknown version property {name}");
    //    }
    //}

    [CsProperty("major")]
    public int Major { get; private set; } = 0;

    [CsProperty("minor")]
    public int Minor { get; private set; } = 0;

}

