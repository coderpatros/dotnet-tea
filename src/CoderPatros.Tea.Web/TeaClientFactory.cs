// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net.Http.Headers;
using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Discovery;
using Microsoft.Extensions.Options;

namespace CoderPatros.Tea.Web;

public interface ITeaClientFactory
{
    (ITeaClient Client, ITeiResolver Resolver, string? BaseUrl) Create(ISession session);
}

public class TeaClientFactory : ITeaClientFactory
{
    private readonly TeaClientOptions _defaultOptions;

    public TeaClientFactory(IOptions<TeaClientOptions> options)
    {
        _defaultOptions = options.Value;
    }

    public (ITeaClient Client, ITeiResolver Resolver, string? BaseUrl) Create(ISession session)
    {
        var baseUrl = session.GetString("Tea:BaseUrl");
        var token = session.GetString("Tea:Token");

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = _defaultOptions.BaseAddress?.ToString();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            token = _defaultOptions.BearerToken;
        }

        var client = new HttpClient();
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            var uri = baseUrl.TrimEnd('/');
            client.BaseAddress = new Uri(uri.EndsWith('/') ? uri : uri + "/");
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var teaClient = new TeaClient(client);
        var resolver = new TeiResolver(new HttpClient());

        return (teaClient, resolver, baseUrl);
    }
}
