namespace CentralStationWebApi.Model;

/// <summary>
/// Represents version information in Central Station configuration files.
/// Provides major and minor version numbers for tracking configuration format versions.
/// </summary>
/// <remarks>
/// Version information is stored in CS2/CS3 configuration files to ensure compatibility
/// between different Central Station software versions. This class can be implicitly
/// converted to <see cref="System.Version"/> for compatibility with .NET version handling.
/// </remarks>
[CsSerialize]
public partial class CsVersion 
{
    /// <summary>
    /// Gets the major version number.
    /// </summary>
    /// <value>
    /// The major version component. Typically incremented for major format changes
    /// or incompatible updates. Default is 0.
    /// </value>
    [CsProperty("major")]
    public int Major { get; private set; } = 0;

    /// <summary>
    /// Gets the minor version number.
    /// </summary>
    /// <value>
    /// The minor version component. Typically incremented for backward-compatible
    /// enhancements or additions. Default is 0.
    /// </value>
    [CsProperty("minor")]
    public int Minor { get; private set; } = 0;

    /// <summary>
    /// Implicitly converts a <see cref="CsVersion"/> to a <see cref="System.Version"/> object.
    /// </summary>
    /// <param name="ver">The <see cref="CsVersion"/> to convert.</param>
    /// <returns>A <see cref="System.Version"/> object with the major and minor version components.</returns>
    /// <remarks>
    /// This implicit conversion allows <see cref="CsVersion"/> to be used seamlessly
    /// with .NET APIs that expect <see cref="System.Version"/> objects.
    /// </remarks>
    public static implicit operator Version(CsVersion ver) => new Version(ver.Major, ver.Minor);
}

