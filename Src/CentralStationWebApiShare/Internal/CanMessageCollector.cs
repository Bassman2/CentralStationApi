namespace CentralStationWebApi.Internal;

internal class CanMessageCollector
{
    protected readonly MemoryStream mem = new(4 * 1024);


    //public long Position
    //{         
    //    get => mem.Position;
    //    set => mem.Position = value;
    //}

    public void SetPositionToStart()
    {
        mem.Position = 0;
    }

    public void AddData(byte[] data, int offset = 0, int count = 8)
    {
        mem.Write(data, offset, count);
    }

    public sbyte ReadSByte()
    {
        int value = mem.ReadByte();
        if (value == -1)
        {
            throw new EndOfStreamException("No more data available to read a byte.");
        }
        return (sbyte)value;
    }

    public byte ReadByte()
    {
        int value = mem.ReadByte();
        if (value == -1)
        {
            throw new EndOfStreamException("No more data available to read a byte.");
        }
        return (byte)value;
    }

    public ushort ReadUInt16()
    {
        int highByte = mem.ReadByte();
        int lowByte = mem.ReadByte();
        if (lowByte == -1 || highByte == -1)
        {
            throw new EndOfStreamException("No more data available to read a UInt16.");
        }
        return (ushort)(lowByte | (highByte << 8));
    }

    public uint ReadUInt32()
    {
        int byte1 = mem.ReadByte();
        int byte2 = mem.ReadByte();
        int byte3 = mem.ReadByte();
        int byte4 = mem.ReadByte();
        if (byte1 == -1 || byte2 == -1 || byte3 == -1 || byte4 == -1)
        {
            throw new EndOfStreamException("No more data available to read a UInt32.");
        }
        return (uint)(byte4 | (byte3 << 8) | (byte2 << 16) | (byte1 << 24));
    }

    public string ReadString()
    {
        StringBuilder sb = new();
        int value;
        while ((value = mem.ReadByte()) > 0)
        {
            sb.Append((char)value);
        }
        return sb.ToString();
    }

    public string ReadString(int length)
    {
        byte[] mem = new byte[length];
        this.mem.Read(mem, 0, length);
        return Encoding.ASCII.GetString(mem, 0, length).Trim((char)0);
    }

    public int Package { get; set; } = 1;


    //////////////////////////////////////////////////////////////////
    // File Collector

    //public CSFileStreamMode Mode => mode;

    //public string FileName => fileName;

    public uint Length { get; set; }

    public ushort Crc { get; set; }

    //public byte Reserved => reserved;

    public bool IsReady() => mem.Length >= Length;

    private bool IsCompressed()
    {
        mem.Position = 4;
        byte res = ReadByte();
        mem.Position = 0;
        return res == 0x78;
    }

    public MemoryStream GetStream()
    {
        MemoryStream memory = new MemoryStream();
        CopyTo(memory);
        memory.Position = 0;
        return memory;
    }

    public void CopyTo(Stream stream)
    {
        if (mem.Length < Length)
        {
            throw new InvalidOperationException("Not enough data in FileCollector");
        }

        // TODO check CRC

        try
        {
            if (IsCompressed())
            {
                mem.Position = 4;
                using var zLibStream = new ZLibStream(mem, CompressionMode.Decompress);
                zLibStream.CopyTo(stream);
            }
            else
            {
                mem.Position = 0;
                mem.CopyTo(stream);
            }

            stream.Position = 0;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }
}
