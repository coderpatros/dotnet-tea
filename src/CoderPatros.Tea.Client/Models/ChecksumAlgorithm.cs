// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace CoderPatros.Tea.Client.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ChecksumAlgorithm>))]
public enum ChecksumAlgorithm
{
    [JsonStringEnumMemberName("MD5")]
    MD5,

    [JsonStringEnumMemberName("SHA-1")]
    SHA1,

    [JsonStringEnumMemberName("SHA-256")]
    SHA256,

    [JsonStringEnumMemberName("SHA-384")]
    SHA384,

    [JsonStringEnumMemberName("SHA-512")]
    SHA512,

    [JsonStringEnumMemberName("SHA3-256")]
    SHA3_256,

    [JsonStringEnumMemberName("SHA3-384")]
    SHA3_384,

    [JsonStringEnumMemberName("SHA3-512")]
    SHA3_512,

    [JsonStringEnumMemberName("BLAKE2b-256")]
    BLAKE2b_256,

    [JsonStringEnumMemberName("BLAKE2b-384")]
    BLAKE2b_384,

    [JsonStringEnumMemberName("BLAKE2b-512")]
    BLAKE2b_512,

    [JsonStringEnumMemberName("BLAKE3")]
    BLAKE3
}
