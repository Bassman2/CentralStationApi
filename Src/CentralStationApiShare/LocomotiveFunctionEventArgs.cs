namespace CentralStationApi;

/// <summary>
/// Provides data for locomotive function change events.
/// Contains the locomotive identifier, function number, and the new function state or value.
/// </summary>
/// <remarks>
/// This event is raised when a locomotive function (lights, sound, smoke, etc.) is changed
/// via the Central Station. Functions can have simple on/off states or complex values for
/// features like sound volume or light intensity. The event is triggered by
/// <see cref="Command.LocoFunction"/> responses on the CAN bus.
/// </remarks>
public class LocomotiveFunctionEventArgs(uint locomotiveId, byte function, byte value, ushort? functionValue = null) : LocomotiveEventArgs(locomotiveId)
{
    /// <summary>
    /// Gets the function number that was changed.
    /// </summary>
    /// <value>
    /// The function index, typically 0-31 depending on decoder capabilities.
    /// Common functions include F0 (headlight), F1-F4 (various lights/sounds), etc.
    /// </value>
    public byte Function => function;

    /// <summary>
    /// Gets the function state or value.
    /// </summary>
    /// <value>
    /// For toggle functions: 0 = off, 1 = on.
    /// For momentary functions: 0 = released, 1 = pressed.
    /// For dimmer/volume functions: 0-255 representing the intensity level.
    /// </value>
    public byte Value => value;

    /// <summary>
    /// Gets a value indicating whether an extended function value is present.
    /// </summary>
    /// <value>
    /// True if the function supports extended 16-bit values (e.g., sound volume, light dimming);
    /// false if only the basic 8-bit <see cref="Value"/> is used.
    /// </value>
    public bool HasFunctionValue => functionValue.HasValue;

    /// <summary>
    /// Gets the extended 16-bit function value.
    /// </summary>
    /// <value>
    /// A 16-bit value (0-65535) for functions that support extended parameters,
    /// such as sound volume levels or precise light dimming values.
    /// Returns 0 if <see cref="HasFunctionValue"/> is false.
    /// </value>
    public ushort FunctionValue => functionValue.GetValueOrDefault();
    
}
