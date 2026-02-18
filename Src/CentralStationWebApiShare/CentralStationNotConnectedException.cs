namespace CentralStationWebApi;

public class CentralStationNotConnectedException() : Exception("CentralStation not connected!")
{
    internal static void ThrowIfNotConnected(IProtocolHandler? protocolHandler)
    {
        if (protocolHandler is null || !protocolHandler.IsConnected)
        {
            throw new CentralStationNotConnectedException();
        }
    }
}
