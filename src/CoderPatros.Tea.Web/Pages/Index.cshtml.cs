// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoderPatros.Tea.Web.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string? BaseUrl { get; set; }

    [BindProperty]
    public string? Token { get; set; }

    public string? ServerStatus { get; set; }

    public void OnGet()
    {
        BaseUrl = HttpContext.Session.GetString("Tea:BaseUrl");
        Token = HttpContext.Session.GetString("Tea:Token");
        ServerStatus = string.IsNullOrWhiteSpace(BaseUrl) ? null : BaseUrl;
    }

    public IActionResult OnPostSettings()
    {
        if (!string.IsNullOrWhiteSpace(BaseUrl))
        {
            HttpContext.Session.SetString("Tea:BaseUrl", BaseUrl.Trim());
        }
        else
        {
            HttpContext.Session.Remove("Tea:BaseUrl");
        }

        if (!string.IsNullOrWhiteSpace(Token))
        {
            HttpContext.Session.SetString("Tea:Token", Token.Trim());
        }
        else
        {
            HttpContext.Session.Remove("Tea:Token");
        }

        return RedirectToPage();
    }

    public IActionResult OnPostLookup(string entityType, string uuid)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            return RedirectToPage();
        }

        return entityType switch
        {
            "product" => RedirectToPage("/Products/Details", new { uuid }),
            "productRelease" => RedirectToPage("/Releases/ProductRelease", new { uuid }),
            "component" => RedirectToPage("/Components/Details", new { uuid }),
            "componentRelease" => RedirectToPage("/Releases/ComponentRelease", new { uuid }),
            "artifact" => RedirectToPage("/Artifacts/Details", new { uuid }),
            _ => RedirectToPage()
        };
    }
}
