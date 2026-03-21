namespace CentralStationApi.Model;

/// <summary>
/// Represents a locomotive function (e.g., lights, sound, smoke) in the Central Station.
/// Functions are controllable features of a locomotive decoder that can be toggled or set to specific values.
/// </summary>
[CsSerialize]
public partial class Function 
{
    /// <summary>
    /// Gets the function number/index.
    /// Typically ranges from 0 (headlight) to 31 or higher depending on decoder capabilities.
    /// </summary>
    [CsProperty("nr")]
    public int Num { get; private set; }

    /// <summary>
    /// Gets the type of function.
    /// Defines the behavior and characteristics of the function (e.g., momentary, toggle, dimmer).
    /// </summary>
    [CsProperty("typ")]
    public int Type { get; private set; }

    /// <summary>
    /// Gets the current value or state of the function.
    /// For toggle functions: 0 = off, 1 = on. For dimmer functions: 0-255 for brightness level.
    /// </summary>
    [CsProperty("wert")]
    public int Value { get; private set; }
}
