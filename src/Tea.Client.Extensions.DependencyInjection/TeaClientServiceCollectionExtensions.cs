// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tea.Client;
using Tea.Client.Discovery;

namespace Tea.Client.Extensions.DependencyInjection;

public static class TeaClientServiceCollectionExtensions
{
    public static IHttpClientBuilder AddTeaClient(this IServiceCollection services, Action<TeaClientOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddTransient<ITeiResolver>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new TeiResolver(httpClientFactory.CreateClient("TeaDiscovery"));
        });

        return services.AddHttpClient<ITeaClient, TeaClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<TeaClientOptions>>().Value;
            if (options.BaseAddress is not null)
            {
                client.BaseAddress = options.BaseAddress;
            }
        });
    }
}
