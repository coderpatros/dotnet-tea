// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models.Cle;

[JsonConverter(typeof(JsonStringEnumConverter<CleEventType>))]
public enum CleEventType
{
    [JsonStringEnumMemberName("released")]
    Released,

    [JsonStringEnumMemberName("endOfDevelopment")]
    EndOfDevelopment,

    [JsonStringEnumMemberName("endOfSupport")]
    EndOfSupport,

    [JsonStringEnumMemberName("endOfLife")]
    EndOfLife,

    [JsonStringEnumMemberName("endOfDistribution")]
    EndOfDistribution,

    [JsonStringEnumMemberName("endOfMarketing")]
    EndOfMarketing,

    [JsonStringEnumMemberName("supersededBy")]
    SupersededBy,

    [JsonStringEnumMemberName("componentRenamed")]
    ComponentRenamed,

    [JsonStringEnumMemberName("withdrawn")]
    Withdrawn
}
