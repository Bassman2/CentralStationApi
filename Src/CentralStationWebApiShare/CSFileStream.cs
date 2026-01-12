namespace CentralStationWebApi;

public class CSFileStream(CSFileStreamMode mode, string fileKey, uint length, ushort cRC, byte reserved = 0)
{
    private readonly MemoryStream mem = new(4 * 1024);

    public CSFileStreamMode Mode => mode;

    public string FileKey => fileKey;
    public string FileName => fileKey switch
    {
        "loks" => "lokomotive.cs2",
        "mags" => "magnetartikel.cs2",
        "gbs" => "gleisbild.cs2",
        "fs" => "fahrstrassen.cs2",
        "lokstat" => "lokomotive.sr2",
        "magstat" => "magnetartikel.sr2",
        "gbsstat" => "gleisbild.sr2",
        "fsstat" => "fahrstrassen.sr2",
        _ => fileKey.StartsWith("gbs-") ? $"gleisbild-{int.Parse(fileKey.Split('-')[1])}.cs2" : throw new InvalidDataException($"Unknown file key {fileKey}")
    };

    public uint Length => length;

    public ushort CRC => cRC;

    public byte Reserved => reserved;

    public void AddData(byte[] chunk) => mem.Write(chunk, 0, chunk.Length);
    
    public bool IsReady() => mem.Length >= Length;

    private bool IsCompressed()
    {
        mem.Position = 0;
        var buffer = new byte[6];
        mem.Read(buffer, 0, 6);
        mem.Position = 0;
        return buffer[4] == 0x78;
    }

    public Stream GetFileStream()
    {
        if (mem.Length < Length)
        {
            throw new InvalidOperationException("Not enough data in CSFileStream");
        }

        // TODO check CRC

        try
        {

            //WriteLog("rawdata.zlib", mem);

            //mem.Position = 0;

            if (IsCompressed())
            {

                // remove first 4 bytes
                //var inputStream = new MemoryStream(mem.ToArray(), 4, (int)Length - 4);


                //var inputStream = mem;
                mem.Position = 4;
                //WriteLog("input.zlib", inputStream);

                var zLibStream = new ZLibStream(mem, CompressionMode.Decompress);

                var outputStream = new MemoryStream();
                zLibStream.CopyTo(outputStream);

                //WriteLog("output.zlib", outputStream);

                //string text = Encoding.UTF8.GetString(outputStream.GetBuffer());
                return outputStream;
            }
            else
            {
                // not compressed
                return new MemoryStream(mem.ToArray(), 4, (int)Length - 4);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public string GetFileText()
    {
        return Encoding.UTF8.GetString(((MemoryStream)GetFileStream()).GetBuffer());
        //using var mem = GetFileStream();
        //using var reader = new StreamReader(mem, System.Text.Encoding.ASCII);
        //return reader.ReadToEnd();
    }

    private static void WriteLog(string fileName, MemoryStream stream)
    {
        using (var file = File.Create(fileName))
        {
            file.Write(stream.ToArray());
        }
        stream.Seek(0, SeekOrigin.Begin);
    }
}


