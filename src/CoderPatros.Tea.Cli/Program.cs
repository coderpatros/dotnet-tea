// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;
using CoderPatros.Tea.Cli.Commands;
using CoderPatros.Tea.Cli.Output;
using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Authentication;
using CoderPatros.Tea.Client.Discovery;

namespace CoderPatros.Tea.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var baseUrlOption = new Option<string?>("--base-url") { Description = "TEA server base URL (or set TEA_BASE_URL env var)", Recursive = true };
        var domainOption = new Option<string?>("--domain") { Description = "Discover TEA server via .well-known/tea for this domain", Recursive = true };
        var tokenOption = new Option<string?>("--token") { Description = "Bearer token for authentication (or set TEA_TOKEN env var)", Recursive = true };
        var jsonOption = new Option<bool>("--json") { Description = "Output raw JSON instead of formatted tables", Recursive = true };
        var timeoutOption = new Option<int>("--timeout") { Description = "Request timeout in seconds", DefaultValueFactory = _ => 30, Recursive = true };

        var globalOptions = new GlobalOptions(baseUrlOption, domainOption, tokenOption, jsonOption, timeoutOption);

        var rootCommand = new RootCommand("CLI tool for the Transparency Exchange API (TEA)");
        rootCommand.Options.Add(baseUrlOption);
        rootCommand.Options.Add(domainOption);
        rootCommand.Options.Add(tokenOption);
        rootCommand.Options.Add(jsonOption);
        rootCommand.Options.Add(timeoutOption);

        rootCommand.Subcommands.Add(DiscoverCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(SearchProductsCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(SearchReleasesCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetProductCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetReleaseCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetCollectionCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetProductReleasesCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetComponentCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetComponentReleasesCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(ListCollectionsCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetArtifactCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(GetCleCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(DownloadCommand.Create(globalOptions));
        rootCommand.Subcommands.Add(InspectCommand.Create(globalOptions));

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }

    internal static IOutputFormatter CreateFormatter(bool json)
    {
        return json ? new JsonOutputFormatter() : new TableOutputFormatter();
    }

    internal static HttpClient CreateHttpClient(string? baseUrl, string? token, int timeout)
    {
        var effectiveBaseUrl = baseUrl ?? Environment.GetEnvironmentVariable("TEA_BASE_URL");
        var effectiveToken = token ?? Environment.GetEnvironmentVariable("TEA_TOKEN");

        HttpMessageHandler handler = new HttpClientHandler();

        if (effectiveToken is not null)
        {
            var tokenHandler = new BearerTokenHandler(effectiveToken)
            {
                InnerHandler = handler
            };
            handler = tokenHandler;
        }

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(timeout)
        };

        if (effectiveBaseUrl is not null)
        {
            client.BaseAddress = new Uri(effectiveBaseUrl);
        }

        return client;
    }

    internal static ITeaClient CreateTeaClient(string? baseUrl, string? token, int timeout)
    {
        var client = CreateHttpClient(baseUrl, token, timeout);
        return new TeaClient(client);
    }

    internal static ITeiResolver CreateTeiResolver(string? baseUrl, string? token, int timeout)
    {
        var client = CreateHttpClient(baseUrl, token, timeout);
        return new TeiResolver(client);
    }
}

internal sealed record GlobalOptions(
    Option<string?> BaseUrl,
    Option<string?> Domain,
    Option<string?> Token,
    Option<bool> Json,
    Option<int> Timeout);
