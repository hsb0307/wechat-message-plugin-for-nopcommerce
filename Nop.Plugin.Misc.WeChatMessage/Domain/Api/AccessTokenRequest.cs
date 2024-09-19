using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api
{
    public class AccessTokenRequest : ApiRequest
    {
        public override string BaseUrl => "https://api.weixin.qq.com/cgi-bin/"; // throw new NotImplementedException();

        public override string Path => "token"; //throw new NotImplementedException();

        public override string Method => HttpMethods.Get;//  HttpMethod.Get; // throw new NotImplementedException();

        // [JsonIgnore]
        [JsonProperty(PropertyName = "grant_type ")]
        public string GrantType { get; set; }

        [JsonProperty(PropertyName = "appid ")]
        public string AppID { get; set; }

        [JsonProperty(PropertyName = "secret ")]
        public string AppSecret { get; set; }


    }
}
