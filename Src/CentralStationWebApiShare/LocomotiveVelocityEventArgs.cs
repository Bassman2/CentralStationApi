namespace CentralStationWebApi;

public class LocomotiveVelocityEventArgs(uint locomotiveId, ushort velocity) : EventArgs
{
    public uint LocomotiveId => locomotiveId;
    public ushort Velocity => velocity;
}
