using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;

namespace Nop.Plugin.Misc.WeChatMessage;

/// <summary>
/// Represents plugin constants
/// </summary>
public class WeChatMessageDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Misc.WeChatMessage";

    /// <summary>
    /// Gets the user agent used to request third-party services
    /// </summary>
    public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

    /// <summary>
    /// Gets the application name
    /// </summary>
    public static string ApplicationName => "nopCommerce-integration";

    /// <summary>
    /// Gets the partner identifier
    /// </summary>
    public static string PartnerIdentifier => "nopCommerce";

    /// <summary>
    /// Gets the partner affiliation header used for each request to APIs
    /// </summary>
    public static (string Name, string Value) PartnerHeader => ("X-iWeChatMessage-Application-Id", "f4954821-e7e4-4fca-854e-e36060b5748d");

    /// <summary>
    /// Gets the webhook request signature header
    /// </summary>
    public static string SignatureHeader => "X-iWeChatMessage-Signature";

    /// <summary>
    /// Gets a default period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 15;

    /// <summary>
    /// Gets a default number of products to import in one request
    /// </summary>
    public static int ImportProductsNumber => 500;

    /// <summary>
    /// Gets webhook event names to subscribe
    /// </summary>
    public static List<string> WebhookEventNames =>
    [
    "ProductCreated",
    "InventoryBalanceChanged",
    "InventoryTrackingStopped",
    "ApplicationConnectionRemoved"
    ];

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Misc.WeChatMessage.Configure";

    /// <summary>
    /// Gets the webhook route name
    /// </summary>
    public static string WebhookRouteName => "Plugin.Misc.WeChatMessage.Webhook";

    /// <summary>
    /// Gets a name, type and period (in seconds) of the auto synchronization task
    /// </summary>
    public static (string Name, string Type, int Period) SynchronizationTask =>
        ("Synchronization (PayPal WeChatMessage plugin)", "Nop.Plugin.Misc.WeChatMessage.Services.WeChatMessageSyncTask", 28800);

    //  public static string AllPrefix => $"Nop.{EntityTypeName}.all.";
    //  NopEntityCacheDefaults<MessageTemplate>.AllPrefix
    public static CacheKey MessageTemplatesAllCacheKey => new("Nop.custom-message-template.all.{0}-{1}", "Nop.custom-message-template.all.");


    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : template name
    /// {1} : store ID
    /// </remarks>
    public static CacheKey MessageTemplatesByNameCacheKey => new("Nop.custom-message-template.byname.{0}-{1}", MessageTemplatesByNamePrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : template name
    /// </remarks>
    public static string MessageTemplatesByNamePrefix => "Nop.custom-message-template.byname.{0}";



}