// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Tea.Client.Discovery;
using Tea.Client.Exceptions;

namespace Tea.Client.Tests.Discovery;

public class TeiResolverTests
{
    private readonly MockHttpMessageHandler _mockHttp = new();

    [Fact]
    public async Task ResolveAsync_FullFlow_Succeeds()
    {
        // Mock .well-known/tea
        _mockHttp.When("https://products.example.com/.well-known/tea")
            .Respond("application/json", """
            {
              "schemaVersion": 1,
              "endpoints": [
                {
                  "url": "https://api.teaexample.com",
                  "versions": ["0.3.0-beta.2", "1.0.0"],
                  "priority": 1
                }
              ]
            }
            """);

        // Mock discovery endpoint
        _mockHttp.When("https://api.teaexample.com/v0.3.0-beta.2/discovery*")
            .Respond("application/json", """
            [
              {
                "productReleaseUuid": "d4d9f54a-abcf-11ee-ac79-1a52914d44b",
                "servers": [
                  {
                    "rootUrl": "https://api.teaexample.com",
                    "versions": ["0.3.0-beta.2"],
                    "priority": 0.8
                  }
                ]
              }
            ]
            """);

        var httpClient = _mockHttp.ToHttpClient();
        var resolver = new TeiResolver(httpClient);

        var results = await resolver.ResolveAsync("urn:tei:uuid:products.example.com:d4d9f54a-abcf-11ee-ac79-1a52914d44b");

        results.Should().HaveCount(1);
        results[0].ProductReleaseUuid.Should().Be("d4d9f54a-abcf-11ee-ac79-1a52914d44b");
    }

    [Fact]
    public async Task ResolveAsync_SelectsHighestPriorityEndpoint()
    {
        _mockHttp.When("https://example.com/.well-known/tea")
            .Respond("application/json", """
            {
              "schemaVersion": 1,
              "endpoints": [
                {
                  "url": "https://low-priority.com",
                  "versions": ["0.3.0-beta.2"],
                  "priority": 0.3
                },
                {
                  "url": "https://high-priority.com",
                  "versions": ["0.3.0-beta.2"],
                  "priority": 0.9
                }
              ]
            }
            """);

        // Only the high priority endpoint should be called
        _mockHttp.When("https://high-priority.com/v0.3.0-beta.2/discovery*")
            .Respond("application/json", """
            [
              {
                "productReleaseUuid": "test-uuid",
                "servers": [{ "rootUrl": "https://high-priority.com", "versions": ["0.3.0-beta.2"] }]
              }
            ]
            """);

        var httpClient = _mockHttp.ToHttpClient();
        var resolver = new TeiResolver(httpClient);

        var results = await resolver.ResolveAsync("urn:tei:uuid:example.com:some-id");
        results[0].Servers[0].RootUrl.Should().Be("https://high-priority.com");
    }

    [Fact]
    public async Task ResolveAsync_NoCompatibleVersion_ThrowsDiscoveryException()
    {
        _mockHttp.When("https://example.com/.well-known/tea")
            .Respond("application/json", """
            {
              "schemaVersion": 1,
              "endpoints": [
                {
                  "url": "https://api.example.com",
                  "versions": ["1.0.0", "2.0.0"]
                }
              ]
            }
            """);

        var httpClient = _mockHttp.ToHttpClient();
        var resolver = new TeiResolver(httpClient);

        var act = () => resolver.ResolveAsync("urn:tei:uuid:example.com:some-id");
        await act.Should().ThrowAsync<TeaDiscoveryException>()
            .WithMessage("*No compatible endpoint*");
    }

    [Fact]
    public async Task ResolveAsync_InvalidTei_ThrowsFormatException()
    {
        var httpClient = _mockHttp.ToHttpClient();
        var resolver = new TeiResolver(httpClient);

        var act = () => resolver.ResolveAsync("not-a-tei");
        await act.Should().ThrowAsync<FormatException>();
    }

    [Fact]
    public async Task ResolveAsync_EmptyEndpoints_ThrowsDiscoveryException()
    {
        _mockHttp.When("https://empty.com/.well-known/tea")
            .Respond("application/json", """
            {
              "schemaVersion": 1,
              "endpoints": []
            }
            """);

        var httpClient = _mockHttp.ToHttpClient();
        var resolver = new TeiResolver(httpClient);

        var act = () => resolver.ResolveAsync("urn:tei:uuid:empty.com:some-id");
        await act.Should().ThrowAsync<TeaDiscoveryException>()
            .WithMessage("*No endpoints*");
    }
}
