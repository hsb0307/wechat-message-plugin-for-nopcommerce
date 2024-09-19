using FluentValidation;
using Nop.Plugin.Misc.WeChatMessage.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.WeChatMessage.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        //RuleFor(model => model.AppID)
        //    .NotEmpty()
        //    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.WeChatMessage.Configuration.Fields.ApiKey.Required"))
        //    .When(model => !string.IsNullOrEmpty(model.ClientId));

        RuleFor(model => model.AppID)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.WeChatMessage.Configuration.Fields.AppID.Required"))
            .When(model => model.Enabled);

        RuleFor(model => model.AppSecret)
           .NotEmpty()
           .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.WeChatMessage.Configuration.Fields.AppSecret.Required"))
           .When(model => model.Enabled);
    }

    #endregion
}