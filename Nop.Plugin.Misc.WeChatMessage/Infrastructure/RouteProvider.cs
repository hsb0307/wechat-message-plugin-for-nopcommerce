using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.WeChatMessage.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: WeChatMessageDefaults.ConfigurationRouteName,
            pattern: "Admin/WeChatMessage/Configure",
            defaults: new { controller = "WeChatMessageAdmin", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: WeChatMessageDefaults.WebhookRouteName,
            pattern: "weChatMessage/webhook",
            defaults: new { controller = "WeChatMessageWebhook", action = "Webhook" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}