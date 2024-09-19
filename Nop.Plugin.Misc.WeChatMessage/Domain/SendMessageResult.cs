using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain
{
    public partial class SendMessageResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SendMessageResult()
        {
            this.Errors = new List<string>();
        }

        public SendMessageResult(int messageId, string message) : this()
        {

        }

        public int MessageId { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 绑定成功
        /// </summary>
        public bool Success
        {
            get { return (!Errors.Any()); }
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }

        /// <summary>
        /// 错误列表
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}
