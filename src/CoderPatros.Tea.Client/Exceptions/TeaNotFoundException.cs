// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;
using CoderPatros.Tea.Client.Models;

namespace CoderPatros.Tea.Client.Exceptions;

public sealed class TeaNotFoundException : TeaApiException
{
    public ErrorResponse? ErrorResponse { get; }

    public TeaNotFoundException(string? responseBody = null, ErrorResponse? errorResponse = null)
        : base(HttpStatusCode.NotFound, responseBody,
            errorResponse is not null
                ? $"TEA API returned 404: {errorResponse.Error}"
                : "TEA API returned 404: Not Found")
    {
        ErrorResponse = errorResponse;
    }
}
