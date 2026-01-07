namespace CentralStationWebApi;

public enum DeviceType : ushort
{
    /// <summary>
    /// Track Format Processor / Gleis Format Prozessor
    /// 60213 Central Station 2008 - 2009
    /// 60214 Central Station 2009 - 2011  
    /// 60173 Booster 2008 - 2010 for H0 and 1
    /// 60174 Booster 2010 - 2016
    /// </summary>
    GFP = 0x0000,

    /// <summary>
    /// Digital Connector Box / Gleisbox 
    /// 60112 for 1 Gauge 
    /// 60113 for Märklin H0, Trix H0 and Minitrix
    /// </summary>
    DCB = 0x0010,
    DCB1 = 0x0011,

    /// <summary>
    /// Connect 6021
    /// 60128
    /// </summary>
    Connect = 0x0020,

    /// <summary>
    /// Mobile Station 2
    /// 60653 SW-Version ?.? (from documentation)       
    /// </summary>
    MS2 = 0x0030,

    /// <summary>
    /// Mobil Station 2
    /// 60653 SW-Version
    /// </summary>
    MS2_1 = 0x0031,

    /// <summary>
    /// Mobil Station 2
    /// 60653 SW-Version 
    /// </summary>
    MS2_2 = 0x0032,

    /// <summary>
    /// Mobil Station 2
    /// 60653 SW-Version 5.6  (from original device)
    /// </summary>
    MS2_3 = 0x0033,

    /// <summary>
    /// Mobil Station 2
    /// 60653 SW-Version 
    /// </summary>
    MS2_4 = 0x0034,

    /// <summary>
    /// S88 Link
    /// SRSEII (non Märklin produkt)
    /// </summary>
    LinkS88 = 0x0040,

    /// <summary>
    /// GFP3 from CS3
    /// 60226 Central Station 3       (from original device) 
    /// 60216 Central Station 3 plus
    /// </summary>
    GFP3 = 0x0050,

    /// <summary>
    /// Central Station 2
    /// Software App
    /// </summary>
    CS2 = 0xeeee,

    /// <summary>
    /// Wireless Controllers
    /// </summary>
    Wireless = 0xffe0,

    /// <summary>
    /// CS2 Slave
    /// Wired Controller
    /// </summary>
    Wired = 0xfff0,

    /// <summary>
    /// Central Station Master GUI
    /// ???
    /// xxxxx GUI Central Station 2
    /// 60226 GUI Central Station 3         (from original device)
    /// 60216 GUI Central Station 3 plus
    /// </summary>
    GUI = 0xffff        
}
