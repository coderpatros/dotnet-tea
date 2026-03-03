// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models.Cle;

public sealed record CleVersionSpecifier(
    [property: JsonPropertyName("version")] string? Version = null,
    [property: JsonPropertyName("range")] string? Range = null);
