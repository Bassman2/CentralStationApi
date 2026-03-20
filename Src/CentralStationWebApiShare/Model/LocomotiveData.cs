namespace CentralStationWebApi.Model;

/// <summary>
/// Represents the collection of locomotive data from the Central Station.
/// Contains version information and a list of all locomotives configured in the system.
/// </summary>
[CsSerialize]
public partial class LocomotiveData 
{
    /// <summary>
    /// Gets the version information of the locomotive data.
    /// </summary>
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    /// <summary>
    /// Gets the list of locomotives configured in the Central Station.
    /// Each locomotive includes decoder information, address, functions, and other settings.
    /// </summary>
    [CsProperty("lokomotive")]
    public List<Locomotive>? Locomotives { get; private set; }
}
