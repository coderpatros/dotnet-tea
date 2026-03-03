// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages.Collections;

public class DetailsModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public DetailsModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public TeaCollection? Collection { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Uuid { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public bool Component { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Version { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Collections", null),
            ("Details", null)
        };

        try
        {
            var (client, _, _) = _factory.Create(HttpContext.Session);
            if (Version is not null)
            {
                Collection = Component
                    ? await client.GetComponentReleaseCollectionAsync(Uuid, Version.Value, cancellationToken)
                    : await client.GetProductReleaseCollectionAsync(Uuid, Version.Value, cancellationToken);
            }
            else
            {
                Collection = Component
                    ? await client.GetComponentReleaseLatestCollectionAsync(Uuid, cancellationToken)
                    : await client.GetProductReleaseLatestCollectionAsync(Uuid, cancellationToken);
            }
        }
        catch (TeaNotFoundException)
        {
            ErrorMessage = $"Collection not found for release {Uuid}.";
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
