//namespace CentralStationWebApi.Internal;

//internal class FileCollector(CSFileStreamMode mode, string fileName, uint length, ushort cRC, byte reserved = 0) : CanMessageCollector
//{
//    public CSFileStreamMode Mode => mode;

//    public string FileName => fileName;

//    public uint Length => length;

//    public ushort CRC => cRC;

//    public byte Reserved => reserved;

//    public bool IsReady() => mem.Length >= Length;

//    private bool IsCompressed()
//    {
//        mem.Position = 4;
//        byte res = ReadByte();
//        mem.Position = 0;
//        return res == 0x78;
//    }

//    public void CopyTo(Stream stream)
//    {
//        if (mem.Length < Length)
//        {
//            throw new InvalidOperationException("Not enough data in FileCollector");
//        }

//        // TODO check CRC

//        try
//        {
//            if (IsCompressed())
//            {
//                mem.Position = 4;
//                using var zLibStream = new ZLibStream(mem, CompressionMode.Decompress);
//                zLibStream.CopyTo(stream);
//            }
//            else
//            {
//                mem.Position = 0;
//                mem.CopyTo(stream);
//            }

//            stream.Position = 0;

//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine(ex);
//            throw;
//        }
//    }
//}
