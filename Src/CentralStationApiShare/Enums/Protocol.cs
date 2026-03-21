namespace CentralStationApi;

/// <summary>
/// Specifies the network protocol used for communication with the Märklin Central Station.
/// </summary>
/// <remarks>
/// The Central Station supports TCP, UDP, and direct CAN protocols for communication.
/// TCP is recommended for reliable, connection-oriented communication, while UDP can be used
/// for scenarios requiring lower latency or broadcast capabilities. CAN provides direct
/// access to the CAN bus for hardware-level communication.
/// </remarks>
public enum Protocol
{
    /// <summary>
    /// Transmission Control Protocol (TCP).
    /// Provides reliable, ordered, and error-checked delivery of CAN messages over the network.
    /// This is the default and recommended protocol for most applications.
    /// </summary>
    TCP,

    /// <summary>
    /// User Datagram Protocol (UDP).
    /// Provides connectionless, lightweight message delivery with lower latency.
    /// Suitable for time-sensitive operations where occasional packet loss is acceptable.
    /// </summary>
    UDP,

    /// <summary>
    /// Controller Area Network (CAN) protocol.
    /// Provides direct hardware-level access to the CAN bus.
    /// Requires physical CAN bus interface hardware for direct communication with devices.
    /// </summary>
    /// <remarks>
    /// !!! For later implementation !!!
    /// The CAN protocol is typically used for low-level hardware communication and requires
    /// specialized interface hardware to connect to the CAN bus. It is not commonly used
    /// for standard network communication scenarios.
    /// </remarks>
    CAN
}
