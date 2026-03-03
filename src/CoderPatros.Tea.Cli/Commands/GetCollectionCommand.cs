// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class GetCollectionCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Release UUID" };
        var versionOption = new Option<int?>("--version") { Description = "Collection version (omit for latest)" };
        var componentOption = new Option<bool>("--component") { Description = "Get collection for a component release instead of a product release" };

        var command = new Command("get-collection") { Description = "Get the collection for a product or component release" };
        command.Arguments.Add(uuidArg);
        command.Options.Add(versionOption);
        command.Options.Add(componentOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var uuid = parseResult.GetValue(uuidArg)!;
            var version = parseResult.GetValue(versionOption);
            var component = parseResult.GetValue(componentOption);

            if (version is not null)
            {
                var result = component
                    ? await client.GetComponentReleaseCollectionAsync(uuid, version.Value, ct)
                    : await client.GetProductReleaseCollectionAsync(uuid, version.Value, ct);
                formatter.WriteCollection(result);
            }
            else
            {
                var result = component
                    ? await client.GetComponentReleaseLatestCollectionAsync(uuid, ct)
                    : await client.GetProductReleaseLatestCollectionAsync(uuid, ct);
                formatter.WriteCollection(result);
            }
        });

        return command;
    }
}
