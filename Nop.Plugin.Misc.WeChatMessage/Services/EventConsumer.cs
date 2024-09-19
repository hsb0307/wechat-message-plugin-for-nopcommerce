using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using Nop.Services.Catalog;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.WeChatMessage.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer :
   
    IConsumer<OrderPaidEvent>

{
    #region Fields


    protected readonly QueuedMessageService _weChatMessageService;
    protected readonly WeChatMessageSettings _weChatMessageSettings;

    #endregion

    #region Ctor

    public EventConsumer(

        QueuedMessageService weChatMessageService,
        WeChatMessageSettings weChatMessageSettings)
    {
       
        _weChatMessageService = weChatMessageService;
        _weChatMessageSettings = weChatMessageSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle entity created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        if (eventMessage.Order is null)
            return;

       

        await _weChatMessageService.SendOrderPaidMessage(eventMessage.Order, eventMessage.Order.CustomerLanguageId);
    }


    #endregion
}