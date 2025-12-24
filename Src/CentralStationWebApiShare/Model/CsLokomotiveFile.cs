namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsLokomotiveFile 
{
    private List<CsLocomotive>? locomotives;

    //public ICsSerialize DeserializeLeave(string line)
    //{
    //    switch (line)
    //    {
    //    case "version":
    //        return Version = new CsVersion();
    //    case "lokomotive":
    //        //var locomotive = new CsLocomotive();
    //        //Locomotives ??= [];
    //        //Locomotives.Add(locomotive);
    //        //return locomotive;
    //        return CsSerializer.AddToList(ref locomotives, new CsLocomotive());
    //    default:
    //        throw new InvalidDataException($"Unknown lokomotive file section {line}");
    //    }
    //}

    //public void DeserializeProperty(string name, string value)
    //{
    //    // no properties
    //    throw new InvalidDataException($"Unknown lokomotive file property {name}");
    //}

    public CsVersion? Version { get; private set; }

    public List<CsLocomotive>? Locomotives => locomotives;
}
