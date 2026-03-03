// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;

namespace Tea.Client.Exceptions;

public sealed class TeaBadRequestException : TeaApiException
{
    public TeaBadRequestException(string? responseBody = null)
        : base(HttpStatusCode.BadRequest, responseBody, "TEA API returned 400: Bad Request")
    {
    }
}
