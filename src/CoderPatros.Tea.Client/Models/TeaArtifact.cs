// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

public sealed record TeaArtifact(
    [property: JsonPropertyName("uuid")] string? Uuid = null,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("type")] ArtifactType? Type = null,
    [property: JsonPropertyName("distributionTypes")] IReadOnlyList<string>? DistributionTypes = null,
    [property: JsonPropertyName("formats")] IReadOnlyList<ArtifactFormat>? Formats = null);
