using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Article
{
    /// <summary>
    /// Address of the article
    /// </summary>
    [CsProperty("id")]
    public uint Id { get; private set; }

    /// <summary>
    /// Article name/description
    /// </summary>
    [CsProperty("name")]
    public string? Name { get; private set; }

    /// <summary>
    /// Type of article
    /// </summary>
    [CsProperty("typ")]
    public ArticleType Type { get; private set; }

    /// <summary>
    /// Current position
    /// </summary>
    [CsProperty("stellung")]
    public uint Position { get; private set; }

    /// <summary>
    /// Set switching time
    /// </summary>
    [CsProperty("schaltzeit")]
    public uint SwitchingTime { get; private set; }

    /// <summary>
    /// For articles containing a relevant term
    /// </summary>
    [CsProperty("ungerade")]
    public uint Odd { get; private set; }

    /// <summary>
    /// Decoder type, either DCC or mm2
    /// </summary>
    [CsProperty("dectyp")]
    public DecoderType DecoderType { get; private set; }

    [CsProperty("decoder")]
    public string? Decoder { get; private set; }

    public Uri? IconUri
    {
        get
        {

            var memberInfo = typeof(ArticleType).GetMember(Type.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var attr = memberInfo.GetCustomAttributes(typeof(FileNameIdAttribute), false)
                                     .OfType<FileNameIdAttribute>()
                                     .FirstOrDefault();
                if (attr != null)
                {
                    return new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/magicon_a_{attr.Id:000}_01.svg");
                }
            }
            return new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/magicon_a_000_00.svg"); 

            //string file = Type switch
            //{
            //    ArticleType.Unknown => "magicon_a_000_00",

            //    ArticleType.StdRedGreen => "magicon_a_000_01",
            //    ArticleType.StdRed => "magicon_a_001_01",
            //    ArticleType.StdGreen => "magicon_a_002_01",
            //    ArticleType.Uncoupler => "magicon_a_003_01",
            //    ArticleType.Uncoupler1 => "magicon_a_004_01",
            //    ArticleType.RightTurnout => "magicon_a_005_01",
            //    ArticleType.LeftTurnout => "magicon_a_006_01",
            //    ArticleType.YSwitch => "magicon_a_007_01",
            //    ArticleType.K84Exit => "magicon_a_008_01",
            //    ArticleType.K84DoubleExit => "magicon_a_009_01",
            //    ArticleType.ThreeWaySwitch => "magicon_a_010_01",
            //    ArticleType.DKW2Drive => "magicon_a_011_00",
            //    ArticleType.DKW1Drive => "magicon_a_012_01",


            //    // 0013
            //    // 0014
            //    ArticleType.UrcLightSignalHp01 => "magicon_a_015_01",
            //    ArticleType.LightSignalHp01 => "magicon_a_015_01",
            //    ArticleType.LightSignalHp02 => "magicon_a_016_01",
            //    ArticleType.LightSignalHp012 => "magicon_a_017_01",
            //    ArticleType.UrcLightSignalHp012 => "magicon_a_017_01",

            //    ArticleType.LightSignalHp012Sh01 => "magicon_a_018_01",

            //    ArticleType.LightSignalSh01 => "magicon_a_019_01",
            //    ArticleType.UrcLightSignalSh01 => "magicon_a_019_01",

            //    ArticleType.SemaphoreSignalHp01 => "magicon_a_020_01",
            //    ArticleType.SemaphoreSignalHp02 => "magicon_a_021_01",
            //    ArticleType.SemaphoreSignalHp012 => "magicon_a_022_01",
            //    ArticleType.SemaphoreSignalHp012Sh01 => "magicon_a_023_01",
            //    ArticleType.SemaphoreSignalSh01 => "magicon_a_024_01",  // ??

            //    ArticleType.UrcLightSignalHp012Sh01 => "magicon_a_027_01",

            //    ArticleType.Slideway => "magicon_a_000_00",
            //    ArticleType.TurntableOld => "magicon_a_000_00",
            //    ArticleType.TurntableDigital => "magicon_a_000_00",
            //    ArticleType.TurntableMfx => "magicon_a_000_00",

            //    ArticleType.LightStreetLamp => "magicon_a_033_01",
            //    ArticleType.LightStreetNeon => "magicon_a_034_01",
            //    ArticleType.LightLamp => "magicon_a_036_01",
            //    ArticleType.LightNeon => "magicon_a_037_01",

            //    ArticleType.MonitoringSignal => "magicon_a_040_00",
            //    ArticleType.SoundOperation => "magicon_a_048_01",
            //    ArticleType.SoundWarning => "magicon_a_049_01",
            //    ArticleType.SoundMisc => "magicon_a_050_01",

            //    ArticleType.LightOutdoor => "magicon_a_062_01",

            //    ArticleType.DepartureSignal => "magicon_a_063_01",

            //    ArticleType.DoubleCrossing => "magicon_a_070_01",
            //    _ => throw new NotImplementedException()
            //};
            //return new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/{file}.svg");
        }
    }


}
