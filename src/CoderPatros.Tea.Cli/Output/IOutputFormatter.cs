// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;

namespace CoderPatros.Tea.Cli.Output;

public interface IOutputFormatter
{
    void WriteDiscoveryResults(IReadOnlyList<DiscoveryInfo> results);
    void WriteProduct(TeaProduct product);
    void WriteProducts(PaginatedResponse<TeaProduct> response);
    void WriteProductRelease(TeaProductRelease release);
    void WriteProductReleases(PaginatedResponse<TeaProductRelease> response);
    void WriteComponentRelease(TeaComponentRelease release);
    void WriteComponentReleaseWithCollection(ComponentReleaseWithCollection releaseWithCollection);
    void WriteComponent(TeaComponent component);
    void WriteComponentReleases(IReadOnlyList<TeaComponentRelease> releases);
    void WriteCollection(TeaCollection collection);
    void WriteCollections(IReadOnlyList<TeaCollection> collections);
    void WriteArtifact(TeaArtifact artifact);
    void WriteCle(CleData cle);
    void WriteMessage(string message);
}
