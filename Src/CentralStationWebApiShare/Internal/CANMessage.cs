using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CentralStationWebApi.Internal;

/*
 


00004711    system send
00014711    system receive
00024711    discovery send 
00044711    bind send
00054711    bind send
00064711    verify send
00074711    verify receive
00084711    loc speed send

00000000111111112222222233333333 
76543210765432107654321076543210     
PPPPxxxCCCCCCCCRHHHHHHHHHHHHHHHH
*/
internal class CANMessage
{
    private byte[] buffer = new byte[13];

    public CANMessage(Priority priority, Command command, uint hash, byte[] data)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, 8, nameof(data.Length));

        SetHeader(priority, command, hash);
        //uint messageId =
        //    ((((uint)priority) & 0x000f) << 25) +       // Prio
        //    ((((uint)command) & 0x00ff) << 17) +        // Command
        //    ((((uint)0) & 0x0001) << 16) +              // Resp.
        //    ((((uint)hash) & 0xffff));            // Command
        //var msgId = BitConverter.GetBytes(messageId);
        //Array.Reverse(msgId);
        //Array.Copy(msgId, 0, buffer, 0, 4);
        //buffer[2] = 0x47;                               // Resp.
        //buffer[3] = 0x11;                               // Hash
        //buffer[4] = (byte)data.Length;                  // DLC
        //Array.Copy(data, 0, buffer, 5, data.Length);    // Byte 0 - 7

        SetData(data, data.Length);
    }

    public CANMessage(byte[] data)
    {
        Array.Copy(data, buffer, 13);
    }

    public byte[] Data => buffer;

    public Priority Priority => (Priority)GetBits(GetHeader(), 25, 4);

    public Command Command => (Command)GetBits(GetHeader(), 17, 8);

    public bool IsResponse => GetBits(GetHeader(), 16, 1) == 1;

    public ushort Hash => (ushort)GetBits(GetHeader(), 0, 16);

    public override string ToString()
    {
        return $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} {buffer[5]:X2} {buffer[6]:X2} {buffer[7]:X2} {buffer[8]:X2} {buffer[9]:X2} {buffer[10]:X2} {buffer[11]:X2} {buffer[12]:X2} - " +
            $"Prio.: {Priority} Command: {Command} IsResp: {IsResponse} Hash: {Hash:X4}";
    }

    private void SetHeader(Priority priority, Command command, uint hash)
    {
        //uint messageId =
        //    ((((uint)priority) & 0x000f) << 25) +       // Prio
        //    ((((uint)command) & 0x00ff) << 17) +        // Command
        //    ((((uint)0) & 0x0001) << 16) +              // Resp.
        //    ((((uint)hash) & 0xffff));            // Command

        uint messageId = 0;
        SetBits(ref messageId, 25, 4, (uint)priority);
        SetBits(ref messageId, 17, 8, (uint)command);
        SetBits(ref messageId, 16, 1, 0u);
        SetBits(ref messageId, 0, 16, hash);
        
        var msgId = BitConverter.GetBytes(messageId);
        Array.Reverse(msgId);
        Array.Copy(msgId, 0, buffer, 0, 4);
    }

    private uint GetHeader()
    {
        var msgId = new byte[4];
        Array.Copy(buffer, 0, msgId, 0, 4);
        Array.Reverse(msgId);
        return BitConverter.ToUInt32(msgId, 0);
    }

    private void SetData(byte[] bytes, int length)
    {
        // set DLC
        buffer[4] = (byte)length;       
        // set data bytes 0 - 7
        Array.Copy(bytes, 0, buffer, 5, length);
    }

    private static void SetBits(ref uint value, int position, int length, uint bits)
    {
        uint mask = ((1u << length) - 1u) << position;
        value = (value & ~mask) | ((bits << position) & mask);
    }   

    private static uint GetBits(uint value, int position, int length)
    {
        uint mask = (1u << length) - 1u;
        return (value >> position) & mask;
    }
}
