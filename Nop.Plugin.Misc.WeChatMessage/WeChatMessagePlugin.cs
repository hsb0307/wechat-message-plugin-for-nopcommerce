using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.WeChatMessage.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.WeChatMessage;

/// <summary>
/// Represents WeChatMessage plugin
/// </summary>
public class WeChatMessagePlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly ISettingService _settingService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly MediaSettings _mediaSettings;
    protected readonly QueuedMessageService _weChatMessageService;

    #endregion

    #region Ctor

    public WeChatMessagePlugin(IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        MediaSettings mediaSettings,
        QueuedMessageService weChatMessageService)
    {
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _scheduleTaskService = scheduleTaskService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _mediaSettings = mediaSettings;
        _weChatMessageService = weChatMessageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(WeChatMessageDefaults.ConfigurationRouteName);
    }

   

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        await _settingService.SetSettingAsync($"{nameof(MediaSettings)}.{nameof(MediaSettings.UseAbsoluteImagePath)}", true, clearCache: false);

        await _settingService.SaveSettingAsync(new WeChatMessageSettings
        {
            Enabled = true,
            AppID = "wx5934415fe9xxxxxx",
            AppSecret = "2bd087c7edc96b7c5f670611f6yyyyyy"
        });

        if (await _scheduleTaskService.GetTaskByTypeAsync(WeChatMessageDefaults.SynchronizationTask.Type) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new()
            {
                Enabled = false,
                StopOnError = false,
                LastEnabledUtc = DateTime.UtcNow,
                Name = WeChatMessageDefaults.SynchronizationTask.Name,
                Type = WeChatMessageDefaults.SynchronizationTask.Type,
                Seconds = WeChatMessageDefaults.SynchronizationTask.Period
            });
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.WeChatMessage.Menu.Pos"] = "POS",

            ["Plugins.Misc.WeChatMessage.Title"] = "WeChat Template Message",

            // ["Plugins.Misc.WeChatMessage.Configuration.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.Enabled"] = "Enabled",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.AppID"] = "App ID",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.AppID.Hint"] = "Enter the App ID. The App ID is used together with the API key to identify the merchant.",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.AppSecret"] = "App Secret",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.AppSecret.Hint"] = "Enter the App Secret The App Secret contains merchant identity information, and is valid until the merchant revokes it.",
            ["Plugins.Misc.WeChatMessage.Configuration.Fields.AppSecret.Required"] = "App Secret is required",
            
            ["Plugins.Misc.WeChatMessage.Sync.Start.Confirm"] = @"
                    <p>
                        You want to start synchronization with the connected account.
                    </p>
                    <ol>
                        <li>Discounts assigned to order subtotal will be added if the setting is enabled.</li>
                        <li>Existing library items will be removed before products are imported if the setting is enabled.</li>
                        <li>Products removed from the catalog will be removed.</li>
                        <li>Updated images will be replaced.</li>
                        <li>Added and updated products will be imported with the appropriate settings (prices, images, inventory tracking, default tax).</li>
                    </ol>
                    <p>
                        Synchronization may take some time.
                    </p>",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        var weChatMessageSettings = await _settingService.LoadSettingAsync<WeChatMessageSettings>();

        await _settingService.DeleteSettingAsync<WeChatMessageSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.WeChatMessage");

        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(WeChatMessageDefaults.SynchronizationTask.Type);
        if (scheduleTask is not null)
            await _scheduleTaskService.DeleteTaskAsync(scheduleTask);

        await base.UninstallAsync();
    }

    #endregion
}