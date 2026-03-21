namespace CentralStationApi.Internal;

internal class CanStatusCollector : CanMessageCollector
{
    public CanStatusCollector(CanMessage req)
    {
        if (req.Command != Command.StatusData && req.Command != Command.ConfigDataRequest)
        {
            throw new ArgumentException($"Command {req.Command} not supported for CanMessageCollector");
        }


        if (req.Command == Command.StatusData && !req.IsResponse)
        {
            DeviceId = req.DeviceId;
            Index = req.GetDataByte(4);
        }
    }

    public override bool AddMessage(CanMessage msg)
    {
        if (msg.Command == Command.StatusData)
        {
            switch (msg.DataLength)
            {
            case 5:
                break;
            case 6:
                // must be always first message; break if not
                if (mem.Length > 0)
                {

                }
                DeviceId = msg.GetDataUInt(0);
                Index = msg.GetDataByte(4);
                PackageNumber = msg.GetDataByte(5);
                mem.Position = 0;
                return true;    // is ready
            case 8:
                mem.Write(msg.GetData(), 0, 8);
                break;
            default:
                throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");

            }
        }
        return false;
    }

    public uint DeviceId { get; private set; }
    public uint Index { get; private set; }
    public uint PackageNumber { get; private set; }
}
