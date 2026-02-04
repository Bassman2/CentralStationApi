namespace CentralStationWebApi;

public sealed class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(CanMessage message) => Message = message;
    public CanMessage Message { get; }
}
