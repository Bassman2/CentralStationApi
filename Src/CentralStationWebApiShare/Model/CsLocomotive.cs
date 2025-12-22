using System.Globalization;

namespace CentralStationWebApi.Model;

public class CsLocomotive : ICsSerialize
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
    //            case " .uid":
    //                UID = uint.Parse(parts[1], NumberStyles.HexNumber );
    //                break;
    //            case " .name":
    //                Name = parts[1];
    //                break;
    //            default:
    //                throw new InvalidDataException($"Unknown lokomotive section {line}");
    //            }
    //        }
    //    }

    //}

    public ICsSerialize DeserializeLeave(string line)
    {
        switch (line.Trim(' ', '.'))
        {
        //case "funktionen":
        //    return Version = new CsVersion();
        //case "lokomotive":
        //    var leave = new CsLocomotive();
        //    Locomotives ??= [];
        //    Locomotives.Add(leave);
        //    return leave;
        default:
            throw new InvalidDataException($"Unknown lokomotive section {line}");
        }
    }

    public void DeserializeProperty(string name, string value)
    {
        switch (name.Trim(' ', '.'))
        {
        case "major":
            UID = uint.Parse(value, NumberStyles.HexNumber); 
            break;
        case "minor":
            Name = value;
            break;
        case "mfxtyp":
            MfxTyp = byte.Parse(value);
            break;
        case "blocks":
            Blocks = value.Split(' ', StringSplitOptions.TrimEntries).Select(i => byte.Parse(i)).ToArray();
            break;
        default:
            throw new InvalidDataException($"Unknown property {name}");
        }
    }

    public uint UID { get; private set; }

    public string? Name { get; private set; }

    public byte MfxTyp { get; private set; }

    public byte[] Blocks { get; private set; }
}
