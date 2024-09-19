using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Services
{
    public partial interface IMessageSender
    {
        Task<bool> SendAsync(string openid, string templateCode, string templateParams = null, string outId = null);
    }
}
