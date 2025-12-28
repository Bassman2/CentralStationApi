namespace CentralStationWebApi;

public class MessageBuffer
{
    private readonly byte[] buffer = new byte[13];

    public byte[] Buffer => buffer; 

    #region Header

    public void SetHeader(Priority priority, Command command, uint hash, byte dataLength)
    {
        uint header = 0;
        SetBits(ref header, 25, 4, (uint)priority);
        SetBits(ref header, 17, 8, (uint)command);
        SetBits(ref header, 16, 1, 0u);
        SetBits(ref header, 0, 16, hash);

        //var msgId = BitConverter.GetBytes(messageId);
        //Array.Reverse(msgId);
        //Array.Copy(msgId, 0, buffer, 0, 4);

        SetData(header, 0);
        buffer[4] = (byte)dataLength;
    }

    public Priority Priority => (Priority)GetBits(GetDataUInt(0), 25, 4);

    public Command Command => (Command)GetBits(GetDataUInt(0), 17, 8);

    public bool IsResponse => GetBits(GetDataUInt(0), 16, 1) == 1;

    public ushort Hash => (ushort)GetBits(GetDataUInt(0), 0, 16);

    public string Binary => $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} " + string.Join(" ", buffer[5..(5 + Math.Min(buffer[4], (byte)8))].Select(b => b.ToString("X2")));


    #endregion

    #region Data

    public int DataLength
    {
        get => buffer[4];
        set => buffer[4] = (byte)value;
    }

    public void SetData(byte value, int index) => buffer[index] = value;

    public void SetData(ushort value, int index)
    {
        byte[] data = new byte[2];
        Array.Copy(BitConverter.GetBytes(value), 0, data, 0, 2);
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, index, 2);
    }

    public void SetData(uint value, int index)
    {
        byte[] mem = new byte[4];
        Array.Copy(BitConverter.GetBytes(value), 0, mem, 0, 4);
        Array.Reverse(mem);
        Array.Copy(mem, 0, buffer, index, 4);
    }

    public void SetData(string value, int index = 5, int length = 8)
    {
        byte[] mem = Encoding.ASCII.GetBytes(value);
        Array.Clear(buffer, index, length);
        Array.Copy(mem, 0, buffer, index, Math.Min(length, mem.Length));
    }

    public void SetData(SystemCommand systemCommand, int index = 9)
    {
        buffer[index] = (byte)systemCommand;
    }

    

    public byte GetDataByte(int index) => buffer[index];

    public ushort GetDataUShort(int index)
    {
        byte[] mem = new byte[2];
        Array.Copy(buffer, index, mem, 0, 2);
        Array.Reverse(mem);
        return BitConverter.ToUInt16(mem, 0);
    }

    public uint GetDataUInt(int index)
    {
        byte[] mem = new byte[4];
        Array.Copy(buffer, index, mem, 0, 4);
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    public string GetDataString(int index = 5, int length = 8)
    {
        return Encoding.ASCII.GetString(buffer, index, length);
    }

    public byte[] GetData(int index = 5, int length = 8)
    {
        byte[] mem = new byte[length];
        Array.Copy(buffer, index, mem, 0, length);
        return mem;
    }

    #endregion


    #region Private

    protected static void SetBits(ref uint value, int position, int length, uint bits)
    {
        uint mask = ((1u << length) - 1u) << position;
        value = (value & ~mask) | ((bits << position) & mask);
    }

    protected static uint GetBits(uint value, int position, int length)
    {
        uint mask = (1u << length) - 1u;
        return (value >> position) & mask;
    }

    #endregion

}
