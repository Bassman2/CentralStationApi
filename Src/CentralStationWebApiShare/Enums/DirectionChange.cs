namespace CentralStationWebApi;

/// <summary>
/// Specifies the direction change command for locomotive control.
/// Used to set or modify the direction of travel for a locomotive.
/// </summary>
/// <remarks>
/// Direction commands are sent using the <see cref="Command.LocoDirection"/> command.
/// The actual direction depends on the locomotive's current state and the command sent.
/// Some values toggle or maintain the current direction, while others set an absolute direction.
/// </remarks>
public enum DirectionChange : byte
{
    /// <summary>
    /// Remain in current direction.
    /// The locomotive continues moving in its current direction without change.
    /// </summary>
    Remain = 0,

    /// <summary>
    /// Set direction to forward.
    /// The locomotive will move in the forward direction (typically the direction of the front of the locomotive).
    /// </summary>
    Forward = 1,

    /// <summary>
    /// Set direction to backward (reverse).
    /// The locomotive will move in the backward/reverse direction (typically toward the rear of the locomotive).
    /// </summary>
    Backward = 2,

    /// <summary>
    /// Toggle direction.
    /// Reverses the current direction - forward becomes backward, backward becomes forward.
    /// </summary>
    Toggle = 3,

    /// <summary>
    /// Unknown direction command 4.
    /// Purpose and behavior are undocumented.
    /// </summary>
    Unknown4 = 4,

    /// <summary>
    /// Unknown direction command 5.
    /// Purpose and behavior are undocumented.
    /// </summary>
    Unknown5 = 5

}
