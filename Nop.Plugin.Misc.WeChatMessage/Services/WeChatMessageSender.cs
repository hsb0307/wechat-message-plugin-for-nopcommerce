using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.WeChatMessage;
using Nop.Plugin.Misc.WeChatMessage.Domain.Api;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Nop.Plugin.Misc.WeChatMessage.Domain.Api.WeChatTemplateMessageRequest;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Nop.Plugin.Misc.WeChatMessage.Services
{
    /// <summary>
    /// 此类可以配合定时任务使用，也可以直接在 BalanceChangedEventConsumer 中使用
    /// </summary>
    public partial class WeChatMessageSender : IMessageSender
    {

        // private readonly IQueuedSmsService _queuedSmsService;
        // private readonly IPluginFinder _pluginFinder;
        protected readonly WeChatMessageHttpClient _weChatMessageHttpClient;
        private readonly WeChatMessageSettings _weChatSettings;
        
        private readonly ILogger _logger;

        public WeChatMessageSender(WeChatMessageHttpClient weChatMessageHttpClient,
            WeChatMessageSettings weChatSettings,
            ILogger logger)
        {
            _weChatMessageHttpClient = weChatMessageHttpClient;
            this._weChatSettings = weChatSettings;
            this._logger = logger;
        }

        public async Task<bool> SendAsync(string openid, string templateCode, string templateParams = null, string outId = null)
        {
            var accessToken = await _weChatMessageHttpClient.GetAccessTokenAsync();
            BalanceData data = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceData>(templateParams);
            var request = new WeChatTemplateMessageRequest
            {
                OpenId = openid,   
                TemplateId = templateCode,
                TitleColor = "#FF0000",
                Data = new Dictionary<string, DictionaryItem>()
                {
                    { "MobileNumber", new DictionaryItem {  value = data.MobileNumber, color = "#173177" } },
                    { "Money", new DictionaryItem {  value = data.Money, color = "#173177" }  },
                    { "Balance", new DictionaryItem {  value = data.Balance, color = "#173177" }  },
                    { "Date", new DictionaryItem {  value = data.Date, color = "#173177" }  }
                }
            };

            var response = await _weChatMessageHttpClient.RequestAsync<WeChatTemplateMessageRequest, WeChatTemplateMessageResponse>(request);
            if (response == null || response.ErrorCode > 0)
            {
                _logger.Information(response.ErrorMessage);
                return false;
            }
            return true;
            // throw new NotImplementedException();
        }


        // 要配置内网穿透进行测试，这里使用花生壳
        // 在 花生壳 中配置 映射协议为 http 或者 https 

        // 在 applicationhost.config 文件中添加： <binding protocol="http" bindingInformation="*:15637:192.168.0.55" />
        // 以管理员身份启动 visual studio ，打开该项目

        // https://mp.weixin.qq.com/debug/cgi-bin/sandboxinfo?action=showinfo&t=sandbox/index
        // https://mp.weixin.qq.com/cgi-bin/user_tag?action=get_all_data&lang=zh_CN&token=1133418786
        // 模板消息
        // https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Template_Message_Interface.html
        // https://mp.weixin.qq.com/debug/cgi-bin/readtmpl?t=tmplmsg/faq_tmpl
        // 尊敬的{{MobileNumber.DATA}}:您的最新余额为{{Balance.DATA}}。交易时间:{{Date.DATA}}, 金额{{Money.DATA}}

        //EncodingInfo[] encodings = Encoding.GetEncodings();

        //Console.WriteLine("Registered Encodings:");
        //foreach (EncodingInfo encodingInfo in encodings)
        //{
        //    Console.WriteLine($"- Name: {encodingInfo.Name}, CodePage: {encodingInfo.CodePage}");
        //}

        //var ss =  Encoding.UTF8.EncodingName;
        //
        //if (accessToken != null)
        //{
        //    string sa = accessToken;
        //}

        //public bool Send(string recipient, string message)
        //{
        //    // throw new NotImplementedException();
        //    var parameters = new Dictionary<string, string>()
        //    {
        //        { "grant_type", "client_credential" },
        //        { "appid", "wx5934415fe9235f9f" },
        //        { "secret", "2bd087c7edc96b7c5f670611f67dd03a" }
        //    };

        //    var accessToken = HttpClientHelper.Get("https://api.weixin.qq.com/cgi-bin/token", parameters, Encoding.UTF8.WebName);
        //    if (accessToken != null)
        //    {
        //        string sa = accessToken;
        //    }
        //    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        //    {
        //        NullValueHandling = NullValueHandling.Ignore,
        //    };
        //    var weChatTemplateMessageBody = new WeChatTemplateMessageBody()
        //    {
        //        touser = recipient,
        //        template_id = message,
        //    };
        //    // {"MobileNumber": "18753686528","Money": "178","Balance": "2,109.1","Date": "2024-08-19 10:29:51"}
        //    string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(weChatTemplateMessageBody, Newtonsoft.Json.Formatting.None, jsonSerializerSettings);
        //    var response = HttpClientHelper.PostJson("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + accessToken, requestBody);
        //    return true;
        //}

        //public bool Send(string openid, string templateCode, string templateParams = null, string outId = null)
        //{
        //    if (!_weChatSettings.Enabled)
        //    {
        //        return false;
        //    }
        //    //// 测试用
        //    //var parameters = new Dictionary<string, string>()
        //    //{
        //    //    { "grant_type", "client_credential" },
        //    //    { "appid", "wx5934415fe9235f9f" },
        //    //    { "secret", "2bd087c7edc96b7c5f670611f67dd03a" }
        //    //};
        //    //// 测试用的 openid 和 template_id
        //    //openid = "ote6l6nrihu6r3Kv201rKdR4lMSs";
        //    //templateCode = "pc8tmcJFIgro-GHZhegZKvgaHfzMiRvvTwN4gjGfo1w";

        //    // 实际
        //    //var parameters = new Dictionary<string, string>()
        //    //{
        //    //    { "grant_type", "client_credential" },
        //    //    { "appid", "wx7edd1b58863bd45c" },
        //    //    { "secret", "9d68d3944e59e426735a723799a40f0b" }
        //    //};
        //    // 从数据库中获取
        //    var parameters = new Dictionary<string, string>()
        //    {
        //        { "grant_type", "client_credential" },
        //        { "appid", _weChatSettings.AppID },
        //        { "secret", _weChatSettings.AppSecret }
        //    };
        //    var accessTokenResponse = HttpClientHelper.Get("https://api.weixin.qq.com/cgi-bin/token", parameters, Encoding.UTF8.WebName);
        //    if (accessTokenResponse == null)
        //    {
        //        return false;
        //    }
        //    //if (accessTokenResponse != null)
        //    //{
        //    //    _logger.Information(accessTokenResponse);
        //    //}
        //    AccessTokenResponse accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenResponse>(accessTokenResponse);
        //    if (accessToken.access_token == null)
        //    {
        //        // {"errcode":40164,"errmsg":"invalid ip 124.134.196.125 ipv6 ::ffff:124.134.196.125, not in whitelist rid: 66c544cf-009df51f-6fd6670c"}
        //        _logger.Information(accessTokenResponse);
        //        return false;
        //    }
        //    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        //    {
        //        NullValueHandling = NullValueHandling.Ignore,
        //        DateFormatString = "yyyy-MM-dd HH:mm:ss",
        //    };
        //    // {"MobileNumber": "18753686528","Money": "178","Balance": "2,109.1","Date": "2024-08-19 10:29:51"}
        //    BalanceMessage message = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceMessage>(templateParams);
        //    var weChatTemplateMessageBody = new WeChatTemplateMessageBody()
        //    {
        //        touser = openid,
        //        template_id = templateCode,
        //        topcolor = "#FF0000",
        //        data = new Dictionary<string, DictionaryItem>()
        //        {
        //            { "MobileNumber", new DictionaryItem {  value = message.MobileNumber, color = "#173177" } },
        //            { "Money", new DictionaryItem {  value = message.Money, color = "#173177" }  },
        //            { "Balance", new DictionaryItem {  value = message.Balance, color = "#173177" }  },
        //            { "Date", new DictionaryItem {  value = message.Date, color = "#173177" }  }
        //        }
        //    };

        //    string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(weChatTemplateMessageBody, Newtonsoft.Json.Formatting.None, jsonSerializerSettings);
        //    var response = HttpClientHelper.PostJson("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + accessToken.access_token, requestBody);

        //    var res = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatTemplateMessageResponse>(response); 
        //    if(res != null && res.errcode > 0)
        //    {
        //        _logger.Information(response);
        //        return false;
        //    }

        //    return true;
        //}

    }
}
