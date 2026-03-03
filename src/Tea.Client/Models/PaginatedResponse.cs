// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Tea.Client.Models;

public sealed record PaginatedResponse<T>(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("pageStartIndex")] long PageStartIndex,
    [property: JsonPropertyName("pageSize")] long PageSize,
    [property: JsonPropertyName("totalResults")] long TotalResults,
    [property: JsonPropertyName("results")] IReadOnlyList<T> Results);
