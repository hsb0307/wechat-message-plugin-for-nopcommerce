using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.WeChatMessage.Domain.Api;
using Nop.Services.Logging;
using Nop.Services.Payments;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Nop.Plugin.Misc.WeChatMessage.Services;

/// <summary>
/// Represents HTTP client to request third-party services
/// </summary>
public class WeChatMessageHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;
    protected readonly WeChatMessageSettings _weChatMessageSettings;
    private readonly ILogger _logger;

    protected string _accessToken;

    #endregion

    #region Ctor

    public WeChatMessageHttpClient(HttpClient httpClient, WeChatMessageSettings weChatMessageSettings, ILogger logger)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(weChatMessageSettings.RequestTimeout ?? WeChatMessageDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, WeChatMessageDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
        // httpClient.DefaultRequestHeaders.Add(WeChatMessageDefaults.PartnerHeader.Name, WeChatMessageDefaults.PartnerHeader.Value);

        _httpClient = httpClient;
        _logger = logger;
        _weChatMessageSettings = weChatMessageSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <returns>The asynchronous task whose result contains access token</returns>
    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        if (string.IsNullOrEmpty(_weChatMessageSettings.AppID))
            throw new NopException("API key is not set");

        _accessToken = (await RequestAsync<AccessTokenRequest, AccessTokenResponse>(new()
        {
            GrantType = "client_credential",
            AppID = _weChatMessageSettings.AppID,
            AppSecret = _weChatMessageSettings.AppSecret,
           
        }))?.AccessToken;

        return _accessToken;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request services
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) 
        where TRequest : IApiRequest 
        where TResponse : IApiResponse
    {
        //prepare request parameters    AccessTokenRequest   AccessTokenResponse
        //var requestString = JsonConvert.SerializeObject(request);
        //var requestContent = request is not AccessTokenRequest authentication
        //    ? (ByteArrayContent)new StringContent(requestString, Encoding.UTF8, MimeTypes.ApplicationJson)
        //    : new FormUrlEncodedContent(new Dictionary<string, string>
        //    {
        //        ["grant_type"] = authentication.GrantType,
        //        ["appid"] = authentication.AppID,
        //        ["secret"] = authentication.AppSecret
        //    });

        //var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(new Uri(request.BaseUrl), request.Path))
        //{
        //    Content = requestContent
        //};

        try
        {
            var requestString = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestString, Encoding.UTF8, MimeTypes.ApplicationJson); // Encoding.Default
            // var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(new Uri(request.BaseUrl), 
                String.IsNullOrEmpty(request.AccessToken) ? request.Path : request.Path + $"?access_token={request.AccessToken}"))
            {
                Content = requestContent
            };

            var httpResponse = await _httpClient.SendAsync(requestMessage);
            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
            if (result.ErrorCode > 0)
            {
                // var error = !string.IsNullOrEmpty(result.ErrorMessage) ? result.ErrorMessage : result.Error;
                throw new NopException($"Request error: {result.ErrorMessage}");
            }
            //if (!string.IsNullOrEmpty(result?.DeveloperMessage))
            //    throw new NopException($"Request error: {result.DeveloperMessage}{Environment.NewLine}Details: {responseString}");

            return result;
        }
        catch (AggregateException exception)
        {
            //rethrow actual exception
            // throw exception.InnerException;
            await _logger.ErrorAsync($"{WeChatMessageDefaults.SystemName} error", exception);

            // return new WeChatTemplateMessageResponse() { ErrorDescription = exception.Message };
            throw exception.InnerException;
            // return Task.FromResult(new WeChatTemplateMessageResponse() { ErrorDescription = exception.Message });
        }

        // return Task.FromResult(new WeChatTemplateMessageResponse() { ErrorDescription = "" });
        // return Task.FromResult(new WeChatTemplateMessageResponse { Error = "Capture method not supported" });
        // return new WeChatTemplateMessageResponse() { ErrorDescription = "" };
    }

    #endregion
}