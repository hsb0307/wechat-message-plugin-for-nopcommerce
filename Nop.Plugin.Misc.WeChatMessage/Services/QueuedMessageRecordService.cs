using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Services
{
    public partial class QueuedMessageRecordService
    {
        #region Fields

        protected readonly IRepository<QueuedMessage> _messageRepository;
        protected readonly WeChatMessageSettings _messageSettings;

        #endregion

        #region Ctor

        public QueuedMessageRecordService(

            IRepository<QueuedMessage> messageRepository,
            WeChatMessageSettings messageSettings)
        {
            _messageRepository = messageRepository;

            _messageSettings = messageSettings;
        }

        #endregion

        #region Utilities


        #endregion

        #region Methods

        /// <summary>
        /// Get a record by the identifier
        /// </summary>
        /// <param name="id">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the record for synchronization
        /// </returns>
        public async Task<QueuedMessage> GetQueuedMessageByIdAsync(int id)
        {
            return await _messageRepository.GetByIdAsync(id, null);
        }

        /// <summary>
        /// Insert the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertRecordAsync(QueuedMessage record)
        {
            await _messageRepository.InsertAsync(record, false);
        }

        /// <summary>
        /// Insert records
        /// </summary>
        /// <param name="records">Records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertRecordsAsync(List<QueuedMessage> records)
        {
            await _messageRepository.InsertAsync(records, false);
        }

        /// <summary>
        /// Update the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateRecordAsync(QueuedMessage record)
        {
            await _messageRepository.UpdateAsync(record, false);
        }

        /// <summary>
        /// Update records
        /// </summary>
        /// <param name="records">Records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateRecordsAsync(List<QueuedMessage> records)
        {
            await _messageRepository.UpdateAsync(records, false);
        }

        /// <summary>
        /// Delete the record
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteRecordAsync(QueuedMessage record)
        {
            await _messageRepository.DeleteAsync(record, false);
        }

        /// <summary>
        /// Delete records
        /// </summary>
        /// <param name="ids">Records identifiers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteRecordsAsync(List<int> ids)
        {
            await _messageRepository.DeleteAsync(record => ids.Contains(record.Id));
        }

      

        /// <summary>
        /// Get all records for synchronization
        /// </summary>
        /// <param name="productOnly">Whether to load only product records</param>
        /// <param name="active">Whether to load only active records; true - active only, false - inactive only, null - all records</param>
        /// <param name="operationTypes">Operation types; pass null to load all records</param>
        /// <param name="productUuid">Product unique identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the records for synchronization
        /// </returns>
        public async Task<IPagedList<QueuedMessage>> GetAllRecordsAsync(bool productOnly = false,
            bool? active = null,string productUuid = null, //  List<OperationType> operationTypes = null, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _messageRepository.GetAllPagedAsync(query =>
            {
                //if (productOnly)
                //    query = query.Where(record => record.ProductId > 0 && record.CombinationId == 0);

                //if (active.HasValue)
                //    query = query.Where(record => record.Active == active.Value);

                //if (operationTypes?.Any() ?? false)
                //    query = query.Where(record => operationTypes.Contains((OperationType)record.OperationTypeId));

                //if (!string.IsNullOrEmpty(productUuid))
                //    query = query.Where(record => record.Uuid == productUuid);

                query = query.OrderBy(record => record.Id);

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<QueuedMessage>> SearchQueuedMessagesAsync(string recipient, 
        DateTime? createdFromUtc, DateTime? createdToUtc,
        bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
        int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _messageRepository.Table;

            //if (!string.IsNullOrEmpty(recipient))
            //    query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!string.IsNullOrEmpty(recipient))
                query = query.Where(qe => qe.Recipient == recipient);
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            if (loadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries < maxSendTries);
            //query = loadNewest ?
            //    //load the newest records
            //    query.OrderByDescending(qe => qe.CreatedOnUtc) :
            //    //load by priority
            //    query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);
            query = query.OrderByDescending(qe => qe.CreatedOnUtc);

            var queuedEmails = await query.ToPagedListAsync(pageIndex, pageSize);

            return queuedEmails;
        }



        #endregion

    }
}
