// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Tea.Client.Authentication;

namespace Tea.Client.Extensions.DependencyInjection;

public static class TeaClientBuilderExtensions
{
    public static IHttpClientBuilder AddBearerToken(this IHttpClientBuilder builder, string token)
    {
        return builder.AddHttpMessageHandler(() => new BearerTokenHandler(token));
    }

    public static IHttpClientBuilder AddBearerToken(this IHttpClientBuilder builder, Func<CancellationToken, Task<string>> tokenProvider)
    {
        return builder.AddHttpMessageHandler(() => new BearerTokenHandler(tokenProvider));
    }

    public static IHttpClientBuilder AddMutualTls(this IHttpClientBuilder builder, X509Certificate2 clientCertificate)
    {
        return builder.ConfigurePrimaryHttpMessageHandler(() => MutualTlsHandler.Create(clientCertificate));
    }

    public static IHttpClientBuilder AddMutualTls(this IHttpClientBuilder builder, string certificatePath, string? password = null)
    {
        return builder.ConfigurePrimaryHttpMessageHandler(() => MutualTlsHandler.Create(certificatePath, password));
    }
}
