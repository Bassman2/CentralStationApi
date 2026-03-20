namespace CentralStationWebApi.Model;

/// <summary>
/// Represents an article (device or accessory) in the Central Station system.
/// </summary>
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
    public ArticleType ArticleType { get; private set; }

    /// <summary>
    /// Gets the human-readable name of the article type.
    /// </summary>
    public string ArticleTypeName => ArticleType.GetDescription();

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

    /// <summary>
    /// Gets the decoder protocol type (DCC or MM2).
    /// </summary>
    [CsProperty("decoder")]
    public string? Decoder { get; private set; }

    /// <summary>
    /// Gets the decoder identifier or configuration string.
    /// </summary>
    public Uri? IconUri => ArticleType.GetFileNamePath(CentralStation.MagUri);
  }
