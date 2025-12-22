namespace CentralStationWebApi;

public class FileReceivedEventArgs : EventArgs
{
    internal FileReceivedEventArgs(CSFileStream scFileStream)
    {
        CSFileStream = scFileStream;
    }

    public CSFileStream CSFileStream { get; }

    

}
