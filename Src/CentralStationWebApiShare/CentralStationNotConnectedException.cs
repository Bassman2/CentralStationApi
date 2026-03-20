using System.Diagnostics.CodeAnalysis;

namespace CentralStationWebApi;

/// <summary>
/// The exception that is thrown when an operation is attempted on a Central Station that is not connected.
/// </summary>
public class CentralStationNotConnectedException() : Exception("CentralStation not connected!")
{
    /// <summary>
    /// Throws a <see cref="CentralStationNotConnectedException"/> if the protocol handler is null or not connected.
    /// </summary>
    /// <param name="protocolHandler">The protocol handler to check for connection status.</param>
    /// <exception cref="CentralStationNotConnectedException">
    /// Thrown when <paramref name="protocolHandler"/> is null or when the protocol handler is not connected.
    /// </exception>
    internal static void ThrowIfNotConnected([NotNull] IProtocolHandler? protocolHandler)
    {
        if (protocolHandler is null || !protocolHandler.IsConnected)
        {
            throw new CentralStationNotConnectedException();
        }
    }
}
