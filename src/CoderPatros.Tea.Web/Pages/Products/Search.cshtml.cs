// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages.Products;

public class SearchModel : PageModel
{
    private readonly ITeaClientFactory _factory;

    public SearchModel(ITeaClientFactory factory)
    {
        _factory = factory;
    }

    public PaginatedResponse<TeaProduct>? Results { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? IdType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? IdValue { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? PageOffset { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? PageSize { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(IdValue))
        {
            return;
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Products", null),
            ("Search", null)
        };

        try
        {
            var (client, _, _) = _factory.Create(HttpContext.Session);
            var idType = Enum.TryParse<IdentifierType>(IdType, true, out var parsed) ? parsed : (IdentifierType?)null;
            Results = await client.QueryProductsAsync(idType, IdValue, PageOffset, PageSize ?? 20, cancellationToken);
        }
        catch (TeaAuthenticationException)
        {
            ErrorMessage = "Authentication failed. Check your bearer token in Server Settings.";
        }
        catch (TeaBadRequestException ex)
        {
            ErrorMessage = $"Bad request: {ex.ResponseBody}";
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
