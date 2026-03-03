// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;
using CoderPatros.Tea.Client.Models;

namespace CoderPatros.Tea.Cli.Commands;

internal static class SearchProductsCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var idTypeOption = new Option<IdentifierType?>("--id-type") { Description = "Identifier type to search by (CPE, TEI, PURL)" };
        var idValueOption = new Option<string?>("--id-value") { Description = "Identifier value to search for" };
        var pageOffsetOption = new Option<int?>("--page-offset") { Description = "Page offset for pagination" };
        var pageSizeOption = new Option<int?>("--page-size") { Description = "Page size for pagination" };

        var command = new Command("search-products") { Description = "Search for products" };
        command.Options.Add(idTypeOption);
        command.Options.Add(idValueOption);
        command.Options.Add(pageOffsetOption);
        command.Options.Add(pageSizeOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var result = await client.QueryProductsAsync(
                parseResult.GetValue(idTypeOption),
                parseResult.GetValue(idValueOption),
                parseResult.GetValue(pageOffsetOption),
                parseResult.GetValue(pageSizeOption),
                ct);

            formatter.WriteProducts(result);
        });

        return command;
    }
}
