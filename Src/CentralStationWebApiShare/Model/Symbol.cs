namespace CentralStationWebApi.Model;

/// <summary>
/// Represents the symbol or icon type used to visually identify different locomotive propulsion types.
/// Used by the Central Station to display appropriate icons for locomotives.
/// </summary>
[EnumConverter]
public enum Symbol
{
    /// <summary>
    /// Electric locomotive symbol (for electric-powered trains).
    /// </summary>
    Electric = 0,

    /// <summary>
    /// Diesel locomotive symbol (for diesel-powered trains).
    /// </summary>
    Diesel = 1,

    /// <summary>
    /// Steam locomotive symbol (for steam-powered trains).
    /// </summary>
    Steam = 2,

    /// <summary>
    /// No icon or default symbol.
    /// </summary>
    NoIcon = 3
}
