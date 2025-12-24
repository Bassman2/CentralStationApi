namespace CentralStationWebApi.Serializer;

[AttributeUsage(AttributeTargets.Property)]
internal class CsPropertyAttribute(string name) : Attribute
{
    public string Name => name;
}
