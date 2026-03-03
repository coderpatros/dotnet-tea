// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class GetProductReleasesCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Product UUID" };
        var pageOffsetOption = new Option<int?>("--page-offset") { Description = "Page offset for pagination" };
        var pageSizeOption = new Option<int?>("--page-size") { Description = "Page size for pagination" };

        var command = new Command("get-product-releases") { Description = "Get releases for a product" };
        command.Arguments.Add(uuidArg);
        command.Options.Add(pageOffsetOption);
        command.Options.Add(pageSizeOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var result = await client.GetProductReleasesAsync(
                parseResult.GetValue(uuidArg)!,
                parseResult.GetValue(pageOffsetOption),
                parseResult.GetValue(pageSizeOption),
                ct);

            formatter.WriteProductReleases(result);
        });

        return command;
    }
}
