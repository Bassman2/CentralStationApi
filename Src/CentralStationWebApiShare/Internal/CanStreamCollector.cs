namespace CentralStationWebApi.Internal;

internal class CanStreamCollector : CanMessageCollector
{
    public CanStreamCollector(CanMessage req)
    {
        if (req.Command == Command.ConfigDataRequest && !req.IsResponse)
        {
            FileName = req.GetDataString();
        }
    }

    public override bool AddMessage(CanMessage msg)
    {
        if (msg.Command == Command.ConfigDataStream)
        {
            switch (msg.DataLength)
            {
            case 6:
            case 7:
                Length = msg.GetDataUInt(0);
                Crc = msg.GetDataUShort(4);
                break;
            case 8:
                mem.Write(msg.GetData(), 0, 8);
                if (mem.Length >= Length)
                {
                    return true;    // is ready
                }
                break;
            }
        }
        return false;
    }

    public string FileName { get; private set; } = string.Empty;
    public uint Length { get; private set; }
    public ushort Crc { get; private set; }

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

    private void CopyTo(Stream stream)
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
