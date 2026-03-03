// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;

namespace CoderPatros.Tea.Cli.Commands;

internal static class GetCleCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var uuidArg = new Argument<string>("uuid") { Description = "Entity UUID" };
        var entityOption = new Option<string>("--entity") { Description = "Entity type: product, product-release, component, or component-release", Required = true };
        entityOption.AcceptOnlyFromAmong("product", "product-release", "component", "component-release");

        var command = new Command("get-cle") { Description = "Get Component Lifecycle Events (CLE) for an entity" };
        command.Arguments.Add(uuidArg);
        command.Options.Add(entityOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var client = Program.CreateTeaClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var uuid = parseResult.GetValue(uuidArg)!;
            var entity = parseResult.GetValue(entityOption)!;

            var result = entity switch
            {
                "product" => await client.GetProductCleAsync(uuid, ct),
                "product-release" => await client.GetProductReleaseCleAsync(uuid, ct),
                "component" => await client.GetComponentCleAsync(uuid, ct),
                "component-release" => await client.GetComponentReleaseCleAsync(uuid, ct),
                _ => throw new ArgumentException($"Unknown entity type: {entity}")
            };

            formatter.WriteCle(result);
        });

        return command;
    }
}
