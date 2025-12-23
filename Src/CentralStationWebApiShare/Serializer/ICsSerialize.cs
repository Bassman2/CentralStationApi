namespace CentralStationWebApi.Serializer;

public interface ICsSerialize
{
    ICsSerialize DeserializeLeave(string line);
    void DeserializeProperty(string name, string value);
}
