// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json;
using FluentAssertions;
using CoderPatros.Tea.Client.Models;
using CoderPatros.Tea.Client.Models.Cle;
using CoderPatros.Tea.Client.Models.Discovery;
using CoderPatros.Tea.Client.Serialization;

namespace CoderPatros.Tea.Client.Tests.Models;

public class ModelSerializationTests
{
    private readonly JsonSerializerOptions _options = TeaJsonSerializerOptions.Default;

    [Fact]
    public void Product_RoundTrips()
    {
        const string json = """
        {
          "uuid": "09e8c73b-ac45-4475-acac-33e6a7314e6d",
          "name": "Apache Log4j 2",
          "identifiers": [
            { "idType": "CPE", "idValue": "cpe:2.3:a:apache:log4j" },
            { "idType": "PURL", "idValue": "pkg:maven/org.apache.logging.log4j/log4j-api" }
          ]
        }
        """;

        var product = JsonSerializer.Deserialize<TeaProduct>(json, _options)!;
        product.Uuid.Should().Be("09e8c73b-ac45-4475-acac-33e6a7314e6d");
        product.Name.Should().Be("Apache Log4j 2");
        product.Identifiers.Should().HaveCount(2);
        product.Identifiers[0].IdType.Should().Be(IdentifierType.CPE);
        product.Identifiers[1].IdType.Should().Be(IdentifierType.PURL);

        var roundTripped = JsonSerializer.Serialize(product, _options);
        var deserialized = JsonSerializer.Deserialize<TeaProduct>(roundTripped, _options)!;
        deserialized.Should().BeEquivalentTo(product);
    }

    [Fact]
    public void ProductRelease_RoundTrips()
    {
        const string json = """
        {
          "uuid": "123e4567-e89b-12d3-a456-426614174000",
          "version": "2.24.3",
          "createdDate": "2025-04-01T15:43:00Z",
          "releaseDate": "2025-04-01T15:43:00Z",
          "identifiers": [
            { "idType": "TEI", "idValue": "tei:vendor:product@2.24.3" }
          ],
          "components": [
            { "uuid": "3910e0fd-aff4-48d6-b75f-8bf6b84687f0" },
            { "uuid": "b844c9bd-55d6-478c-af59-954a932b6ad3", "release": "da89e38e-95e7-44ca-aa7d-f3b6b34c7fab" }
          ]
        }
        """;

        var release = JsonSerializer.Deserialize<TeaProductRelease>(json, _options)!;
        release.Uuid.Should().Be("123e4567-e89b-12d3-a456-426614174000");
        release.Version.Should().Be("2.24.3");
        release.Components.Should().HaveCount(2);
        release.Components[0].Release.Should().BeNull();
        release.Components[1].Release.Should().Be("da89e38e-95e7-44ca-aa7d-f3b6b34c7fab");

        var roundTripped = JsonSerializer.Serialize(release, _options);
        var deserialized = JsonSerializer.Deserialize<TeaProductRelease>(roundTripped, _options)!;
        deserialized.Should().BeEquivalentTo(release);
    }

    [Fact]
    public void Component_RoundTrips()
    {
        const string json = """
        {
          "uuid": "3910e0fd-aff4-48d6-b75f-8bf6b84687f0",
          "name": "Apache Log4j API",
          "identifiers": [
            { "idType": "PURL", "idValue": "pkg:maven/org.apache.logging.log4j/log4j-api" }
          ]
        }
        """;

        var component = JsonSerializer.Deserialize<TeaComponent>(json, _options)!;
        component.Uuid.Should().Be("3910e0fd-aff4-48d6-b75f-8bf6b84687f0");
        component.Name.Should().Be("Apache Log4j API");

        var roundTripped = JsonSerializer.Serialize(component, _options);
        var deserialized = JsonSerializer.Deserialize<TeaComponent>(roundTripped, _options)!;
        deserialized.Should().BeEquivalentTo(component);
    }

    [Fact]
    public void ComponentRelease_WithDistributions_RoundTrips()
    {
        const string json = """
        {
          "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
          "version": "11.0.7",
          "createdDate": "2025-05-07T18:08:00Z",
          "releaseDate": "2025-05-12T18:08:00Z",
          "identifiers": [
            { "idType": "PURL", "idValue": "pkg:maven/org.apache.tomcat/tomcat@11.0.7" }
          ],
          "distributions": [
            {
              "distributionType": "zip",
              "description": "Core binary distribution, zip archive",
              "checksums": [
                { "algType": "SHA-256", "algValue": "9da736a1cdd27231e70187cbc67398d29ca0b714f885e7032da9f1fb247693c1" }
              ],
              "url": "https://repo.maven.apache.org/maven2/org/apache/tomcat/tomcat/11.0.7/tomcat-11.0.6.zip"
            }
          ]
        }
        """;

        var release = JsonSerializer.Deserialize<TeaComponentRelease>(json, _options)!;
        release.Uuid.Should().Be("605d0ecb-1057-40e4-9abf-c400b10f0345");
        release.Distributions.Should().HaveCount(1);
        release.Distributions![0].DistributionType.Should().Be("zip");
        release.Distributions[0].Checksums.Should().HaveCount(1);
        release.Distributions[0].Checksums![0].AlgType.Should().Be(ChecksumAlgorithm.SHA256);
    }

    [Fact]
    public void Collection_RoundTrips()
    {
        const string json = """
        {
          "uuid": "4c72fe22-9d83-4c2f-8eba-d6db484f32c8",
          "version": 3,
          "date": "2024-12-13T00:00:00Z",
          "updateReason": {
            "type": "ARTIFACT_UPDATED",
            "comment": "VDR file updated"
          },
          "artifacts": [
            {
              "uuid": "1cb47b95-8bf8-3bad-a5a4-0d54d86e10ce",
              "name": "Build SBOM",
              "type": "BOM",
              "formats": [
                {
                  "mediaType": "application/vnd.cyclonedx+xml",
                  "description": "CycloneDX SBOM (XML)",
                  "url": "https://example.com/sbom.xml",
                  "signatureUrl": "https://example.com/sbom.xml.asc",
                  "checksums": [
                    { "algType": "MD5", "algValue": "2e1a525afc81b0a8ecff114b8b743de9" },
                    { "algType": "SHA-1", "algValue": "5a7d4caef63c5c5ccdf07c39337323529eb5a770" }
                  ]
                }
              ]
            }
          ]
        }
        """;

        var collection = JsonSerializer.Deserialize<TeaCollection>(json, _options)!;
        collection.Uuid.Should().Be("4c72fe22-9d83-4c2f-8eba-d6db484f32c8");
        collection.Version.Should().Be(3);
        collection.UpdateReason!.Type.Should().Be(UpdateReasonType.ARTIFACT_UPDATED);
        collection.Artifacts.Should().HaveCount(1);
        collection.Artifacts![0].Type.Should().Be(ArtifactType.BOM);
        collection.Artifacts[0].Formats.Should().HaveCount(1);
        collection.Artifacts[0].Formats![0].Checksums.Should().HaveCount(2);
        collection.Artifacts[0].Formats![0].Checksums![0].AlgType.Should().Be(ChecksumAlgorithm.MD5);
        collection.Artifacts[0].Formats![0].Checksums![1].AlgType.Should().Be(ChecksumAlgorithm.SHA1);
    }

    [Fact]
    public void ComponentReleaseWithCollection_RoundTrips()
    {
        const string json = """
        {
          "release": {
            "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
            "version": "11.0.7",
            "createdDate": "2025-05-07T18:08:00Z"
          },
          "latestCollection": {
            "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
            "version": 2,
            "date": "2025-05-12T18:08:00Z",
            "belongsTo": "COMPONENT_RELEASE",
            "updateReason": {
              "type": "INITIAL_RELEASE",
              "comment": "Initial collection for this release"
            },
            "artifacts": []
          }
        }
        """;

        var result = JsonSerializer.Deserialize<ComponentReleaseWithCollection>(json, _options)!;
        result.Release.Uuid.Should().Be("605d0ecb-1057-40e4-9abf-c400b10f0345");
        result.LatestCollection.BelongsTo.Should().Be(CollectionBelongsToType.COMPONENT_RELEASE);
        result.LatestCollection.UpdateReason!.Type.Should().Be(UpdateReasonType.INITIAL_RELEASE);
    }

    [Fact]
    public void CleData_RoundTrips()
    {
        const string json = """
        {
          "events": [
            {
              "id": 3,
              "type": "endOfSupport",
              "effective": "2025-06-01T00:00:00Z",
              "versions": [
                { "range": "vers:npm/>=1.0.0|<2.0.0" }
              ],
              "supportId": "standard",
              "published": "2025-01-01T00:00:00Z"
            },
            {
              "id": 1,
              "type": "released",
              "effective": "2024-01-01T00:00:00Z",
              "version": "1.0.0",
              "license": "Apache-2.0",
              "published": "2024-01-01T00:00:00Z"
            }
          ],
          "definitions": {
            "support": [
              {
                "id": "standard",
                "description": "Standard product support policy",
                "url": "https://example.com/support/standard"
              }
            ]
          }
        }
        """;

        var cle = JsonSerializer.Deserialize<CleData>(json, _options)!;
        cle.Events.Should().HaveCount(2);
        cle.Events[0].Type.Should().Be(CleEventType.EndOfSupport);
        cle.Events[0].Versions.Should().HaveCount(1);
        cle.Events[0].Versions![0].Range.Should().Be("vers:npm/>=1.0.0|<2.0.0");
        cle.Events[1].Type.Should().Be(CleEventType.Released);
        cle.Events[1].License.Should().Be("Apache-2.0");
        cle.Definitions.Should().NotBeNull();
        cle.Definitions!.Support.Should().HaveCount(1);
        cle.Definitions.Support![0].Id.Should().Be("standard");
    }

    [Fact]
    public void ErrorResponse_RoundTrips()
    {
        const string json = """{ "error": "OBJECT_UNKNOWN" }""";
        var error = JsonSerializer.Deserialize<ErrorResponse>(json, _options)!;
        error.Error.Should().Be(ErrorType.OBJECT_UNKNOWN);

        var roundTripped = JsonSerializer.Serialize(error, _options);
        roundTripped.Should().Contain("OBJECT_UNKNOWN");
    }

    [Fact]
    public void PaginatedProductResponse_Deserializes()
    {
        const string json = """
        {
          "timestamp": "2024-03-20T15:30:00Z",
          "pageStartIndex": 0,
          "pageSize": 100,
          "totalResults": 1,
          "results": [
            {
              "uuid": "09e8c73b-ac45-4475-acac-33e6a7314e6d",
              "name": "Apache Log4j 2",
              "identifiers": []
            }
          ]
        }
        """;

        var paginated = JsonSerializer.Deserialize<PaginatedResponse<TeaProduct>>(json, _options)!;
        paginated.TotalResults.Should().Be(1);
        paginated.Results.Should().HaveCount(1);
        paginated.Results[0].Name.Should().Be("Apache Log4j 2");
    }

    [Theory]
    [InlineData("MD5", ChecksumAlgorithm.MD5)]
    [InlineData("SHA-1", ChecksumAlgorithm.SHA1)]
    [InlineData("SHA-256", ChecksumAlgorithm.SHA256)]
    [InlineData("SHA-384", ChecksumAlgorithm.SHA384)]
    [InlineData("SHA-512", ChecksumAlgorithm.SHA512)]
    [InlineData("SHA3-256", ChecksumAlgorithm.SHA3_256)]
    [InlineData("SHA3-384", ChecksumAlgorithm.SHA3_384)]
    [InlineData("SHA3-512", ChecksumAlgorithm.SHA3_512)]
    [InlineData("BLAKE2b-256", ChecksumAlgorithm.BLAKE2b_256)]
    [InlineData("BLAKE2b-384", ChecksumAlgorithm.BLAKE2b_384)]
    [InlineData("BLAKE2b-512", ChecksumAlgorithm.BLAKE2b_512)]
    [InlineData("BLAKE3", ChecksumAlgorithm.BLAKE3)]
    public void ChecksumAlgorithm_SerializesCorrectly(string wireValue, ChecksumAlgorithm expected)
    {
        var json = $$$"""{ "algType": "{{{wireValue}}}", "algValue": "abc123" }""";
        var checksum = JsonSerializer.Deserialize<Checksum>(json, _options)!;
        checksum.AlgType.Should().Be(expected);

        var serialized = JsonSerializer.Serialize(checksum, _options);
        serialized.Should().Contain($"\"{wireValue}\"");
    }

    [Theory]
    [InlineData("released", CleEventType.Released)]
    [InlineData("endOfDevelopment", CleEventType.EndOfDevelopment)]
    [InlineData("endOfSupport", CleEventType.EndOfSupport)]
    [InlineData("endOfLife", CleEventType.EndOfLife)]
    [InlineData("endOfDistribution", CleEventType.EndOfDistribution)]
    [InlineData("endOfMarketing", CleEventType.EndOfMarketing)]
    [InlineData("supersededBy", CleEventType.SupersededBy)]
    [InlineData("componentRenamed", CleEventType.ComponentRenamed)]
    [InlineData("withdrawn", CleEventType.Withdrawn)]
    public void CleEventType_SerializesCorrectly(string wireValue, CleEventType expected)
    {
        var json = $$$"""
        {
          "id": 1,
          "type": "{{{wireValue}}}",
          "effective": "2024-01-01T00:00:00Z",
          "published": "2024-01-01T00:00:00Z"
        }
        """;
        var evt = JsonSerializer.Deserialize<CleEvent>(json, _options)!;
        evt.Type.Should().Be(expected);

        var serialized = JsonSerializer.Serialize(evt, _options);
        serialized.Should().Contain($"\"{wireValue}\"");
    }

    [Fact]
    public void WellKnownResponse_Deserializes()
    {
        const string json = """
        {
          "schemaVersion": 1,
          "endpoints": [
            {
              "url": "https://api.teaexample.com",
              "versions": ["0.2.0-beta.2", "1.0.0"],
              "priority": 1
            },
            {
              "url": "https://api2.teaexample.com/mytea",
              "versions": ["1.0.0"],
              "priority": 0.5
            }
          ]
        }
        """;

        var response = JsonSerializer.Deserialize<WellKnownResponse>(json, _options)!;
        response.SchemaVersion.Should().Be(1);
        response.Endpoints.Should().HaveCount(2);
        response.Endpoints[0].Priority.Should().Be(1.0);
        response.Endpoints[1].Url.Should().Be("https://api2.teaexample.com/mytea");
    }

    [Fact]
    public void DiscoveryInfo_Deserializes()
    {
        const string json = """
        {
          "productReleaseUuid": "d4d9f54a-abcf-11ee-ac79-1a52914d44b",
          "servers": [
            {
              "rootUrl": "https://api.teaexample.com",
              "versions": ["0.2.0-beta.2", "1.0.0"],
              "priority": 0.8
            }
          ]
        }
        """;

        var info = JsonSerializer.Deserialize<DiscoveryInfo>(json, _options)!;
        info.ProductReleaseUuid.Should().Be("d4d9f54a-abcf-11ee-ac79-1a52914d44b");
        info.Servers.Should().HaveCount(1);
        info.Servers[0].RootUrl.Should().Be("https://api.teaexample.com");
        info.Servers[0].Priority.Should().Be(0.8);
    }

    [Theory]
    [InlineData("ATTESTATION", ArtifactType.ATTESTATION)]
    [InlineData("BOM", ArtifactType.BOM)]
    [InlineData("BUILD_META", ArtifactType.BUILD_META)]
    [InlineData("VULNERABILITIES", ArtifactType.VULNERABILITIES)]
    [InlineData("OTHER", ArtifactType.OTHER)]
    public void ArtifactType_SerializesCorrectly(string wireValue, ArtifactType expected)
    {
        var json = $$$"""{ "uuid": "test-uuid", "type": "{{{wireValue}}}" }""";
        var artifact = JsonSerializer.Deserialize<TeaArtifact>(json, _options)!;
        artifact.Type.Should().Be(expected);
    }

    [Fact]
    public void PreReleaseComponentRelease_Deserializes()
    {
        const string json = """
        {
          "uuid": "95f481df-f760-47f4-b2f2-f8b76d858450",
          "version": "11.0.0-M26",
          "createdDate": "2024-09-13T17:49:00Z",
          "preRelease": true,
          "identifiers": [
            { "idType": "PURL", "idValue": "pkg:maven/org.apache.tomcat/tomcat@11.0.0-M26" }
          ]
        }
        """;

        var release = JsonSerializer.Deserialize<TeaComponentRelease>(json, _options)!;
        release.PreRelease.Should().BeTrue();
        release.Version.Should().Be("11.0.0-M26");
        release.Distributions.Should().BeNull();
    }
}
