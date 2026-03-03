// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Cli.Output;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;
using FluentAssertions;
using Spectre.Console;

namespace CoderPatros.Tea.Cli.Tests.Output;

[Collection("ConsoleOutput")]
public class TableOutputFormatterTests : IDisposable
{
    private readonly TableOutputFormatter _formatter = new();
    private readonly StringWriter _writer = new();
    private readonly IAnsiConsole _originalConsole;

    public TableOutputFormatterTests()
    {
        _originalConsole = AnsiConsole.Console;
        AnsiConsole.Console = Spectre.Console.AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new AnsiConsoleOutput(_writer)
        });
    }

    public void Dispose()
    {
        AnsiConsole.Console = _originalConsole;
        _writer.Dispose();
    }

    [Fact]
    public void WriteProduct_ContainsUuidAndName()
    {
        var product = new TeaProduct("test-uuid-123", "My Product", []);

        _formatter.WriteProduct(product);

        var output = _writer.ToString();
        output.Should().Contain("test-uuid-123");
        output.Should().Contain("My Product");
    }

    [Fact]
    public void WriteProduct_WithIdentifiers_ContainsIdentifiers()
    {
        var product = new TeaProduct("uuid-1", "Product",
            [new Identifier(IdentifierType.PURL, "pkg:npm/express")]);

        _formatter.WriteProduct(product);

        var output = _writer.ToString();
        output.Should().Contain("pkg:npm/express");
    }

    [Fact]
    public void WriteMessage_WritesMessageText()
    {
        _formatter.WriteMessage("Operation completed successfully");

        var output = _writer.ToString();
        output.Should().Contain("Operation completed successfully");
    }

    [Fact]
    public void WriteMessage_EscapesSpectreMarkup()
    {
        _formatter.WriteMessage("Value with [brackets] inside");

        // Should not throw and should contain the text (markup-escaped)
        var output = _writer.ToString();
        output.Should().Contain("brackets");
    }

    [Fact]
    public void WriteComponent_ContainsUuidAndName()
    {
        var component = new TeaComponent("comp-uuid", "My Component", []);

        _formatter.WriteComponent(component);

        var output = _writer.ToString();
        output.Should().Contain("comp-uuid");
        output.Should().Contain("My Component");
    }

    [Fact]
    public void WriteProductRelease_ContainsVersionAndUuid()
    {
        var release = new TeaProductRelease(
            "rel-uuid", "1.0.0", DateTimeOffset.UtcNow, []);

        _formatter.WriteProductRelease(release);

        var output = _writer.ToString();
        output.Should().Contain("rel-uuid");
        output.Should().Contain("1.0.0");
    }

    [Fact]
    public void WriteProducts_ContainsPaginationInfo()
    {
        var response = new PaginatedResponse<TeaProduct>(
            DateTimeOffset.UtcNow, 0, 10, 42,
            [new TeaProduct("uuid-1", "Product 1", [])]);

        _formatter.WriteProducts(response);

        var output = _writer.ToString();
        output.Should().Contain("42");
        output.Should().Contain("Product 1");
    }

    [Fact]
    public void WriteCollection_ContainsUuidAndVersion()
    {
        var collection = new TeaCollection(Uuid: "col-uuid", Version: 3);

        _formatter.WriteCollection(collection);

        var output = _writer.ToString();
        output.Should().Contain("col-uuid");
        output.Should().Contain("3");
    }

    [Fact]
    public void WriteArtifact_ContainsUuidAndName()
    {
        var artifact = new TeaArtifact(Uuid: "art-uuid", Name: "sbom.json");

        _formatter.WriteArtifact(artifact);

        var output = _writer.ToString();
        output.Should().Contain("art-uuid");
        output.Should().Contain("sbom.json");
    }

    [Fact]
    public void WriteCle_ContainsEventInfo()
    {
        var cle = new CleData(
            Events: [new CleEvent(1, CleEventType.Released, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, Version: "1.0.0")]);

        _formatter.WriteCle(cle);

        var output = _writer.ToString();
        output.Should().Contain("Released");
        output.Should().Contain("1.0.0");
    }

    [Fact]
    public void WriteDiscoveryResults_ContainsServerInfo()
    {
        var results = new List<DiscoveryInfo>
        {
            new("pr-uuid-123", [new TeaServerInfo("https://tea.example.com/tea/v1/", ["0.3.0"], 1.0)])
        };

        _formatter.WriteDiscoveryResults(results);

        var output = _writer.ToString();
        output.Should().Contain("pr-uuid-123");
        output.Should().Contain("https://tea.example.com/tea/v1/");
    }

    [Fact]
    public void WriteComponentReleases_ContainsReleaseInfo()
    {
        var releases = new List<TeaComponentRelease>
        {
            new("cr-1", "1.0.0", DateTimeOffset.UtcNow, ComponentName: "MyLib")
        };

        _formatter.WriteComponentReleases(releases);

        var output = _writer.ToString();
        output.Should().Contain("cr-1");
        output.Should().Contain("1.0.0");
        output.Should().Contain("MyLib");
    }

    [Fact]
    public void WriteCollections_ContainsCollectionInfo()
    {
        var collections = new List<TeaCollection>
        {
            new(Uuid: "col-1", Version: 1),
            new(Uuid: "col-2", Version: 2)
        };

        _formatter.WriteCollections(collections);

        var output = _writer.ToString();
        output.Should().Contain("col-1");
        output.Should().Contain("col-2");
    }
}
