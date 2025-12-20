namespace CentralStationWebApi;

public sealed class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(CANMessage message) => Message = message;
    public CANMessage Message { get; }
}
