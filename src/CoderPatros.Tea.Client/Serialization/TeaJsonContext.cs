// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;

namespace CoderPatros.Tea.Client.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Identifier))]
[JsonSerializable(typeof(TeaProduct))]
[JsonSerializable(typeof(TeaProductRelease))]
[JsonSerializable(typeof(ComponentRef))]
[JsonSerializable(typeof(TeaComponent))]
[JsonSerializable(typeof(TeaComponentRelease))]
[JsonSerializable(typeof(ComponentReleaseWithCollection))]
[JsonSerializable(typeof(Distribution))]
[JsonSerializable(typeof(TeaCollection))]
[JsonSerializable(typeof(UpdateReason))]
[JsonSerializable(typeof(TeaArtifact))]
[JsonSerializable(typeof(ArtifactFormat))]
[JsonSerializable(typeof(Checksum))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(PaginatedResponse<TeaProduct>))]
[JsonSerializable(typeof(PaginatedResponse<TeaProductRelease>))]
[JsonSerializable(typeof(CleData))]
[JsonSerializable(typeof(CleEvent))]
[JsonSerializable(typeof(CleVersionSpecifier))]
[JsonSerializable(typeof(CleSupportDefinition))]
[JsonSerializable(typeof(CleDefinitions))]
[JsonSerializable(typeof(WellKnownResponse))]
[JsonSerializable(typeof(WellKnownEndpoint))]
[JsonSerializable(typeof(DiscoveryInfo))]
[JsonSerializable(typeof(DiscoveryInfo[]))]
[JsonSerializable(typeof(TeaServerInfo))]
[JsonSerializable(typeof(IReadOnlyList<TeaComponentRelease>))]
[JsonSerializable(typeof(IReadOnlyList<TeaCollection>))]
[JsonSerializable(typeof(List<TeaComponentRelease>))]
[JsonSerializable(typeof(List<TeaCollection>))]
[JsonSerializable(typeof(List<DiscoveryInfo>))]
public partial class TeaJsonContext : JsonSerializerContext
{
}
