// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Tea.Client.Models;

public sealed record TeaComponentRelease(
    [property: JsonPropertyName("uuid")] string Uuid,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("createdDate")] DateTimeOffset CreatedDate,
    [property: JsonPropertyName("component")] string? Component = null,
    [property: JsonPropertyName("componentName")] string? ComponentName = null,
    [property: JsonPropertyName("releaseDate")] DateTimeOffset? ReleaseDate = null,
    [property: JsonPropertyName("preRelease")] bool? PreRelease = null,
    [property: JsonPropertyName("identifiers")] IReadOnlyList<Identifier>? Identifiers = null,
    [property: JsonPropertyName("distributions")] IReadOnlyList<Distribution>? Distributions = null);
