// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models.Cle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages.Cle;

public class DetailsModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public DetailsModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public CleData? CleData { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Uuid { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string Entity { get; set; } = "product";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("CLE", null),
            ("Details", null)
        };

        try
        {
            var (client, _, _) = _factory.Create(HttpContext.Session);
            CleData = Entity switch
            {
                "product" => await client.GetProductCleAsync(Uuid, cancellationToken),
                "productRelease" => await client.GetProductReleaseCleAsync(Uuid, cancellationToken),
                "component" => await client.GetComponentCleAsync(Uuid, cancellationToken),
                "componentRelease" => await client.GetComponentReleaseCleAsync(Uuid, cancellationToken),
                _ => throw new ArgumentException($"Unknown entity type: {Entity}")
            };
        }
        catch (TeaNotFoundException)
        {
            ErrorMessage = $"CLE data not found for {Entity} {Uuid}.";
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
