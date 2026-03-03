// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

public sealed record TeaCollection(
    [property: JsonPropertyName("uuid")] string? Uuid = null,
    [property: JsonPropertyName("version")] int? Version = null,
    [property: JsonPropertyName("date")] DateTimeOffset? Date = null,
    [property: JsonPropertyName("belongsTo")] CollectionBelongsToType? BelongsTo = null,
    [property: JsonPropertyName("updateReason")] UpdateReason? UpdateReason = null,
    [property: JsonPropertyName("artifacts")] IReadOnlyList<TeaArtifact>? Artifacts = null);
