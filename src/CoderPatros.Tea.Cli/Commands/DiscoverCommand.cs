// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class DiscoverCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var teiArg = new Argument<string>("tei") { Description = "TEI URN to discover (e.g. urn:tei:purl:example.com:pkg:npm/express)" };
        var quietOption = new Option<bool>("--quiet") { Description = "Only output server URLs" };

        var command = new Command("discover") { Description = "Discover TEA servers for a TEI" };
        command.Arguments.Add(teiArg);
        command.Options.Add(quietOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var tei = parseResult.GetValue(teiArg)!;
            var quiet = parseResult.GetValue(quietOption);
            var json = parseResult.GetValue(globals.Json);

            var formatter = Program.CreateFormatter(json);
            var resolver = Program.CreateTeiResolver(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var results = await resolver.ResolveAsync(tei, ct);

            if (quiet && !json)
            {
                foreach (var info in results)
                    foreach (var server in info.Servers)
                        Console.WriteLine(server.RootUrl);
            }
            else
            {
                formatter.WriteDiscoveryResults(results);
            }
        });

        return command;
    }
}
