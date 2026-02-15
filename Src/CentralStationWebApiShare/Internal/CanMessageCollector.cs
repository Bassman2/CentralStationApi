namespace CentralStationWebApi.Internal;

internal abstract class CanMessageCollector
{
    protected readonly MemoryStream mem = new(4 * 1024);

    //public CanMessageCollector(CanMessage req)
    //{
    //    if (req.Command != Command.StatusData && req.Command != Command.ConfigDataRequest)
    //    {
    //        throw new ArgumentException($"Command {req.Command} not supported for CanMessageCollector");
    //    }


    //    if (req.Command == Command.StatusData && !req.IsResponse)
    //    {
    //        DeviceId = req.DeviceId;
    //        Index = req.GetDataByte(4);
    //    }
    //    else if (req.Command == Command.ConfigDataRequest && !req.IsResponse)
    //    {
    //        FileName = req.GetDataString();
    //    }
    //}

    public abstract bool AddMessage(CanMessage msg);

    public void SetPositionToStart()
    {
        mem.Position = 0;
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

    //public bool AddMessage(CanMessage msg)
    //{
    //    if (msg.Command == Command.StatusData)
    //    {
    //        switch (msg.DataLength)
    //        {
    //        case 5:
    //            break;
    //        case 6:
    //            // must be always first message; break if not
    //            if (mem.Length > 0)
    //            {
                    
    //            }
    //            DeviceId = msg.GetDataUInt(0);
    //            Index = msg.GetDataByte(4);
    //            PackageNumber = msg.GetDataByte(5);
    //            mem.Position = 0;
    //            return true;    // is ready
    //        case 8:
    //            mem.Write(msg.GetData(), 0, 8);
    //            break;
    //        default:
    //            throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");

    //        }
    //    }
    //    else if (msg.Command == Command.ConfigDataStream)
    //    {
    //        switch (msg.DataLength)
    //        {
    //        case 6:
    //        case 7:
    //            Length = msg.GetDataUInt(0);
    //            Crc = msg.GetDataUShort(4);
    //            break;
    //        case 8:
    //            mem.Write(msg.GetData(), 0, 8);
    //            if (mem.Length >= Length)
    //            {
    //                return true;    // is ready
    //            }
    //            break;
    //        }
    //    }
    //    return false;
    //}

    /////////////////////////////////////////////////////////////////
    // StatusData

    //public uint DeviceId { get; private set; }
    //public uint Index { get; private set; }
    //public uint PackageNumber { get; private set; }

    //////////////////////////////////////////////////////////////////
    // ConfigDataStream

    //public string FileName { get; private set; } = string.Empty;
    //public uint Length { get; private set; }
    //public ushort Crc { get; private set; }

    /////////////////////////////////////////////////////
    //// get stream
    
    //private bool IsCompressed()
    //{
    //    mem.Position = 4;
    //    byte res = ReadByte();
    //    mem.Position = 0;
    //    return res == 0x78;
    //}

    //public MemoryStream GetStream()
    //{
    //    MemoryStream memory = new MemoryStream();
    //    CopyTo(memory);
    //    memory.Position = 0;
    //    return memory;
    //}

    //private void CopyTo(Stream stream)
    //{
    //    if (mem.Length < Length)
    //    {
    //        throw new InvalidOperationException("Not enough data in FileCollector");
    //    }

    //    // TODO check CRC

    //    try
    //    {
    //        if (IsCompressed())
    //        {
    //            mem.Position = 4;
    //            using var zLibStream = new ZLibStream(mem, CompressionMode.Decompress);
    //            zLibStream.CopyTo(stream);
    //        }
    //        else
    //        {
    //            mem.Position = 0;
    //            mem.CopyTo(stream);
    //        }

    //        stream.Position = 0;

    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        throw;
    //    }
    //}
}
