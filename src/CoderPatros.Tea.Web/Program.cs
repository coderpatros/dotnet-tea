// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client;
using CoderPatros.Tea.Client.Extensions.DependencyInjection;
using CoderPatros.Tea.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddTeaClient(options =>
{
    var section = builder.Configuration.GetSection("Tea");
    var baseAddress = section["BaseAddress"];
    if (!string.IsNullOrWhiteSpace(baseAddress))
    {
        options.BaseAddress = new Uri(baseAddress);
    }

    options.BearerToken = section["BearerToken"];
});

builder.Services.AddSingleton<ITeaClientFactory, TeaClientFactory>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();

app.Run();

public partial class Program { }
