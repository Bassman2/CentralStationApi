namespace CentralStationWebApi;

public class FileReceivedEventArgs(string fileName, Stream stream) : EventArgs
{
    public string FileName => fileName;
    public Stream Stream => stream;
}
