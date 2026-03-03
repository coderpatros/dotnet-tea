// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models.Cle;

public sealed record CleData(
    [property: JsonPropertyName("events")] IReadOnlyList<CleEvent> Events,
    [property: JsonPropertyName("definitions")] CleDefinitions? Definitions = null);
