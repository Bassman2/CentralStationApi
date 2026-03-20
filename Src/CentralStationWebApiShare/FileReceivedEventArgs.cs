namespace CentralStationWebApi;

/// <summary>
/// Provides data for file received events from the Central Station.
/// Contains information about a configuration file that has been received via the CAN bus.
/// </summary>
/// <remarks>
/// Configuration files are streamed from the Central Station using the <see cref="Command.ConfigDataStream"/>
/// command in response to a <see cref="Command.ConfigDataRequest"/>. Files include locomotive databases,
/// track layouts, routes, and other configuration data in the CS2/CS3 format.
/// </remarks>
public class FileReceivedEventArgs(string fileKey, string fileName, Stream stream) : EventArgs
{
    /// <summary>
    /// Gets the file key identifier.
    /// </summary>
    /// <value>
    /// A unique key identifying the file type or category (e.g., "lokomotive", "gleisbild", "fahrstrasse").
    /// </value>
    public string FileKey => fileKey;

    /// <summary>
    /// Gets the name of the received file.
    /// </summary>
    /// <value>
    /// The filename as stored on the Central Station (e.g., "lokomotive.cs2", "gleisbild-1.cs2").
    /// </value>
    public string FileName => fileName;

    /// <summary>
    /// Gets the stream containing the file data.
    /// </summary>
    /// <value>
    /// A <see cref="Stream"/> containing the complete file content in CS2/CS3 format.
    /// This stream can be deserialized using <see cref="CsSerializer"/> to obtain strongly-typed objects.
    /// </value>
    public Stream Stream => stream;
}
