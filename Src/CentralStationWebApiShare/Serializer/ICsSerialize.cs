namespace CentralStationWebApi.Serializer;

public interface ICsSerialize
{
    void Deserialize(StreamReader reader);
}
