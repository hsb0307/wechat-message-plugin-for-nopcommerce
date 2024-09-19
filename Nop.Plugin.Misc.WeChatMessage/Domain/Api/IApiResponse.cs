using Newtonsoft.Json;

namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api;

/// <summary>
/// Represents response from the service
/// </summary>
public interface IApiResponse
{
    ///// <summary>
    ///// Gets or sets the error message
    ///// </summary>
    //public string Error { get; set; }

    ///// <summary>
    ///// Gets or sets the error description
    ///// </summary>
    //public string ErrorDescription { get; set; }

    ///// <summary>
    ///// Gets or sets the developer message
    ///// </summary>
    //public string DeveloperMessage { get; set; }


    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }


}