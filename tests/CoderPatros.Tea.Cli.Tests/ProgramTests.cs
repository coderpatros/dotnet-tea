// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Cli.Output;
using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Discovery;
using FluentAssertions;

namespace CoderPatros.Tea.Cli.Tests;

public class ProgramTests
{
    [Fact]
    public void CreateFormatter_WithFalse_ReturnsTableOutputFormatter()
    {
        var formatter = Program.CreateFormatter(false);
        formatter.Should().BeOfType<TableOutputFormatter>();
    }

    [Fact]
    public void CreateFormatter_WithTrue_ReturnsJsonOutputFormatter()
    {
        var formatter = Program.CreateFormatter(true);
        formatter.Should().BeOfType<JsonOutputFormatter>();
    }

    [Fact]
    public void CreateHttpClient_SetsBaseAddress()
    {
        using var client = Program.CreateHttpClient("https://tea.example.com/tea/v1/", null, 30);
        client.BaseAddress.Should().Be(new Uri("https://tea.example.com/tea/v1/"));
    }

    [Fact]
    public void CreateHttpClient_WithoutBaseUrl_HasNoBaseAddress()
    {
        using var client = Program.CreateHttpClient(null, null, 30);
        client.BaseAddress.Should().BeNull();
    }

    [Fact]
    public void CreateHttpClient_SetsTimeout()
    {
        using var client = Program.CreateHttpClient(null, null, 60);
        client.Timeout.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void CreateTeaClient_ReturnsTeaClient()
    {
        var client = Program.CreateTeaClient("https://tea.example.com/tea/v1/", null, 30);
        client.Should().BeOfType<TeaClient>();
    }

    [Fact]
    public void CreateTeiResolver_ReturnsTeiResolver()
    {
        var resolver = Program.CreateTeiResolver("https://tea.example.com/tea/v1/", null, 30);
        resolver.Should().BeOfType<TeiResolver>();
    }
}
