namespace CentralStationWebApi;

public class FileReceivedEventArgs(string fileName, string file) : EventArgs
{
    
    public string FileName => fileName;

    public string File => file;

}
