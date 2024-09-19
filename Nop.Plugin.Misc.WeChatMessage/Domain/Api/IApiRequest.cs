﻿namespace Nop.Plugin.Misc.WeChatMessage.Domain.Api;

/// <summary>
/// Represents request object
/// </summary>
public interface IApiRequest
{
    /// <summary>
    /// Gets the request base URL
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the request method
    /// </summary>
    public string Method { get; }

    public string AccessToken { get; set; }
}