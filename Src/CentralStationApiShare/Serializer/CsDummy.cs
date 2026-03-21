namespace CentralStationApi.Serializer;

internal class CsDummy(string name) : ICsSerialize
{
    public ICsSerialize DeserializeLeave(string line)
    {
        return new CsDummy(line);
    }

    public void DeserializeProperty(string name, string value)
    { }

    public string Name => name;
}
