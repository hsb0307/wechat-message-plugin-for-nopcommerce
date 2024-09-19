using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api
{
    public class WeChatTemplateMessageResponse: ApiResponse
    {

        [JsonProperty(PropertyName = "msgid ")]
        public long MessageId { get; set; }
        
    }
}
