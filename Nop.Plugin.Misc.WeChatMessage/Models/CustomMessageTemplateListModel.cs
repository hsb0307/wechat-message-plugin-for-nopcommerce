using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.WeChatMessage.Models;

/// <summary>
/// Represents a synchronization record list model
/// </summary>
public record CustomMessageTemplateListModel : BasePagedListModel<CustomMessageTemplateModel>
{
}