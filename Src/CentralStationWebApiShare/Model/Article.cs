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

            //var memberInfo = typeof(ArticleType).GetMember(Type.ToString()).FirstOrDefault();
            //if (memberInfo != null)
            //{
            //    var attr = memberInfo.GetCustomAttributes(typeof(FileNameAttribute), false)
            //                         .OfType<FileNameAttribute>()
            //                         .FirstOrDefault();
            //    if (attr != null)
            //    {
            //        return new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/{attr.FileName}");
            //    }
            //}
            //return new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/magicon_a_000_00.svg"); 
            string? fileName = FileNameAttribute.GetFilename(Type);
            return fileName is null ? null : new Uri($"http://{CentralStationBasic.Host}/app/assets/mag/{fileName}");


        }
    }


}
