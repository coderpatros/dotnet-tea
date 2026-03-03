// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Tea.Client.Models;
using Tea.Client.Models.Cle;
using Tea.Client.Models.Discovery;

namespace Tea.Client;

public interface ITeaClient
{
    // Products
    Task<TeaProduct> GetProductAsync(string uuid, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TeaProduct>> QueryProductsAsync(IdentifierType? idType = null, string? idValue = null, int? pageOffset = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<CleData> GetProductCleAsync(string uuid, CancellationToken cancellationToken = default);

    // Product Releases
    Task<TeaProductRelease> GetProductReleaseAsync(string uuid, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TeaProductRelease>> GetProductReleasesAsync(string productUuid, int? pageOffset = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TeaProductRelease>> QueryProductReleasesAsync(IdentifierType? idType = null, string? idValue = null, int? pageOffset = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<CleData> GetProductReleaseCleAsync(string uuid, CancellationToken cancellationToken = default);

    // Product Release Collections
    Task<TeaCollection> GetProductReleaseLatestCollectionAsync(string uuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeaCollection>> GetProductReleaseCollectionsAsync(string uuid, CancellationToken cancellationToken = default);
    Task<TeaCollection> GetProductReleaseCollectionAsync(string uuid, int collectionVersion, CancellationToken cancellationToken = default);

    // Components
    Task<TeaComponent> GetComponentAsync(string uuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeaComponentRelease>> GetComponentReleasesAsync(string componentUuid, CancellationToken cancellationToken = default);
    Task<CleData> GetComponentCleAsync(string uuid, CancellationToken cancellationToken = default);

    // Component Releases
    Task<ComponentReleaseWithCollection> GetComponentReleaseAsync(string uuid, CancellationToken cancellationToken = default);
    Task<CleData> GetComponentReleaseCleAsync(string uuid, CancellationToken cancellationToken = default);

    // Component Release Collections
    Task<TeaCollection> GetComponentReleaseLatestCollectionAsync(string uuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeaCollection>> GetComponentReleaseCollectionsAsync(string uuid, CancellationToken cancellationToken = default);
    Task<TeaCollection> GetComponentReleaseCollectionAsync(string uuid, int collectionVersion, CancellationToken cancellationToken = default);

    // Artifacts
    Task<TeaArtifact> GetArtifactAsync(string uuid, CancellationToken cancellationToken = default);

    // Discovery
    Task<IReadOnlyList<DiscoveryInfo>> DiscoverByTeiAsync(string tei, CancellationToken cancellationToken = default);
}
