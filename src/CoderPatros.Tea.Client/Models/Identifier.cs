// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

public sealed record Identifier(
    [property: JsonPropertyName("idType")] IdentifierType IdType,
    [property: JsonPropertyName("idValue")] string IdValue);
