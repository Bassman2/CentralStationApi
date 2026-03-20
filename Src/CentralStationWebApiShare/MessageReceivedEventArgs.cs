namespace CentralStationWebApi;

/// <summary>
/// Provides data for CAN message received events from the Central Station.
/// Contains the complete CAN message that was received from the network.
/// </summary>
/// <remarks>
/// This event is raised for every CAN message received from the Central Station,
/// including responses to commands, status updates, and broadcast messages.
/// Subscribe to the <see cref="CentralStation.MessageReceived"/> event to monitor
/// all CAN bus traffic for debugging, logging, or custom message handling.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class
/// with the specified CAN message.
/// </remarks>
/// <param name="message">The <see cref="CanMessage"/> that was received.</param>
public sealed class MessageReceivedEventArgs(CanMessage message) : EventArgs
{
    /// <summary>
    /// Gets the received CAN message.
    /// </summary>
    /// <value>
    /// The complete <see cref="CanMessage"/> containing the header, command, data payload,
    /// and all other message information received from the Central Station.
    /// </value>
    public CanMessage Message { get; } = message;
}
