
namespace CentralStationApi.Internal;

//  CAN 2.0B
// 29-Bit-IDs 
// Baudrate 250 kBaud.

// https://www.waveshare.com/wiki/USB-CAN-B?srsltid=AfmBOoq1ePaZ7Enp9sWvnNCJVRptROrrDNTx7FSYwDnfiY9j88lU6aqt#Driver_Installation
// https://www.waveshare.com/wiki/USB-CAN-A?srsltid=AfmBOoq7ZrcF4TVP25PmkYyEvwLRpUS7c4MgSnb8M0thHWall8IavnV6#Features

internal class CanHandler : IProtocolHandler, IDisposable
{
    public bool IsConnected => throw new NotImplementedException();

    public void Connect(string host)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<CanMessage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }

    public void Send(CanMessage msg)
    {
        throw new NotImplementedException();
    }

    public Task SendAsync(CanMessage msg)
    {
        throw new NotImplementedException();
    }
}


