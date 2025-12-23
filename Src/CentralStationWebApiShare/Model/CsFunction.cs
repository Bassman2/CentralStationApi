namespace CentralStationWebApi.Model;

public class CsFunction : ICsSerialize
{
    public ICsSerialize DeserializeLeave(string line)
    {
        throw new InvalidDataException($"Unknown functionen section {line}");
    }

    public void DeserializeProperty(string name, string value)
    {
        switch (name)
        {
        case "nr":
            Num = int.Parse(value);
            break;
        case "typ":
            Type = int.Parse(value);
            break;
        case "wert":
            Value = int.Parse(value);
            break;
        default:
            throw new InvalidDataException($"Unknown functionsn property {name}");
        }

    }

    public int Num { get; private set; }
    public int Type { get; private set; }
    public int Value { get; private set; }
}
