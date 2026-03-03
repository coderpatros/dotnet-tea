// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;

namespace CoderPatros.Tea.Client.Exceptions;

public sealed class TeaAuthenticationException : TeaApiException
{
    public TeaAuthenticationException(HttpStatusCode statusCode, string? responseBody = null)
        : base(statusCode, responseBody,
            $"TEA API authentication failed with status code {(int)statusCode} ({statusCode})")
    {
    }
}
