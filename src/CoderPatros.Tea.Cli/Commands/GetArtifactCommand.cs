// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class GetArtifactCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Artifact UUID" };

        var command = new Command("get-artifact") { Description = "Get an artifact by UUID" };
        command.Arguments.Add(uuidArg);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var result = await client.GetArtifactAsync(parseResult.GetValue(uuidArg)!, ct);
            formatter.WriteArtifact(result);
        });

        return command;
    }
}
