// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;

namespace CoderPatros.Tea.Client.Exceptions;

public class TeaApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }

    public TeaApiException(HttpStatusCode statusCode, string? responseBody = null, string? message = null)
        : base(message ?? $"TEA API request failed with status code {(int)statusCode} ({statusCode})")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public TeaApiException(HttpStatusCode statusCode, string? responseBody, string? message, Exception? innerException)
        : base(message ?? $"TEA API request failed with status code {(int)statusCode} ({statusCode})", innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
