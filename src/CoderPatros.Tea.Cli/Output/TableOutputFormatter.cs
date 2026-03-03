// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;
using Spectre.Console;

namespace CoderPatros.Tea.Cli.Output;

public sealed class TableOutputFormatter : IOutputFormatter
{
    public void WriteDiscoveryResults(IReadOnlyList<DiscoveryInfo> results)
    {
        var table = new Table();
        table.AddColumn("Product Release UUID");
        table.AddColumn("Server URL");
        table.AddColumn("Versions");
        table.AddColumn("Priority");

        foreach (var info in results)
        {
            foreach (var server in info.Servers)
            {
                table.AddRow(
                    info.ProductReleaseUuid.EscapeMarkup(),
                    server.RootUrl.EscapeMarkup(),
                    string.Join(", ", server.Versions),
                    server.Priority?.ToString("F1") ?? "-");
            }
        }

        AnsiConsole.Write(table);
    }

    public void WriteProduct(TeaProduct product)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");
        table.AddRow("UUID", product.Uuid.EscapeMarkup());
        table.AddRow("Name", product.Name.EscapeMarkup());

        if (product.Identifiers.Count > 0)
        {
            table.AddRow("Identifiers", FormatIdentifiers(product.Identifiers));
        }

        AnsiConsole.Write(table);
    }

    public void WriteProducts(PaginatedResponse<TeaProduct> response)
    {
        WritePaginationHeader(response);

        var table = new Table();
        table.AddColumn("UUID");
        table.AddColumn("Name");
        table.AddColumn("Identifiers");

        foreach (var product in response.Results)
        {
            table.AddRow(
                product.Uuid.EscapeMarkup(),
                product.Name.EscapeMarkup(),
                FormatIdentifiers(product.Identifiers));
        }

        AnsiConsole.Write(table);
    }

    public void WriteProductRelease(TeaProductRelease release)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");
        table.AddRow("UUID", release.Uuid.EscapeMarkup());
        table.AddRow("Version", release.Version.EscapeMarkup());
        table.AddRow("Created", release.CreatedDate.ToString("u"));

        if (release.Product is not null)
            table.AddRow("Product UUID", release.Product.EscapeMarkup());
        if (release.ProductName is not null)
            table.AddRow("Product Name", release.ProductName.EscapeMarkup());
        if (release.ReleaseDate is not null)
            table.AddRow("Release Date", release.ReleaseDate.Value.ToString("u"));
        if (release.PreRelease is not null)
            table.AddRow("Pre-Release", release.PreRelease.Value.ToString());
        if (release.Identifiers is { Count: > 0 })
            table.AddRow("Identifiers", FormatIdentifiers(release.Identifiers));
        if (release.Components.Count > 0)
            table.AddRow("Components", string.Join(", ", release.Components.Select(c => c.Uuid)));

        AnsiConsole.Write(table);
    }

    public void WriteProductReleases(PaginatedResponse<TeaProductRelease> response)
    {
        WritePaginationHeader(response);

        var table = new Table();
        table.AddColumn("UUID");
        table.AddColumn("Version");
        table.AddColumn("Created");
        table.AddColumn("Components");

        foreach (var release in response.Results)
        {
            table.AddRow(
                release.Uuid.EscapeMarkup(),
                release.Version.EscapeMarkup(),
                release.CreatedDate.ToString("u"),
                release.Components.Count.ToString());
        }

        AnsiConsole.Write(table);
    }

    public void WriteComponentRelease(TeaComponentRelease release)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");
        table.AddRow("UUID", release.Uuid.EscapeMarkup());
        table.AddRow("Version", release.Version.EscapeMarkup());
        table.AddRow("Created", release.CreatedDate.ToString("u"));

        if (release.Component is not null)
            table.AddRow("Component UUID", release.Component.EscapeMarkup());
        if (release.ComponentName is not null)
            table.AddRow("Component Name", release.ComponentName.EscapeMarkup());
        if (release.ReleaseDate is not null)
            table.AddRow("Release Date", release.ReleaseDate.Value.ToString("u"));
        if (release.PreRelease is not null)
            table.AddRow("Pre-Release", release.PreRelease.Value.ToString());
        if (release.Identifiers is { Count: > 0 })
            table.AddRow("Identifiers", FormatIdentifiers(release.Identifiers));

        AnsiConsole.Write(table);
    }

    public void WriteComponentReleaseWithCollection(ComponentReleaseWithCollection releaseWithCollection)
    {
        WriteComponentRelease(releaseWithCollection.Release);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Latest Collection:[/]");
        WriteCollection(releaseWithCollection.LatestCollection);
    }

    public void WriteComponent(TeaComponent component)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");
        table.AddRow("UUID", component.Uuid.EscapeMarkup());
        table.AddRow("Name", component.Name.EscapeMarkup());

        if (component.Identifiers.Count > 0)
        {
            table.AddRow("Identifiers", FormatIdentifiers(component.Identifiers));
        }

        AnsiConsole.Write(table);
    }

    public void WriteComponentReleases(IReadOnlyList<TeaComponentRelease> releases)
    {
        var table = new Table();
        table.AddColumn("UUID");
        table.AddColumn("Version");
        table.AddColumn("Created");
        table.AddColumn("Component");

        foreach (var release in releases)
        {
            table.AddRow(
                release.Uuid.EscapeMarkup(),
                release.Version.EscapeMarkup(),
                release.CreatedDate.ToString("u"),
                (release.ComponentName ?? release.Component ?? "-").EscapeMarkup());
        }

        AnsiConsole.Write(table);
    }

    public void WriteCollection(TeaCollection collection)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");

        if (collection.Uuid is not null)
            table.AddRow("UUID", collection.Uuid.EscapeMarkup());
        if (collection.Version is not null)
            table.AddRow("Version", collection.Version.Value.ToString());
        if (collection.Date is not null)
            table.AddRow("Date", collection.Date.Value.ToString("u"));
        if (collection.BelongsTo is not null)
            table.AddRow("Belongs To", collection.BelongsTo.Value.ToString());
        if (collection.UpdateReason is not null)
        {
            var reason = collection.UpdateReason.Type?.ToString() ?? "-";
            if (collection.UpdateReason.Comment is not null)
                reason += $" ({collection.UpdateReason.Comment})";
            table.AddRow("Update Reason", reason.EscapeMarkup());
        }

        AnsiConsole.Write(table);

        if (collection.Artifacts is { Count: > 0 })
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Artifacts:[/]");
            WriteArtifactsTable(collection.Artifacts);
        }
    }

    public void WriteCollections(IReadOnlyList<TeaCollection> collections)
    {
        var table = new Table();
        table.AddColumn("UUID");
        table.AddColumn("Version");
        table.AddColumn("Date");
        table.AddColumn("Belongs To");
        table.AddColumn("Artifacts");

        foreach (var collection in collections)
        {
            table.AddRow(
                (collection.Uuid ?? "-").EscapeMarkup(),
                collection.Version?.ToString() ?? "-",
                collection.Date?.ToString("u") ?? "-",
                collection.BelongsTo?.ToString() ?? "-",
                collection.Artifacts?.Count.ToString() ?? "0");
        }

        AnsiConsole.Write(table);
    }

    public void WriteArtifact(TeaArtifact artifact)
    {
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");

        if (artifact.Uuid is not null)
            table.AddRow("UUID", artifact.Uuid.EscapeMarkup());
        if (artifact.Name is not null)
            table.AddRow("Name", artifact.Name.EscapeMarkup());
        if (artifact.Type is not null)
            table.AddRow("Type", artifact.Type.Value.ToString());
        if (artifact.DistributionTypes is { Count: > 0 })
            table.AddRow("Distribution Types", string.Join(", ", artifact.DistributionTypes));

        AnsiConsole.Write(table);

        if (artifact.Formats is { Count: > 0 })
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Formats:[/]");
            var formatTable = new Table();
            formatTable.AddColumn("Media Type");
            formatTable.AddColumn("URL");
            formatTable.AddColumn("Checksums");

            foreach (var format in artifact.Formats)
            {
                formatTable.AddRow(
                    (format.MediaType ?? "-").EscapeMarkup(),
                    (format.Url ?? "-").EscapeMarkup(),
                    format.Checksums is { Count: > 0 }
                        ? string.Join("\n", format.Checksums.Select(c => $"{c.AlgType}: {c.AlgValue}")).EscapeMarkup()
                        : "-");
            }

            AnsiConsole.Write(formatTable);
        }
    }

    public void WriteCle(CleData cle)
    {
        if (cle.Definitions?.Support is { Count: > 0 })
        {
            AnsiConsole.MarkupLine("[bold]Support Definitions:[/]");
            var defTable = new Table();
            defTable.AddColumn("ID");
            defTable.AddColumn("Description");
            defTable.AddColumn("URL");

            foreach (var def in cle.Definitions.Support)
            {
                defTable.AddRow(
                    def.Id.EscapeMarkup(),
                    def.Description.EscapeMarkup(),
                    (def.Url ?? "-").EscapeMarkup());
            }

            AnsiConsole.Write(defTable);
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine("[bold]Events:[/]");
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Type");
        table.AddColumn("Effective");
        table.AddColumn("Published");
        table.AddColumn("Version");
        table.AddColumn("Description");

        foreach (var evt in cle.Events)
        {
            table.AddRow(
                evt.Id.ToString(),
                evt.Type.ToString(),
                evt.Effective.ToString("u"),
                evt.Published.ToString("u"),
                (evt.Version ?? "-").EscapeMarkup(),
                (evt.Description ?? "-").EscapeMarkup());
        }

        AnsiConsole.Write(table);
    }

    public void WriteMessage(string message)
    {
        AnsiConsole.MarkupLine(message.EscapeMarkup());
    }

    private static void WriteArtifactsTable(IReadOnlyList<TeaArtifact> artifacts)
    {
        var table = new Table();
        table.AddColumn("UUID");
        table.AddColumn("Name");
        table.AddColumn("Type");
        table.AddColumn("Formats");

        foreach (var artifact in artifacts)
        {
            table.AddRow(
                (artifact.Uuid ?? "-").EscapeMarkup(),
                (artifact.Name ?? "-").EscapeMarkup(),
                artifact.Type?.ToString() ?? "-",
                artifact.Formats?.Count.ToString() ?? "0");
        }

        AnsiConsole.Write(table);
    }

    private static void WritePaginationHeader<T>(PaginatedResponse<T> response)
    {
        AnsiConsole.MarkupLine(
            $"[dim]Showing {response.Results.Count} of {response.TotalResults} results (offset {response.PageStartIndex})[/]");
        AnsiConsole.WriteLine();
    }

    private static string FormatIdentifiers(IReadOnlyList<Identifier> identifiers)
    {
        return string.Join("\n", identifiers.Select(i => $"{i.IdType}: {i.IdValue}")).EscapeMarkup();
    }
}
