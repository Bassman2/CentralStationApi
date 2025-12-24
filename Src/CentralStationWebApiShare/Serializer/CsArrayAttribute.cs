namespace CentralStationWebApi.Serializer;

[AttributeUsage(AttributeTargets.Property)]
internal class CsArrayAttribute(string name) : Attribute
{
    public string Name => name;
}
