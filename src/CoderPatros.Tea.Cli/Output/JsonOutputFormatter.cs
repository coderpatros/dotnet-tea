// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Nodes;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;
using CoderPatros.Tea.Client.Serialization;

namespace CoderPatros.Tea.Cli.Output;

public sealed class JsonOutputFormatter : IOutputFormatter
{
    private static readonly JsonSerializerOptions Options = new(TeaJsonSerializerOptions.Default)
    {
        WriteIndented = true
    };

    private static void Write<T>(T value)
    {
        Console.WriteLine(JsonSerializer.Serialize(value, Options));
    }

    public void WriteDiscoveryResults(IReadOnlyList<DiscoveryInfo> results) => Write(results);
    public void WriteProduct(TeaProduct product) => Write(product);
    public void WriteProducts(PaginatedResponse<TeaProduct> response) => Write(response);
    public void WriteProductRelease(TeaProductRelease release) => Write(release);
    public void WriteProductReleases(PaginatedResponse<TeaProductRelease> response) => Write(response);
    public void WriteComponentRelease(TeaComponentRelease release) => Write(release);
    public void WriteComponentReleaseWithCollection(ComponentReleaseWithCollection releaseWithCollection) => Write(releaseWithCollection);
    public void WriteComponent(TeaComponent component) => Write(component);
    public void WriteComponentReleases(IReadOnlyList<TeaComponentRelease> releases) => Write(releases);
    public void WriteCollection(TeaCollection collection) => Write(collection);
    public void WriteCollections(IReadOnlyList<TeaCollection> collections) => Write(collections);
    public void WriteArtifact(TeaArtifact artifact) => Write(artifact);
    public void WriteCle(CleData cle) => Write(cle);
    public void WriteMessage(string message)
    {
        var obj = new JsonObject { ["message"] = message };
        Console.WriteLine(obj.ToJsonString(Options));
    }
}
