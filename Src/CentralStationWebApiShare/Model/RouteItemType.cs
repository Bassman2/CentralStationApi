using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationWebApi.Model;

[EnumConverter]
public enum RouteItemType
{
    [EnumMember(Value = "mag")]
    Mag,

}
