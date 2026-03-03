// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Tea.Client.Models.Cle;

public sealed record CleEvent(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("type")] CleEventType Type,
    [property: JsonPropertyName("effective")] DateTimeOffset Effective,
    [property: JsonPropertyName("published")] DateTimeOffset Published,
    [property: JsonPropertyName("version")] string? Version = null,
    [property: JsonPropertyName("versions")] IReadOnlyList<CleVersionSpecifier>? Versions = null,
    [property: JsonPropertyName("supportId")] string? SupportId = null,
    [property: JsonPropertyName("license")] string? License = null,
    [property: JsonPropertyName("supersededByVersion")] string? SupersededByVersion = null,
    [property: JsonPropertyName("identifiers")] IReadOnlyList<Models.Identifier>? Identifiers = null,
    [property: JsonPropertyName("eventId")] int? EventId = null,
    [property: JsonPropertyName("reason")] string? Reason = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("references")] IReadOnlyList<string>? References = null);
