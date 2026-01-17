namespace CentralStationWebApi;

[AttributeUsage(AttributeTargets.Field)]
public class FileNameIdAttribute(int id) : Attribute
{
    public int Id => id;
}
