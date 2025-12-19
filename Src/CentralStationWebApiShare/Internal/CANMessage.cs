namespace CentralStationWebApi.Internal;

internal class CANMessage
{
    private byte[] buffer = new byte[13];

    public CANMessage(Priority priority, Command command, byte[] data)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, 8, nameof(data.Length));
        
        buffer[0] = (byte)priority;                     // Prio
        buffer[1] = (byte)command;                      // Command
        buffer[2] = 0x47;                               // Resp.
        buffer[3] = 0x11;                               // Hash
        buffer[4] = (byte)data.Length;                  // DLC
        Array.Copy(data, 0, buffer, 5, data.Length);    // Byte 0 - 7
    }

    public override string ToString()
    {
        return $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} {buffer[5]:X2} {buffer[6]:X2} {buffer[7]:X2} {buffer[8]:X2} {buffer[9]:X2} {buffer[10]:X2} {buffer[11]:X2} {buffer[12]:X2}";
    }
       

    public byte[] Data => buffer;
}
