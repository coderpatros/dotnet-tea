// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.CommandLine;
using System.Security.Cryptography;

namespace CoderPatros.Tea.Cli.Commands;

internal static class DownloadCommand
{
    public static Command Create(GlobalOptions globals)
    {
        var urlArg = new Argument<string>("url") { Description = "URL of the artifact to download" };
        var destArg = new Argument<string>("dest") { Description = "Destination file path" };
        var checksumOption = new Option<string[]>("--checksum") { Description = "Expected checksum in ALG:VALUE format (repeatable)", AllowMultipleArgumentsPerToken = true };

        var command = new Command("download") { Description = "Download an artifact and optionally verify checksums" };
        command.Arguments.Add(urlArg);
        command.Arguments.Add(destArg);
        command.Options.Add(checksumOption);

        command.SetAction(async (parseResult, ct) =>
        {
            var formatter = Program.CreateFormatter(parseResult.GetValue(globals.Json));
            var httpClient = Program.CreateHttpClient(
                parseResult.GetValue(globals.BaseUrl),
                parseResult.GetValue(globals.Token),
                parseResult.GetValue(globals.Timeout));

            var url = parseResult.GetValue(urlArg)!;
            var dest = parseResult.GetValue(destArg)!;
            var checksums = parseResult.GetValue(checksumOption);

            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            await using var fileStream = File.Create(dest);
            await stream.CopyToAsync(fileStream, ct);

            formatter.WriteMessage($"Downloaded to {dest}");

            if (checksums is { Length: > 0 })
            {
                foreach (var checksum in checksums)
                {
                    var separatorIndex = checksum.IndexOf(':');
                    if (separatorIndex < 0)
                    {
                        Console.Error.WriteLine($"Invalid checksum format: {checksum} (expected ALG:VALUE)");
                        continue;
                    }

                    var algorithm = checksum[..separatorIndex];
                    var expected = checksum[(separatorIndex + 1)..];

                    var actual = await ComputeHashAsync(dest, algorithm);
                    if (actual is null)
                    {
                        Console.Error.WriteLine($"Unsupported algorithm: {algorithm}");
                        continue;
                    }

                    if (string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                    {
                        formatter.WriteMessage($"Checksum OK ({algorithm})");
                    }
                    else
                    {
                        Console.Error.WriteLine($"Checksum MISMATCH ({algorithm}): expected {expected}, got {actual}");
                    }
                }
            }
        });

        return command;
    }

    private static async Task<string?> ComputeHashAsync(string filePath, string algorithm)
    {
        using var hashAlgorithm = algorithm.ToUpperInvariant() switch
        {
            "MD5" => (HashAlgorithm)MD5.Create(),
            "SHA-1" or "SHA1" => SHA1.Create(),
            "SHA-256" or "SHA256" => SHA256.Create(),
            "SHA-384" or "SHA384" => SHA384.Create(),
            "SHA-512" or "SHA512" => SHA512.Create(),
            _ => null
        };

        if (hashAlgorithm is null)
            return null;

        await using var stream = File.OpenRead(filePath);
        var hash = await hashAlgorithm.ComputeHashAsync(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
