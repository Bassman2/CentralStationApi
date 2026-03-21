namespace CentralStationApi.Model;

/// <summary>
/// Specifies the type of item in a route sequence.
/// Defines what kind of operation or command is performed as part of a route execution.
/// </summary>
[EnumConverter]
public enum RouteItemType
{
    /// <summary>
    /// Magnetic article/accessory type.
    /// Represents a magnetic accessory command, such as switching a turnout or changing a signal aspect.
    /// This is the standard type for controlling track accessories with electromagnetic drives.
    /// </summary>
    [EnumMember(Value = "mag")]
    Mag,

}
