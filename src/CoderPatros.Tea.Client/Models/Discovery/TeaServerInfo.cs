// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models.Discovery;

public sealed record TeaServerInfo(
    [property: JsonPropertyName("rootUrl")] string RootUrl,
    [property: JsonPropertyName("versions")] IReadOnlyList<string> Versions,
    [property: JsonPropertyName("priority")] double? Priority = null);
