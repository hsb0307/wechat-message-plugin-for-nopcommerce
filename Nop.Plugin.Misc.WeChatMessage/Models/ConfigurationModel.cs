using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.WeChatMessage.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Ctor



    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Misc.WeChatMessage.Configuration.Fields.Enabled")]
    public bool Enabled { get; set; } = false;
    public bool Enabled_OverrideForStore { get; set; }


    [NopResourceDisplayName("Plugins.Misc.WeChatMessage.Configuration.Fields.AppID")]
    public string AppID { get; set; }
    public bool AppID_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.WeChatMessage.Configuration.Fields.AppSecret")]
    [NoTrim]
    // [DataType(DataType.Password)]
    public string AppSecret { get; set; }
    public bool AppSecret_OverrideForStore { get; set; }


    #endregion


}