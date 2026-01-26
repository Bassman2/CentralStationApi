namespace CentralStationWebApi;

[AttributeUsage(AttributeTargets.Field)]
public class FileNameAttribute(string fileName) : Attribute
{
    public string FileName => fileName;
}
