// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net.Http.Headers;

namespace Tea.Client.Authentication;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly Func<CancellationToken, Task<string>> _tokenProvider;

    public BearerTokenHandler(string token)
        : this(_ => Task.FromResult(token))
    {
    }

    public BearerTokenHandler(Func<CancellationToken, Task<string>> tokenProvider)
    {
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider(cancellationToken).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
