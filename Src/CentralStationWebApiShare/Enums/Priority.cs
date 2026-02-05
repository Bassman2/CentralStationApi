namespace CentralStationWebApi;

public enum Priority : byte
{
    Prio1 = 0,      // Prio 1: Stopp / Go / Kurzschluss-Meldung
    Prio2 = 1,      // Rückmeldungen
    Prio3 = 2,      // Lok anhalten (?)
    Prio4 = 3,      // Lok / Zubehörbefehle
    Free4 = 4,      // Rest Frei
    Free5 = 5,
    Free6 = 6,
    Free7 = 7,
    Free8 = 8,
    Free9 = 9,
    Free10 = 10,
    Free11 = 11,
    Free12 = 12,
    Free13 = 13,
    Free14 = 14,
    Free16 = 15
}
