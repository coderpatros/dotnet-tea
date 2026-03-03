// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoderPatros.Tea.Web.Tests;

public class BasicPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("TEA Explorer");
    }

    [Fact]
    public async Task HomePage_ContainsSearchForms()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Search Products");
        content.Should().Contain("Search Releases");
        content.Should().Contain("Discover / Inspect TEI");
        content.Should().Contain("Direct Lookup");
        content.Should().Contain("Server Settings");
    }

    [Fact]
    public async Task HomePage_ContainsIdentifierTypeOptions()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("CPE");
        content.Should().Contain("PURL");
        content.Should().Contain("TEI");
    }

    [Fact]
    public async Task ProductsSearch_WithoutQuery_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Products/Search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReleasesSearch_WithoutQuery_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Releases/Search");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Discover_WithoutTei_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Discover");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Inspect_WithoutTei_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Inspect");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ErrorPage_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Error");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/Products/Details/00000000-0000-0000-0000-000000000000")]
    [InlineData("/Releases/ProductRelease/00000000-0000-0000-0000-000000000000")]
    [InlineData("/Releases/ComponentRelease/00000000-0000-0000-0000-000000000000")]
    [InlineData("/Components/Details/00000000-0000-0000-0000-000000000000")]
    [InlineData("/Collections/Details/00000000-0000-0000-0000-000000000000")]
    [InlineData("/Artifacts/Details/00000000-0000-0000-0000-000000000000")]
    public async Task DetailPages_WithNoServer_ShowConnectionError(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("alert");
    }

    [Fact]
    public async Task CleDetails_WithNoServer_ShowsError()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Cle/Details?uuid=00000000-0000-0000-0000-000000000000&entity=product");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("alert");
    }

    [Fact]
    public async Task Navbar_ShowsNoServerWhenNotConfigured()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("No server configured");
    }
}
