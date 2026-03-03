// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Tea.Client.Models;

public sealed record Distribution(
    [property: JsonPropertyName("distributionType")] string? DistributionType = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("identifiers")] IReadOnlyList<Identifier>? Identifiers = null,
    [property: JsonPropertyName("url")] string? Url = null,
    [property: JsonPropertyName("signatureUrl")] string? SignatureUrl = null,
    [property: JsonPropertyName("checksums")] IReadOnlyList<Checksum>? Checksums = null);
