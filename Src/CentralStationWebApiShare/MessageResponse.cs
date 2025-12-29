namespace CentralStationWebApi;

public class MessageResponse(CANMessage req, CANMessage? res = null)
{
    public bool Success => res != null;

    public CANMessage RequestMsg => req;

    public CANMessage? ResponseMsg { get; internal set; }
}