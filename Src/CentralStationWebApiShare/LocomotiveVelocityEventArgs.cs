using CentralStationWebApi.Model;

namespace CentralStationWebApi;

public class LocomotiveVelocityEventArgs(uint locomotiveId, ushort velocity) : LocomotiveEventArgs(locomotiveId)
{
    public ushort Velocity => velocity;
}
