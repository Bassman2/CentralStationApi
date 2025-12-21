namespace CentralStationWebApi;

public class MessageBuffer
{
    private readonly byte[] buffer = [13];

    #region Header

    public void SetHeader(Priority priority, Command command, uint hash)
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
    }

    public Priority Priority => (Priority)GetBits(GetDataUint(0), 25, 4);

    public Command Command => (Command)GetBits(GetDataUint(0), 17, 8);

    public bool IsResponse => GetBits(GetDataUint(0), 16, 1) == 1;

    public ushort Hash => (ushort)GetBits(GetDataUint(0), 0, 16);


    #endregion

    #region Data

    public void SetDataLength(byte length) => buffer[4] = length;

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

    public int GetDataLength() => buffer[4];

    public byte GetDataByte(int index) => buffer[index];

    public ushort GetDataUShort(int index)
    {
        byte[] mem = new byte[2];
        Array.Copy(buffer, index, mem, 0, 2);
        Array.Reverse(mem);
        return BitConverter.ToUInt16(mem, 0);
    }

    public uint GetDataUint(int index)
    {
        byte[] mem = new byte[4];
        Array.Copy(buffer, index, mem, 0, 4);
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    #endregion


    #region Private

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

    #endregion

}
