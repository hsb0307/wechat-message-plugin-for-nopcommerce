using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api
{
    public class WeChatTemplateMessageRequest : ApiRequest
    {
        public override string BaseUrl => "https://api.weixin.qq.com/cgi-bin/message/"; // throw new NotImplementedException();

        public override string Path => "template/send";

        public override string Method => HttpMethods.Post;

        // 
        [JsonProperty(PropertyName = "touser ")]
        public string OpenId { get; set; }

        [JsonProperty(PropertyName = "template_id ")]
        public string TemplateId { get; set; }
        [JsonProperty(PropertyName = "topcolor ")]
        public string TitleColor { get; set; }

        [JsonProperty(PropertyName = "data ")]
        public IDictionary<string, DictionaryItem> Data { get; set; }

        #region Nested classes

        public class DictionaryItem
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        #endregion

    }
}
