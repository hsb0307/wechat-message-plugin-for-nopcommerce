using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.WeChatMessage.Services;

/// <summary>
/// Represents a schedule task to synchronize products
/// </summary>
public class WeChatMessageSyncTask : IScheduleTask
{
    #region Fields

    protected readonly IMessageSender _messageSender;
    protected readonly QueuedMessageRecordService _messageService;
    protected readonly WeChatMessageSettings _weChatMessageSettings;

    

    #endregion

    #region Ctor

    public WeChatMessageSyncTask(IMessageSender messageSender,
        QueuedMessageRecordService messageService,
        WeChatMessageSettings weChatMessageSettings)
    {
        _messageSender = messageSender;
        _messageService = messageService;
        _weChatMessageSettings = weChatMessageSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {

        var queuedMessages = await  _messageService.SearchQueuedMessagesAsync(recipient: null, createdFromUtc: null, createdToUtc: null,
             loadNotSentItemsOnly: true, loadOnlyItemsToBeSent: true, maxSendTries: 1, pageIndex: 0, pageSize: 2);

        foreach (var queuedMessage in queuedMessages)
        {
            await _messageSender.SendAsync(queuedMessage.Recipient, queuedMessage.TemplateCode, queuedMessage.TemplateParamJson);
        }
    }

    #endregion
}