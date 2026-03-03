// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json;
using CoderPatros.Tea.Cli.Output;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;
using FluentAssertions;

namespace CoderPatros.Tea.Cli.Tests.Output;

[Collection("ConsoleOutput")]
public class JsonOutputFormatterTests : IDisposable
{
    private readonly JsonOutputFormatter _formatter = new();
    private readonly StringWriter _writer = new();
    private readonly TextWriter _originalOut;

    public JsonOutputFormatterTests()
    {
        _originalOut = Console.Out;
        Console.SetOut(_writer);
        // Trigger any one-time library warnings (e.g. FluentAssertions license notice)
        // so they don't pollute test output capture
        true.Should().BeTrue();
        _writer.GetStringBuilder().Clear();
    }

    public void Dispose()
    {
        Console.SetOut(_originalOut);
        _writer.Dispose();
    }

    [Fact]
    public void WriteProduct_OutputsValidJson()
    {
        var product = new TeaProduct("test-uuid", "Test Product", []);

        _formatter.WriteProduct(product);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("test-uuid");
        doc.RootElement.GetProperty("name").GetString().Should().Be("Test Product");
    }

    [Fact]
    public void WriteProduct_WithIdentifiers_OutputsIdentifiers()
    {
        var product = new TeaProduct("uuid-1", "My Product",
            [new Identifier(IdentifierType.PURL, "pkg:npm/express")]);

        _formatter.WriteProduct(product);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        var identifiers = doc.RootElement.GetProperty("identifiers");
        identifiers.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public void WriteProducts_IncludesPaginationFields()
    {
        var response = new PaginatedResponse<TeaProduct>(
            DateTimeOffset.UtcNow, 0, 10, 42,
            [new TeaProduct("uuid-1", "Product 1", [])]);

        _formatter.WriteProducts(response);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("pageStartIndex").GetInt64().Should().Be(0);
        doc.RootElement.GetProperty("pageSize").GetInt64().Should().Be(10);
        doc.RootElement.GetProperty("totalResults").GetInt64().Should().Be(42);
        doc.RootElement.GetProperty("results").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public void WriteMessage_WrapsInMessageObject()
    {
        _formatter.WriteMessage("Hello World");

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("message").GetString().Should().Be("Hello World");
    }

    [Fact]
    public void WriteComponent_OutputsValidJson()
    {
        var component = new TeaComponent("comp-uuid", "My Component", []);

        _formatter.WriteComponent(component);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("comp-uuid");
        doc.RootElement.GetProperty("name").GetString().Should().Be("My Component");
    }

    [Fact]
    public void WriteProductRelease_OutputsValidJson()
    {
        var release = new TeaProductRelease(
            "rel-uuid", "1.0.0", DateTimeOffset.UtcNow, []);

        _formatter.WriteProductRelease(release);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("rel-uuid");
        doc.RootElement.GetProperty("version").GetString().Should().Be("1.0.0");
    }

    [Fact]
    public void WriteComponentRelease_OutputsValidJson()
    {
        var release = new TeaComponentRelease(
            "crel-uuid", "2.0.0", DateTimeOffset.UtcNow);

        _formatter.WriteComponentRelease(release);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("crel-uuid");
        doc.RootElement.GetProperty("version").GetString().Should().Be("2.0.0");
    }

    [Fact]
    public void WriteCollection_OutputsValidJson()
    {
        var collection = new TeaCollection(Uuid: "col-uuid", Version: 1);

        _formatter.WriteCollection(collection);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("col-uuid");
        doc.RootElement.GetProperty("version").GetInt32().Should().Be(1);
    }

    [Fact]
    public void WriteArtifact_OutputsValidJson()
    {
        var artifact = new TeaArtifact(Uuid: "art-uuid", Name: "sbom.json");

        _formatter.WriteArtifact(artifact);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("uuid").GetString().Should().Be("art-uuid");
        doc.RootElement.GetProperty("name").GetString().Should().Be("sbom.json");
    }

    [Fact]
    public void WriteCle_OutputsValidJson()
    {
        var cle = new CleData(
            Events: [new CleEvent(1, CleEventType.Released, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Version: "1.0.0")]);

        _formatter.WriteCle(cle);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("events").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public void WriteDiscoveryResults_OutputsValidJsonArray()
    {
        var results = new List<DiscoveryInfo>
        {
            new("pr-uuid", [new TeaServerInfo("https://tea.example.com/tea/v1/", ["0.3.0"], 1.0)])
        };

        _formatter.WriteDiscoveryResults(results);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetArrayLength().Should().Be(1);
        doc.RootElement[0].GetProperty("productReleaseUuid").GetString().Should().Be("pr-uuid");
    }

    [Fact]
    public void WriteCollections_OutputsValidJsonArray()
    {
        var collections = new List<TeaCollection>
        {
            new(Uuid: "col-1", Version: 1),
            new(Uuid: "col-2", Version: 2)
        };

        _formatter.WriteCollections(collections);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void WriteComponentReleases_OutputsValidJsonArray()
    {
        var releases = new List<TeaComponentRelease>
        {
            new("cr-1", "1.0.0", DateTimeOffset.UtcNow),
            new("cr-2", "2.0.0", DateTimeOffset.UtcNow)
        };

        _formatter.WriteComponentReleases(releases);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void WriteComponentReleaseWithCollection_OutputsValidJson()
    {
        var releaseWithCollection = new ComponentReleaseWithCollection(
            new TeaComponentRelease("cr-uuid", "1.0.0", DateTimeOffset.UtcNow),
            new TeaCollection(Uuid: "col-uuid", Version: 1));

        _formatter.WriteComponentReleaseWithCollection(releaseWithCollection);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("release").GetProperty("uuid").GetString().Should().Be("cr-uuid");
        doc.RootElement.GetProperty("latestCollection").GetProperty("uuid").GetString().Should().Be("col-uuid");
    }

    [Fact]
    public void WriteProductReleases_IncludesPaginationFields()
    {
        var response = new PaginatedResponse<TeaProductRelease>(
            DateTimeOffset.UtcNow, 5, 20, 100,
            [new TeaProductRelease("rel-1", "1.0.0", DateTimeOffset.UtcNow, [])]);

        _formatter.WriteProductReleases(response);

        var output = _writer.ToString().Trim();
        var doc = JsonDocument.Parse(output);
        doc.RootElement.GetProperty("pageStartIndex").GetInt64().Should().Be(5);
        doc.RootElement.GetProperty("totalResults").GetInt64().Should().Be(100);
    }
}
