// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace CoderPatros.Tea.Client.Discovery;

public sealed record Tei(TeiType Type, string Domain, string Identifier)
{
    private static readonly Dictionary<string, TeiType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["uuid"] = TeiType.Uuid,
        ["purl"] = TeiType.Purl,
        ["hash"] = TeiType.Hash,
        ["swid"] = TeiType.Swid,
        ["eanupc"] = TeiType.EanUpc,
        ["gtin"] = TeiType.Gtin,
        ["asin"] = TeiType.Asin,
        ["udi"] = TeiType.Udi,
    };

    private static readonly Dictionary<TeiType, string> ReverseTypeMap =
        TypeMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    public string ToUrn() => $"urn:tei:{ReverseTypeMap[Type]}:{Domain}:{Identifier}";

    public static Tei Parse(string urn)
    {
        if (!TryParse(urn, out var tei) || tei is null)
            throw new FormatException($"Invalid TEI URN: '{urn}'");
        return tei;
    }

    public static bool TryParse(string? urn, out Tei? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(urn))
            return false;

        // Format: urn:tei:<type>:<domain>:<identifier>
        // The identifier may contain colons (e.g., hash type:value)
        if (!urn.StartsWith("urn:tei:", StringComparison.OrdinalIgnoreCase))
            return false;

        var remainder = urn.AsSpan(8); // skip "urn:tei:"

        // Extract type
        var firstColon = remainder.IndexOf(':');
        if (firstColon <= 0)
            return false;

        var typeStr = remainder[..firstColon].ToString();
        if (!TypeMap.TryGetValue(typeStr, out var type))
            return false;

        remainder = remainder[(firstColon + 1)..];

        // Extract domain
        var secondColon = remainder.IndexOf(':');
        if (secondColon <= 0)
            return false;

        var domain = remainder[..secondColon].ToString();
        remainder = remainder[(secondColon + 1)..];

        // Everything else is the identifier
        if (remainder.IsEmpty)
            return false;

        var identifier = remainder.ToString();

        result = new Tei(type, domain, identifier);
        return true;
    }
}
