namespace CentralStationWebApi.Serializer;

public interface ICsSerialize
{
    //void Deserialize(StreamReader reader, int level);

    ICsSerialize DeserializeLeave(string line);
    void DeserializeProperty(string name, string value);
}
