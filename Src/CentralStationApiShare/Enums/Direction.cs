namespace CentralStationApi;

/// <summary>
/// Specifies the direction of travel for a locomotive.
/// Represents the absolute direction state (forward or backward/reverse).
/// </summary>
/// <remarks>
/// This enum represents the current direction state of a locomotive, as opposed to
/// <see cref="DirectionChange"/> which represents direction change commands.
/// The direction is typically reported in locomotive status responses and configuration data.
/// </remarks>
[EnumConverter]
public enum Direction
{
    /// <summary>
    /// Forward direction.
    /// The locomotive moves in the forward direction (typically toward the front of the locomotive).
    /// </summary>
    Forward = 0,

    /// <summary>
    /// Backward (reverse) direction.
    /// The locomotive moves in the backward/reverse direction (typically toward the rear of the locomotive).
    /// </summary>
    Backward = 1
}

