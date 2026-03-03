// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;
using Spectre.Console;

namespace CoderPatros.Tea.Cli.Commands;

internal static class InspectCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var teiArg = new Argument<string>("tei") { Description = "TEI URN to inspect" };
        var maxComponentsOption = new Option<int>("--max-components") { Description = "Maximum number of components to display per release", DefaultValueFactory = _ => 10 };

        var command = new Command("inspect") { Description = "Full inspection: discover TEI, list releases, collections, and artifacts" };
        command.Arguments.Add(teiArg);
        command.Options.Add(maxComponentsOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var tei = parseResult.GetValue(teiArg)!;
            var maxComponents = parseResult.GetValue(maxComponentsOption);
            var json = parseResult.GetValue(globals.Json);
            var token = parseResult.GetValue(globals.Token);
            var timeout = parseResult.GetValue(globals.Timeout);

            var formatter = Program.CreateFormatter(json);
            var resolver = Program.CreateTeiResolver(
                parseResult.GetValue(globals.BaseUrl),
                token,
                timeout);

            // Step 1: Discover
            var discoveryResults = await resolver.ResolveAsync(tei, ct);
            if (discoveryResults.Count == 0)
            {
                Console.Error.WriteLine("No TEA servers found for this TEI.");
                return 1;
            }

            formatter.WriteDiscoveryResults(discoveryResults);

            // Use first server to create a client
            var firstResult = discoveryResults[0];
            var server = firstResult.Servers[0];
            var version = server.Versions[0];
            var serverUrl = $"{server.RootUrl.TrimEnd('/')}/v{version}";
            var client = Program.CreateTeaClient(serverUrl, token, timeout);

            // Step 2: Get product release
            var releaseUuid = firstResult.ProductReleaseUuid;

            if (!json)
                AnsiConsole.WriteLine();

            Client.Models.TeaProductRelease release;
            try
            {
                release = await client.GetProductReleaseAsync(releaseUuid, ct);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to get product release {releaseUuid}: {ex.Message}");
                return 1;
            }

            if (!json)
                AnsiConsole.MarkupLine("[bold]Product Release:[/]");
            formatter.WriteProductRelease(release);

            // Step 3: Get latest collection
            if (!json)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[bold]Latest Collection:[/]");
            }

            try
            {
                var collection = await client.GetProductReleaseLatestCollectionAsync(releaseUuid, ct);
                formatter.WriteCollection(collection);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to get collection: {ex.Message}");
            }

            // Step 4: Inspect components
            var componentsToShow = release.Components.Take(maxComponents).ToList();

            if (componentsToShow.Count > 0 && !json)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[bold]Components ({componentsToShow.Count} of {release.Components.Count}):[/]");
            }

            foreach (var componentRef in componentsToShow)
            {
                if (!json)
                    AnsiConsole.WriteLine();

                try
                {
                    var componentRelease = await client.GetComponentReleaseAsync(componentRef.Uuid, ct);
                    formatter.WriteComponentReleaseWithCollection(componentRelease);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to get component release {componentRef.Uuid}: {ex.Message}");
                }
            }

            if (release.Components.Count > maxComponents && !json)
            {
                AnsiConsole.MarkupLine(
                    $"[dim]... and {release.Components.Count - maxComponents} more components (use --max-components to show more)[/]");
            }

            return 0;
        });

        return command;
    }
}
