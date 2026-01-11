using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationWebApi;

public class LocomotiveDirectionEventArgs(uint locomotiveId, DirectionChange direction) : EventArgs
{
    public uint LocomotiveId => locomotiveId;
    public DirectionChange Direction => direction;
}
