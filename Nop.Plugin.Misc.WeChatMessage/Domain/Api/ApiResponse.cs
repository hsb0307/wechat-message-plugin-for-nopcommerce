using Newtonsoft.Json;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api;

/// <summary>
/// Represents base response details
/// </summary>
public class ApiResponse : IApiResponse
{
    ///// <summary>
    ///// Gets or sets the error message
    ///// </summary>
    //[JsonProperty(PropertyName = "error")]
    //public string Error { get; set; }

    ///// <summary>
    ///// Gets or sets the error description
    ///// </summary>
    //[JsonProperty(PropertyName = "error_description")]
    //public string ErrorDescription { get; set; }

    ///// <summary>
    ///// Gets or sets the developer message
    ///// </summary>
    //[JsonProperty(PropertyName = "developerMessage")]
    //public string DeveloperMessage { get; set; }

    [JsonProperty(PropertyName = "errcode")]
    public int ErrorCode { get; set; }

    [JsonProperty(PropertyName = "errmsg")]
    public string ErrorMessage { get; set; }
}