using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using Nop.Plugin.Misc.WeChatMessage.Models;
using Nop.Plugin.Misc.WeChatMessage.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.WeChatMessage.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class WeChatMessageAdminController : BasePluginController
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly TaxSettings _taxSettings;
    protected readonly QueuedMessageRecordService _weChatMessageRecordService;
    protected readonly QueuedMessageService _weChatMessageService;
    protected readonly WeChatMessageSettings _weChatMessageSettings;

    #endregion

    #region Ctor

    public WeChatMessageAdminController(CurrencySettings currencySettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductService productService,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        IStoreContext storeContext,
        TaxSettings taxSettings,
        QueuedMessageRecordService weChatMessageRecordService,
        QueuedMessageService weChatMessageService,
        WeChatMessageSettings weChatMessageSettings)
    {
        _currencySettings = currencySettings;
        _baseAdminModelFactory = baseAdminModelFactory;
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _productService = productService;
        _scheduleTaskService = scheduleTaskService;
        _settingService = settingService;
        _storeContext = storeContext;
        _taxSettings = taxSettings;
        _weChatMessageRecordService = weChatMessageRecordService;
        _weChatMessageService = weChatMessageService;
        _weChatMessageSettings = weChatMessageSettings;
    }

    #endregion

    #region Methods

    #region Configuration

    
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var messageSettings = await _settingService.LoadSettingAsync<WeChatMessageSettings>(storeScope);

        var model = new ConfigurationModel
        {
            Enabled = _weChatMessageSettings.Enabled,
            AppID = _weChatMessageSettings.AppID,
            AppSecret = _weChatMessageSettings.AppSecret,
            ActiveStoreScopeConfiguration = storeScope
        };
        if (storeScope > 0)
        {
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(messageSettings, x => x.Enabled, storeScope);
            model.AppID_OverrideForStore = await _settingService.SettingExistsAsync(messageSettings, x => x.AppID, storeScope);
            model.AppSecret_OverrideForStore = await _settingService.SettingExistsAsync(messageSettings, x => x.AppSecret, storeScope);
        }

        //var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(WeChatMessageDefaults.SynchronizationTask.Type);
        //if (scheduleTask is not null)
        //{
        //    model.AutoSyncEnabled = scheduleTask.Enabled;
        //    model.AutoSyncPeriod = scheduleTask.Seconds / 60;
        //}

        return View("~/Plugins/Misc.WeChatMessage/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var messageSettings = await _settingService.LoadSettingAsync<WeChatMessageSettings>(storeScope);

        //save settings
        messageSettings.Enabled = model.Enabled;
        messageSettings.AppID = model.AppID;
        messageSettings.AppSecret = model.AppSecret;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */

        await _settingService.SaveSettingOverridablePerStoreAsync(messageSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(messageSettings, x => x.AppID, model.AppID_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(messageSettings, x => x.AppSecret, model.AppSecret_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion

    #endregion
}