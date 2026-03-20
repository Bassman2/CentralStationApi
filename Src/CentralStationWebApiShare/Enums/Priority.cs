namespace CentralStationWebApi;

/// <summary>
/// Specifies the priority level for CAN messages sent to the Märklin Central Station.
/// Higher priority messages (lower numeric values) are processed before lower priority messages.
/// </summary>
/// <remarks>
/// Priority levels 0-3 are defined for specific message types, while levels 4-15 are reserved
/// for future use. The priority determines message processing order in the CAN bus queue.
/// In CAN bus terminology, lower numeric values have higher priority on the bus.
/// </remarks>
public enum Priority : byte
{
    /// <summary>
    /// Highest priority (Priority 1): System-critical commands.
    /// Used for Stop, Go, and short-circuit/overload notifications (Kurzschluss-Meldung).
    /// </summary>
    Prio1 = 0,

    /// <summary>
    /// High priority (Priority 2): Feedback and status messages.
    /// Used for feedback responses (Rückmeldungen) from S88 modules and other sensors.
    /// </summary>
    Prio2 = 1,

    /// <summary>
    /// Medium priority (Priority 3): Locomotive emergency commands.
    /// Possibly used for emergency locomotive stop commands (Lok anhalten).
    /// </summary>
    Prio3 = 2,

    /// <summary>
    /// Normal priority (Priority 4): Standard locomotive and accessory commands.
    /// Used for regular locomotive control and accessory switching (Lok / Zubehörbefehle).
    /// </summary>
    Prio4 = 3,

    /// <summary>
    /// Free priority level 4
    /// </summary>
    Free4 = 4,

    /// <summary>
    /// Free priority level 5   
    /// </summary>
    Free5 = 5,

    /// <summary>
    /// Free priority level 6
    /// </summary>
    Free6 = 6,

    /// <summary>
    /// Free priority level 7
    /// </summary>
    Free7 = 7,

    /// <summary>
    /// Free priority level 8
    /// </summary>
    Free8 = 8,

    /// <summary>
    /// Free priority level 9
    /// </summary>
    Free9 = 9,

    /// <summary>
    /// Free priority level 10
    /// </summary>
    Free10 = 10,

    /// <summary>
    /// Free priority level 11
    /// </summary>
    Free11 = 11,

    /// <summary>
    /// Free priority level 12
    /// </summary>
    Free12 = 12,

    /// <summary>
    /// Free priority level 13
    /// </summary>
    Free13 = 13,

    /// <summary>
    /// Free priority level 14
    /// </summary>
    Free14 = 14,

    /// <summary>
    /// Free priority level 15
    /// </summary>
    Free16 = 15
}
