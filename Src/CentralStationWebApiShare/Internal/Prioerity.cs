namespace CentralStationWebApi.Internal;

internal enum Priority : byte
{
    Proirity1 = 0,      // Prio 1: Stopp / Go / Kurzschluss-Meldung
    Proirity2 = 1,      // Rückmeldungen
    Proirity3 = 2,      // Lok anhalten (?)
    Proirity4 = 3,      // Lok / Zubehörbefehle
    Free = 4,           // Rest Frei
}
