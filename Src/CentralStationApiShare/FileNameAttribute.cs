namespace CentralStationApi;

/// <summary>
/// Specifies the filename associated with an enumeration member.
/// Used to map enum values to their corresponding icon or image files in the Central Station system.
/// </summary>
/// <remarks>
/// This attribute is typically applied to <see cref="ArticleType"/> enum members to define
/// the SVG icon filename for each article type (e.g., "magicon_a_005_01.svg" for turnouts).
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public class FileNameAttribute(string fileName) : Attribute
{
    /// <summary>
    /// Gets the filename associated with the enumeration member.
    /// </summary>
    /// <value>
    /// The filename, typically an SVG icon file used for visual representation in the Central Station UI.
    /// </value>
    public string FileName => fileName;
}
