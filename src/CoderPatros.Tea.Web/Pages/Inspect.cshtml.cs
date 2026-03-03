// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Discovery;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Discovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages;

public class InspectModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public InspectModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<DiscoveryInfo>? DiscoveryResults { get; set; }
    public TeaProductRelease? Release { get; set; }
    public TeaCollection? LatestCollection { get; set; }
    public List<ComponentInspection> Components { get; set; } = new();
    public string? ServerUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ReleaseError { get; set; }
    public string? CollectionError { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tei { get; set; }

    public class ComponentInspection
    {
        public string ComponentUuid { get; set; } = "";
        public string? ReleaseUuid { get; set; }
        public TeaComponent? Component { get; set; }
        public ComponentReleaseWithCollection? ReleaseWithCollection { get; set; }
        public IReadOnlyList<TeaComponentRelease>? Releases { get; set; }
        public string? Error { get; set; }
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Tei))
        {
            return;
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Inspect", null)
        };

        try
        {
            var (_, resolver, _) = _factory.Create(HttpContext.Session);

            // Step 1: Discover
            DiscoveryResults = await resolver.ResolveAsync(Tei, cancellationToken);
            if (DiscoveryResults.Count == 0)
            {
                ErrorMessage = "No TEA servers found for this TEI.";
                return;
            }

            // Use first server
            var firstResult = DiscoveryResults[0];
            var server = firstResult.Servers[0];
            var version = server.Versions[0];
            ServerUrl = $"{server.RootUrl.TrimEnd('/')}/v{version}";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ServerUrl.EndsWith('/') ? ServerUrl : ServerUrl + "/");

            var token = HttpContext.Session.GetString("Tea:Token");
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var client = new TeaClient(httpClient);
            var releaseUuid = firstResult.ProductReleaseUuid;

            // Step 2: Get product release
            try
            {
                Release = await client.GetProductReleaseAsync(releaseUuid, cancellationToken);
            }
            catch (Exception ex)
            {
                ReleaseError = $"Failed to get product release {releaseUuid}: {ex.Message}";
                return;
            }

            // Step 3: Get latest collection
            try
            {
                LatestCollection = await client.GetProductReleaseLatestCollectionAsync(releaseUuid, cancellationToken);
            }
            catch (Exception ex)
            {
                CollectionError = $"Failed to get collection: {ex.Message}";
            }

            // Step 4: Inspect components (first 10)
            var componentsToShow = Release.Components.Take(10).ToList();
            foreach (var componentRef in componentsToShow)
            {
                var inspection = new ComponentInspection
                {
                    ComponentUuid = componentRef.Uuid,
                    ReleaseUuid = componentRef.Release
                };

                try
                {
                    if (componentRef.Release is not null)
                    {
                        inspection.ReleaseWithCollection = await client.GetComponentReleaseAsync(componentRef.Release, cancellationToken);
                    }
                    else
                    {
                        inspection.Component = await client.GetComponentAsync(componentRef.Uuid, cancellationToken);
                        inspection.Releases = await client.GetComponentReleasesAsync(componentRef.Uuid, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    inspection.Error = $"Failed to get component {componentRef.Uuid}: {ex.Message}";
                }

                Components.Add(inspection);
            }
        }
        catch (TeaDiscoveryException ex)
        {
            ErrorMessage = $"Discovery failed: {ex.Message}";
        }
        catch (FormatException ex)
        {
            ErrorMessage = $"Invalid TEI URN: {ex.Message}";
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}";
        }
    }
}
