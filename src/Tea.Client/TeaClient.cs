// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tea.Client.Exceptions;
using Tea.Client.Models;
using Tea.Client.Models.Cle;
using Tea.Client.Models.Discovery;
using Tea.Client.Serialization;

namespace Tea.Client;

public sealed class TeaClient : ITeaClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TeaClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = TeaJsonSerializerOptions.Default;
    }

    public TeaClient(HttpClient httpClient, TeaClientOptions options)
        : this(httpClient)
    {
        if (options?.BaseAddress is not null)
        {
            _httpClient.BaseAddress = options.BaseAddress;
        }
    }

    // Products

    public async Task<TeaProduct> GetProductAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaProduct>($"product/{uuid}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<PaginatedResponse<TeaProduct>> QueryProductsAsync(
        IdentifierType? idType = null, string? idValue = null,
        int? pageOffset = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildQueryUrl("products", idType, idValue, pageOffset, pageSize);
        return await GetAsync<PaginatedResponse<TeaProduct>>(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<CleData> GetProductCleAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CleData>($"product/{uuid}/cle", cancellationToken).ConfigureAwait(false);
    }

    // Product Releases

    public async Task<TeaProductRelease> GetProductReleaseAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaProductRelease>($"productRelease/{uuid}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<PaginatedResponse<TeaProductRelease>> GetProductReleasesAsync(
        string productUuid, int? pageOffset = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildPaginatedUrl($"product/{productUuid}/releases", pageOffset, pageSize);
        return await GetAsync<PaginatedResponse<TeaProductRelease>>(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PaginatedResponse<TeaProductRelease>> QueryProductReleasesAsync(
        IdentifierType? idType = null, string? idValue = null,
        int? pageOffset = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildQueryUrl("productReleases", idType, idValue, pageOffset, pageSize);
        return await GetAsync<PaginatedResponse<TeaProductRelease>>(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<CleData> GetProductReleaseCleAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CleData>($"productRelease/{uuid}/cle", cancellationToken).ConfigureAwait(false);
    }

    // Product Release Collections

    public async Task<TeaCollection> GetProductReleaseLatestCollectionAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaCollection>($"productRelease/{uuid}/collection/latest", cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TeaCollection>> GetProductReleaseCollectionsAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<TeaCollection>>($"productRelease/{uuid}/collections", cancellationToken).ConfigureAwait(false);
    }

    public async Task<TeaCollection> GetProductReleaseCollectionAsync(string uuid, int collectionVersion, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaCollection>($"productRelease/{uuid}/collection/{collectionVersion}", cancellationToken).ConfigureAwait(false);
    }

    // Components

    public async Task<TeaComponent> GetComponentAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaComponent>($"component/{uuid}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TeaComponentRelease>> GetComponentReleasesAsync(string componentUuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<TeaComponentRelease>>($"component/{componentUuid}/releases", cancellationToken).ConfigureAwait(false);
    }

    public async Task<CleData> GetComponentCleAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CleData>($"component/{uuid}/cle", cancellationToken).ConfigureAwait(false);
    }

    // Component Releases

    public async Task<ComponentReleaseWithCollection> GetComponentReleaseAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<ComponentReleaseWithCollection>($"componentRelease/{uuid}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<CleData> GetComponentReleaseCleAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CleData>($"componentRelease/{uuid}/cle", cancellationToken).ConfigureAwait(false);
    }

    // Component Release Collections

    public async Task<TeaCollection> GetComponentReleaseLatestCollectionAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaCollection>($"componentRelease/{uuid}/collection/latest", cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TeaCollection>> GetComponentReleaseCollectionsAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<TeaCollection>>($"componentRelease/{uuid}/collections", cancellationToken).ConfigureAwait(false);
    }

    public async Task<TeaCollection> GetComponentReleaseCollectionAsync(string uuid, int collectionVersion, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaCollection>($"componentRelease/{uuid}/collection/{collectionVersion}", cancellationToken).ConfigureAwait(false);
    }

    // Artifacts

    public async Task<TeaArtifact> GetArtifactAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TeaArtifact>($"artifact/{uuid}", cancellationToken).ConfigureAwait(false);
    }

    // Discovery

    public async Task<IReadOnlyList<DiscoveryInfo>> DiscoverByTeiAsync(string tei, CancellationToken cancellationToken = default)
    {
        var encodedTei = Uri.EscapeDataString(tei);
        return await GetAsync<List<DiscoveryInfo>>($"discovery?tei={encodedTei}", cancellationToken).ConfigureAwait(false);
    }

    // Internal helpers

    private async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);

        var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return result ?? throw new TeaApiException(response.StatusCode, null, "Response deserialized to null");
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new TeaBadRequestException(body);

            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                throw new TeaAuthenticationException(response.StatusCode, body);

            case HttpStatusCode.NotFound:
                ErrorResponse? errorResponse = null;
                try
                {
                    errorResponse = JsonSerializer.Deserialize<ErrorResponse>(body, _jsonOptions);
                }
                catch (JsonException)
                {
                    // body wasn't a valid ErrorResponse, that's fine
                }
                throw new TeaNotFoundException(body, errorResponse);

            default:
                throw new TeaApiException(response.StatusCode, body);
        }
    }

    private static string BuildPaginatedUrl(string basePath, int? pageOffset, int? pageSize)
    {
        var queryParams = new List<string>();
        if (pageOffset.HasValue) queryParams.Add($"pageOffset={pageOffset.Value}");
        if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

        return queryParams.Count > 0
            ? $"{basePath}?{string.Join("&", queryParams)}"
            : basePath;
    }

    private static string BuildQueryUrl(string basePath, IdentifierType? idType, string? idValue, int? pageOffset, int? pageSize)
    {
        var queryParams = new List<string>();
        if (idType.HasValue) queryParams.Add($"idType={idType.Value}");
        if (idValue is not null) queryParams.Add($"idValue={Uri.EscapeDataString(idValue)}");
        if (pageOffset.HasValue) queryParams.Add($"pageOffset={pageOffset.Value}");
        if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

        return queryParams.Count > 0
            ? $"{basePath}?{string.Join("&", queryParams)}"
            : basePath;
    }
}
