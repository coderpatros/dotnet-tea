// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class ListCollectionsCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Release UUID" };
        var componentOption = new Option<bool>("--component") { Description = "List collections for a component release instead of a product release" };

        var command = new Command("list-collections") { Description = "List collections for a product or component release" };
        command.Arguments.Add(uuidArg);
        command.Options.Add(componentOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var uuid = parseResult.GetValue(uuidArg)!;
            var result = parseResult.GetValue(componentOption)
                ? await client.GetComponentReleaseCollectionsAsync(uuid, ct)
                : await client.GetProductReleaseCollectionsAsync(uuid, ct);

            formatter.WriteCollections(result);
        });

        return command;
    }
}
