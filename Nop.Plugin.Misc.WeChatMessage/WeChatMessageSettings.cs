using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.WeChatMessage;

/// <summary>
/// Represents plugin settings
/// </summary>
public class WeChatMessageSettings : ISettings
{
    public bool Enabled { get; set; } = false;

    public string AppID { get; set; }

    public string AppSecret { get; set; }

    public int? RequestTimeout { get; set; }


}