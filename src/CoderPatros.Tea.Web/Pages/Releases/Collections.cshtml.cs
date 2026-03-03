// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages.Releases;

public class CollectionsModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public CollectionsModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<TeaCollection>? CollectionsList { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Uuid { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public bool Component { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var entityType = Component ? "Component Release" : "Product Release";
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Releases", null),
            (entityType, Component ? $"/Releases/ComponentRelease/{Uuid}" : $"/Releases/ProductRelease/{Uuid}"),
            ("Collections", null)
        };

        try
        {
            var (client, _, _) = _factory.Create(HttpContext.Session);
            CollectionsList = Component
                ? await client.GetComponentReleaseCollectionsAsync(Uuid, cancellationToken)
                : await client.GetProductReleaseCollectionsAsync(Uuid, cancellationToken);
        }
        catch (TeaNotFoundException)
        {
            ErrorMessage = $"Release {Uuid} not found.";
        }
        catch (TeaAuthenticationException)
        {
            ErrorMessage = "Authentication failed. Check your bearer token in Server Settings.";
        }
        catch (TeaApiException ex)
        {
            ErrorMessage = $"API error ({ex.StatusCode}): {ex.Message}";
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}. Check your server URL in Settings.";
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = "No TEA server configured. Set a server URL in Settings on the Home page.";
        }
    }
}
