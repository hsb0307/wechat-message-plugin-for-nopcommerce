using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Stores;
using Nop.Core.Events;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.WeChatMessage.Services
{
    public partial class QueuedMessageService
    {
        #region Fields

        protected readonly WeChatMessageHttpClient _messageHttpClient;
        protected readonly QueuedMessageRecordService _recordService;
        protected readonly WeChatMessageSettings _messageSettings;

        protected readonly ICustomerService _customerService;
        protected readonly ILocalizationService _localizationService;
        protected readonly CustomMessageTemplateService _messageTemplateService;
        protected readonly IMessageTokenProvider _messageTokenProvider;
        protected readonly ITokenizer _tokenizer;

        protected readonly IEventPublisher _eventPublisher;
        protected readonly ILanguageService _languageService;
        protected readonly IStoreContext _storeContext;
        protected readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public QueuedMessageService(
            WeChatMessageHttpClient messageHttpClient,
            QueuedMessageRecordService recordService,

            ILocalizationService localizationService,
            CustomMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
             ITokenizer tokenizer,
            WeChatMessageSettings messageSettings)
        {
            _messageHttpClient = messageHttpClient;
            _recordService = recordService;
            _messageSettings = messageSettings;

            _localizationService = localizationService; 
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _tokenizer = tokenizer;
        }

        #endregion

        #region Utilities

        protected virtual async Task<int> EnsureLanguageIsActiveAsync(int languageId, int storeId)
        {
            //load language by specified ID
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = (await _languageService.GetAllLanguagesAsync(storeId: storeId)).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        protected virtual async Task<IList<CustomMessageTemplate>> GetActiveMessageTemplatesAsync(string messageTemplateName, int storeId)
        {
            //get message templates by the name
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName, storeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<CustomMessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }


        #endregion

        #region Methods

        public virtual async Task<IList<int>> SendOrderPaidMessage(Order order, int languageId)
        {
            ArgumentNullException.ThrowIfNull(order);

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PAID_CUSTOMER_NOTIFICATION, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                // var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                // await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

                //event notification
                // await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                // var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                // var toEmail = billingAddress.Email;
                // var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return await SendNotificationAsync(messageTemplate, languageId, tokens, customer.SystemName, customer.Email );
            }).ToListAsync();
        }


        public virtual async Task<int> SendNotificationAsync(CustomMessageTemplate messageTemplate,
        int languageId, IList<Token> tokens,
        string recipient, string toName, string subject = null)
        {
            ArgumentNullException.ThrowIfNull(messageTemplate);

            if (string.IsNullOrEmpty(subject))
                subject = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Subject, languageId);
            var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedMessage
            {
                // Priority = QueuedEmailPriority.High,
                OutGuid = Guid.NewGuid(),
                Recipient = recipient,
                TemplateCode = messageTemplate.Name,
                TemplateParamJson = messageTemplate.Name,
                CreatedOnUtc = DateTime.UtcNow,

                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            await _recordService.InsertRecordAsync(email);
            return email.Id;
        }



        #endregion
    }
}
