namespace CentralStationWebApi;

/// <summary>
/// Specifies the mode for Central Station file streaming operations.
/// Determines whether file data is requested explicitly or broadcast to all listeners.
/// </summary>
/// <remarks>
/// This mode is used when streaming configuration files from the Central Station.
/// Request mode is for point-to-point communication, while Broadcast mode allows
/// multiple clients to receive the same file data simultaneously.
/// </remarks>
public enum CSFileStreamMode
{
    /// <summary>
    /// Request mode: File data is sent in response to a specific request.
    /// The file stream is directed to the requesting client only.
    /// This is the standard mode for individual file downloads.
    /// </summary>
    Request,

    /// <summary>
    /// Broadcast mode: File data is broadcast to all connected clients.
    /// All clients on the network receive the file stream data.
    /// Useful for synchronizing configuration across multiple control stations or applications.
    /// </summary>
    Broadcast
}
