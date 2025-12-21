namespace CentralStationWebApi;

public class FileReceivedEventArgs : EventArgs
{
    internal FileReceivedEventArgs(CSFile scFile)
    {
        FileName = scFile.FileName;
        FileText = scFile.FileText;
    }

    public string FileName { get; }

    public string FileText { get; }

}
