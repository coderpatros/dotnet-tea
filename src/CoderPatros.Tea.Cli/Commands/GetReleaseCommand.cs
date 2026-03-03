// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class GetReleaseCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Release UUID" };
        var componentOption = new Option<bool>("--component") { Description = "Get a component release instead of a product release" };

        var command = new Command("get-release") { Description = "Get a product or component release by UUID" };
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

            if (parseResult.GetValue(componentOption))
            {
                var result = await client.GetComponentReleaseAsync(uuid, ct);
                formatter.WriteComponentReleaseWithCollection(result);
            }
            else
            {
                var result = await client.GetProductReleaseAsync(uuid, ct);
                formatter.WriteProductRelease(result);
            }
        });

        return command;
    }
}
