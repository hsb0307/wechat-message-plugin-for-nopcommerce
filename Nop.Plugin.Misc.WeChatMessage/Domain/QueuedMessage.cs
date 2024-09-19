using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain
{
    public partial class QueuedMessage: BaseEntity
    {
        /// <summary>
        /// Gets or sets the To property (email address)
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        public string TemplateCode { get; set; }
        public string TemplateParamJson { get; set; }

        public Guid OutGuid { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time in UTC before which this email should not be sent
        /// </summary>
        public DateTime? DontSendBeforeDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }

    }
}
