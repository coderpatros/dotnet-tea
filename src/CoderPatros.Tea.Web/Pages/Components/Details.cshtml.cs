// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages.Components;

public class DetailsModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public DetailsModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public TeaComponent? Component { get; set; }
    public IReadOnlyList<TeaComponentRelease>? Releases { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Uuid { get; set; } = "";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Components", null),
            ("Details", null)
        };

        try
        {
            var (client, _, _) = _factory.Create(HttpContext.Session);
            Component = await client.GetComponentAsync(Uuid, cancellationToken);

            try
            {
                Releases = await client.GetComponentReleasesAsync(Uuid, cancellationToken);
            }
            catch (TeaApiException)
            {
                // Releases may not be available
            }
        }
        catch (TeaNotFoundException)
        {
            ErrorMessage = $"Component {Uuid} not found.";
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
