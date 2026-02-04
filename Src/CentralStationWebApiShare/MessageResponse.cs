namespace CentralStationWebApi;

public class MessageResponse(CanMessage req, CanMessage? res = null)
{
    public bool Success => res != null;

    public CanMessage RequestMsg => req;

    public CanMessage? ResponseMsg { get; internal set; }
}