// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client.Discovery;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models.Discovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages;

public class DiscoverModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public DiscoverModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<DiscoveryInfo>? Results { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tei { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Tei))
        {
            return;
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Discover", null)
        };

        try
        {
            var (_, resolver, _) = _factory.Create(HttpContext.Session);
            Results = await resolver.ResolveAsync(Tei, cancellationToken);
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
