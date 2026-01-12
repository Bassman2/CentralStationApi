namespace CentralStationWebApi;

public class FileReceivedEventArgs(string fileKey, string fileName, Stream stream) : EventArgs
{
    public string FileKey => fileKey;
    public string FileName => fileName;
    public Stream Stream => stream;
}
