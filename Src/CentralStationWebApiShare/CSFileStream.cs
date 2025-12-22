namespace CentralStationWebApi;

public class CSFileStream
{
    public CSFileStream(CSFileStreamMode mode, uint length, ushort cRC, byte reserved = 0)
    {
        Mode = mode;
        Length = length;
        CRC = cRC;
        Reserved = reserved;
    }

    private MemoryStream mem = new(4 * 1024);

    public CSFileStreamMode Mode { get; }

    public uint Length { get; }

    public ushort CRC { get; }

    public byte Reserved { get; }

    public bool AddData(byte[] chunk)
    {
        mem.Write(chunk, 0, chunk.Length);
        return mem.Length >= Length;
    }

    public Stream GetFileStream()
    {
        if (mem.Length < Length)
        {
            throw new InvalidOperationException("Not enough data in stream");
        }

        // TODO check CRC

        try
        {
            
            //WriteLog("rawdata.zlib", mem);

            // remove first 4 bytes
            var inputStream = new MemoryStream(mem.ToArray(), 4, (int)Length - 4);
            //WriteLog("input.zlib", inputStream);

            var zLibStream = new ZLibStream(inputStream, CompressionMode.Decompress);

            var outputStream = new MemoryStream();
            zLibStream.CopyTo(outputStream);

            //WriteLog("output.zlib", outputStream);

            //string text = Encoding.UTF8.GetString(outputStream.GetBuffer());
            return outputStream;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public string GetFileText()
    {
        return Encoding.UTF8.GetString(((MemoryStream)GetFileStream()).GetBuffer());
        //using var stream = GetFileStream();
        //using var reader = new StreamReader(stream, System.Text.Encoding.ASCII);
        //return reader.ReadToEnd();
    }

    private void WriteLog(string fileName, MemoryStream stream)
    {
        using (var file = File.Create(fileName))
        {
            file.Write(stream.ToArray());
        }
        stream.Seek(0, SeekOrigin.Begin);
    }
}


