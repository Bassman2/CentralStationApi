namespace CentralStationDemo.ViewModel;

public class MessageViewModel
{
    public MessageViewModel(CANMessage msg)
    {
        Timestamp = msg.Timestamp.ToString("HH:mm:ss.ffff");
        //IPHostEntry entry = Dns.GetHostEntry(msg.Sender);
        //Sender = entry != null ? entry.HostName : msg.Sender;
        Sender = msg.Sender;
        Binary = msg.Binary; 
        Priority = msg.Priority.ToString();
        Command = msg.Command.ToString();
        IsResponse = msg.IsResponse;    
        Hash = msg.Hash.ToString("X4");
        Description = msg.Description;        
    }
    public string Timestamp { get; }
    public string Sender { get; }
    public string Binary { get; }
    public string Priority { get; }
    public string Command { get; }
    public bool IsResponse { get; }
    public string Hash { get; }
    public string Description { get; }
}
