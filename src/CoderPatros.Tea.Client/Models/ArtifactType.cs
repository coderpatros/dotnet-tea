// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ArtifactType>))]
public enum ArtifactType
{
    ATTESTATION,
    BOM,
    BUILD_META,
    CERTIFICATION,
    FORMULATION,
    LICENSE,
    RELEASE_NOTES,
    SECURITY_TXT,
    THREAT_MODEL,
    VULNERABILITIES,
    OTHER
}
