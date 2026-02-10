using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsVersion 
{
    [CsProperty("major")]
    public int Major { get; private set; } = 0;

    [CsProperty("minor")]
    public int Minor { get; private set; } = 0;

    public static implicit operator Version(CsVersion ver) => new Version(ver.Major, ver.Minor);
}

