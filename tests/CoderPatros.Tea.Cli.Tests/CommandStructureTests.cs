// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;
using CoderPatros.Tea.Cli;
using CoderPatros.Tea.Cli.Commands;
using FluentAssertions;

namespace CoderPatros.Tea.Cli.Tests;

public class CommandStructureTests
{
    private readonly RootCommand _rootCommand;
    private readonly GlobalOptions _globalOptions;

    public CommandStructureTests()
    {
        var baseUrlOption = new Option<string?>("--base-url") { Recursive = true };
        var domainOption = new Option<string?>("--domain") { Recursive = true };
        var tokenOption = new Option<string?>("--token") { Recursive = true };
        var jsonOption = new Option<bool>("--json") { Recursive = true };
        var timeoutOption = new Option<int>("--timeout") { DefaultValueFactory = _ => 30, Recursive = true };

        _globalOptions = new GlobalOptions(baseUrlOption, domainOption, tokenOption, jsonOption, timeoutOption);

        _rootCommand = new RootCommand("CLI tool for the Transparency Exchange API (TEA)");
        _rootCommand.Options.Add(baseUrlOption);
        _rootCommand.Options.Add(domainOption);
        _rootCommand.Options.Add(tokenOption);
        _rootCommand.Options.Add(jsonOption);
        _rootCommand.Options.Add(timeoutOption);

        _rootCommand.Subcommands.Add(DiscoverCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(SearchProductsCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(SearchReleasesCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetProductCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetReleaseCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetCollectionCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetProductReleasesCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetComponentCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetComponentReleasesCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(ListCollectionsCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetArtifactCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(GetCleCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(DownloadCommand.Create(_globalOptions));
        _rootCommand.Subcommands.Add(InspectCommand.Create(_globalOptions));
    }

    [Fact]
    public void RootCommand_HasAll14Subcommands()
    {
        _rootCommand.Subcommands.Should().HaveCount(14);
    }

    [Theory]
    [InlineData("discover")]
    [InlineData("search-products")]
    [InlineData("search-releases")]
    [InlineData("get-product")]
    [InlineData("get-release")]
    [InlineData("get-collection")]
    [InlineData("get-product-releases")]
    [InlineData("get-component")]
    [InlineData("get-component-releases")]
    [InlineData("list-collections")]
    [InlineData("get-artifact")]
    [InlineData("get-cle")]
    [InlineData("download")]
    [InlineData("inspect")]
    public void RootCommand_HasSubcommandNamed(string name)
    {
        _rootCommand.Subcommands.Should().Contain(c => c.Name == name);
    }

    [Theory]
    [InlineData("--base-url")]
    [InlineData("--domain")]
    [InlineData("--token")]
    [InlineData("--json")]
    [InlineData("--timeout")]
    public void RootCommand_HasGlobalOption(string name)
    {
        _rootCommand.Options.Should().Contain(o => o.Name == name);
    }

    [Fact]
    public void GlobalOptions_AreRecursive()
    {
        var globalOptionNames = new[] { "--base-url", "--domain", "--token", "--json", "--timeout" };
        foreach (var name in globalOptionNames)
        {
            var option = _rootCommand.Options.Single(o => o.Name == name);
            option.Recursive.Should().BeTrue($"option {option.Name} should be recursive");
        }
    }

    [Fact]
    public void TimeoutOption_DefaultsTo30()
    {
        var parseResult = _rootCommand.Parse("get-product some-uuid --base-url https://example.com");
        var timeoutValue = parseResult.GetValue(_globalOptions.Timeout);
        timeoutValue.Should().Be(30);
    }

    [Fact]
    public void DiscoverCommand_HasTeiArgument()
    {
        var cmd = GetSubcommand("discover");
        cmd.Arguments.Should().ContainSingle(a => a.Name == "tei");
    }

    [Fact]
    public void DiscoverCommand_HasQuietOption()
    {
        var cmd = GetSubcommand("discover");
        cmd.Options.Should().Contain(o => o.Name == "--quiet");
    }

    [Fact]
    public void GetProductCommand_HasUuidArgument()
    {
        var cmd = GetSubcommand("get-product");
        cmd.Arguments.Should().ContainSingle(a => a.Name == "uuid");
    }

    [Fact]
    public void GetReleaseCommand_HasComponentOption()
    {
        var cmd = GetSubcommand("get-release");
        cmd.Options.Should().Contain(o => o.Name == "--component");
    }

    [Fact]
    public void GetCollectionCommand_HasVersionAndComponentOptions()
    {
        var cmd = GetSubcommand("get-collection");
        cmd.Options.Should().Contain(o => o.Name == "--version");
        cmd.Options.Should().Contain(o => o.Name == "--component");
    }

    [Fact]
    public void SearchProductsCommand_HasSearchOptions()
    {
        var cmd = GetSubcommand("search-products");
        cmd.Options.Should().Contain(o => o.Name == "--id-type");
        cmd.Options.Should().Contain(o => o.Name == "--id-value");
        cmd.Options.Should().Contain(o => o.Name == "--page-offset");
        cmd.Options.Should().Contain(o => o.Name == "--page-size");
    }

    [Fact]
    public void SearchReleasesCommand_HasSearchOptions()
    {
        var cmd = GetSubcommand("search-releases");
        cmd.Options.Should().Contain(o => o.Name == "--id-type");
        cmd.Options.Should().Contain(o => o.Name == "--id-value");
        cmd.Options.Should().Contain(o => o.Name == "--page-offset");
        cmd.Options.Should().Contain(o => o.Name == "--page-size");
    }

    [Fact]
    public void GetProductReleasesCommand_HasPaginationOptions()
    {
        var cmd = GetSubcommand("get-product-releases");
        cmd.Options.Should().Contain(o => o.Name == "--page-offset");
        cmd.Options.Should().Contain(o => o.Name == "--page-size");
    }

    [Fact]
    public void GetCleCommand_HasRequiredEntityOption()
    {
        var cmd = GetSubcommand("get-cle");
        var entityOption = cmd.Options.Single(o => o.Name == "--entity");
        entityOption.Required.Should().BeTrue();
    }

    [Theory]
    [InlineData("product")]
    [InlineData("product-release")]
    [InlineData("component")]
    [InlineData("component-release")]
    public void GetCleCommand_EntityAcceptsValidValue(string entityType)
    {
        var parseResult = _rootCommand.Parse($"get-cle some-uuid --entity {entityType} --base-url https://example.com");
        parseResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void GetCleCommand_EntityRejectsInvalidValue()
    {
        var parseResult = _rootCommand.Parse("get-cle some-uuid --entity invalid --base-url https://example.com");
        parseResult.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void DownloadCommand_HasUrlAndDestArguments()
    {
        var cmd = GetSubcommand("download");
        cmd.Arguments.Should().Contain(a => a.Name == "url");
        cmd.Arguments.Should().Contain(a => a.Name == "dest");
    }

    [Fact]
    public void DownloadCommand_HasChecksumOption()
    {
        var cmd = GetSubcommand("download");
        cmd.Options.Should().Contain(o => o.Name == "--checksum");
    }

    [Fact]
    public void InspectCommand_HasTeiArgument()
    {
        var cmd = GetSubcommand("inspect");
        cmd.Arguments.Should().ContainSingle(a => a.Name == "tei");
    }

    [Fact]
    public void InspectCommand_HasMaxComponentsOption()
    {
        var cmd = GetSubcommand("inspect");
        cmd.Options.Should().Contain(o => o.Name == "--max-components");
    }

    [Fact]
    public void ListCollectionsCommand_HasComponentOption()
    {
        var cmd = GetSubcommand("list-collections");
        cmd.Options.Should().Contain(o => o.Name == "--component");
    }

    private Command GetSubcommand(string name)
    {
        return _rootCommand.Subcommands.Single(c => c.Name == name);
    }
}
