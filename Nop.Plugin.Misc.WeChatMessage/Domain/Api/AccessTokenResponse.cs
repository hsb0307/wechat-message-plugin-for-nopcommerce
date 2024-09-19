using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api
{
    public class AccessTokenResponse: ApiResponse
    {
        /// <summary>
        /// Gets or sets the access token that is exchanged with the authorisation code
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the remaining lifetime of an access token in seconds
        /// </summary>
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        //[JsonProperty(PropertyName = "errcode")]
        //public int ErrorCode { get; set; }

        //[JsonProperty(PropertyName = "errmsg")]
        //public string ErrorMessage { get; set; }
        
    }
}
