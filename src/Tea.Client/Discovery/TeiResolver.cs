// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net.Http.Json;
using Tea.Client.Exceptions;
using Tea.Client.Models.Discovery;
using Tea.Client.Serialization;

namespace Tea.Client.Discovery;

public sealed class TeiResolver : ITeiResolver
{
    private readonly HttpClient _httpClient;
    private readonly string _supportedVersion;

    public TeiResolver(HttpClient httpClient, string supportedVersion = "0.3.0-beta.2")
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _supportedVersion = supportedVersion;
    }

    public async Task<IReadOnlyList<DiscoveryInfo>> ResolveAsync(string tei, CancellationToken cancellationToken = default)
    {
        var parsed = Tei.Parse(tei);

        // Step 1: Fetch .well-known/tea from domain
        var wellKnownUrl = $"https://{parsed.Domain}/.well-known/tea";
        WellKnownResponse wellKnown;
        try
        {
            wellKnown = await _httpClient.GetFromJsonAsync<WellKnownResponse>(
                wellKnownUrl, TeaJsonSerializerOptions.Default, cancellationToken).ConfigureAwait(false)
                ?? throw new TeaDiscoveryException($"Empty .well-known/tea response from {parsed.Domain}");
        }
        catch (HttpRequestException ex)
        {
            throw new TeaDiscoveryException($"Failed to fetch .well-known/tea from {parsed.Domain}", ex);
        }

        if (wellKnown.Endpoints.Count == 0)
            throw new TeaDiscoveryException($"No endpoints found in .well-known/tea response from {parsed.Domain}");

        // Step 2: Select best endpoint (highest priority that supports our version)
        var endpoint = SelectBestEndpoint(wellKnown.Endpoints);
        if (endpoint is null)
            throw new TeaDiscoveryException($"No compatible endpoint found in .well-known/tea response from {parsed.Domain}");

        // Step 3: Find the best matching version
        var selectedVersion = SelectBestVersion(endpoint.Versions);
        if (selectedVersion is null)
            throw new TeaDiscoveryException($"No compatible version found at endpoint {endpoint.Url}");

        // Step 4: Call discovery endpoint
        var encodedTei = Uri.EscapeDataString(tei);
        var discoveryUrl = $"{endpoint.Url.TrimEnd('/')}/v{selectedVersion}/discovery?tei={encodedTei}";

        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<DiscoveryInfo>>(
                discoveryUrl, TeaJsonSerializerOptions.Default, cancellationToken).ConfigureAwait(false)
                ?? throw new TeaDiscoveryException($"Empty discovery response from {discoveryUrl}");

            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new TeaDiscoveryException($"Discovery request failed for TEI '{tei}' at {discoveryUrl}", ex);
        }
    }

    private WellKnownEndpoint? SelectBestEndpoint(IReadOnlyList<WellKnownEndpoint> endpoints)
    {
        return endpoints
            .Where(e => e.Versions.Any(v => IsVersionCompatible(v)))
            .OrderByDescending(e => e.Priority ?? 1.0)
            .ThenByDescending(e => e.Versions.Where(IsVersionCompatible).Max())
            .FirstOrDefault();
    }

    private string? SelectBestVersion(IReadOnlyList<string> versions)
    {
        return versions
            .Where(IsVersionCompatible)
            .OrderByDescending(v => v)
            .FirstOrDefault();
    }

    private bool IsVersionCompatible(string version)
    {
        // Simple compatibility: major version must match
        // For pre-1.0, we check exact match of major.minor
        var supported = ParseVersion(_supportedVersion);
        var candidate = ParseVersion(version);

        if (supported.Major == 0)
        {
            // Pre-1.0: major.minor must match
            return candidate.Major == supported.Major && candidate.Minor == supported.Minor;
        }

        // Post-1.0: major must match
        return candidate.Major == supported.Major;
    }

    private static (int Major, int Minor, int Patch) ParseVersion(string version)
    {
        // Strip pre-release suffix for comparison
        var dashIndex = version.IndexOf('-');
        var versionCore = dashIndex >= 0 ? version[..dashIndex] : version;

        var parts = versionCore.Split('.');
        var major = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        var minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
        var patch = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        return (major, minor, patch);
    }
}
