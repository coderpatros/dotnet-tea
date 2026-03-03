// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

[JsonConverter(typeof(JsonStringEnumConverter<UpdateReasonType>))]
public enum UpdateReasonType
{
    INITIAL_RELEASE,
    VEX_UPDATED,
    ARTIFACT_UPDATED,
    ARTIFACT_ADDED,
    ARTIFACT_REMOVED
}
