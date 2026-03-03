// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Net;
using FluentAssertions;
using RichardSzalay.MockHttp;
using CoderPatros.Tea.Client.Exceptions;
using CoderPatros.Tea.Client.Models;

namespace CoderPatros.Tea.Client.Tests;

public class TeaClientTests
{
    private readonly MockHttpMessageHandler _mockHttp = new();
    private readonly ITeaClient _client;

    public TeaClientTests()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.example.com/tea/v1/");
        _client = new TeaClient(httpClient);
    }

    [Fact]
    public async Task GetProductAsync_ReturnsProduct()
    {
        const string uuid = "09e8c73b-ac45-4475-acac-33e6a7314e6d";
        _mockHttp.When($"https://api.example.com/tea/v1/product/{uuid}")
            .Respond("application/json", """
            {
              "uuid": "09e8c73b-ac45-4475-acac-33e6a7314e6d",
              "name": "Apache Log4j 2",
              "identifiers": [
                { "idType": "CPE", "idValue": "cpe:2.3:a:apache:log4j" }
              ]
            }
            """);

        var product = await _client.GetProductAsync(uuid);

        product.Uuid.Should().Be(uuid);
        product.Name.Should().Be("Apache Log4j 2");
        product.Identifiers.Should().HaveCount(1);
    }

    [Fact]
    public async Task QueryProductsAsync_BuildsCorrectUrl()
    {
        _mockHttp.When("https://api.example.com/tea/v1/products?idType=PURL&idValue=pkg%3Amaven%2Flog4j&pageOffset=0&pageSize=10")
            .Respond("application/json", """
            {
              "timestamp": "2024-03-20T15:30:00Z",
              "pageStartIndex": 0,
              "pageSize": 10,
              "totalResults": 1,
              "results": [
                {
                  "uuid": "09e8c73b-ac45-4475-acac-33e6a7314e6d",
                  "name": "Log4j",
                  "identifiers": []
                }
              ]
            }
            """);

        var result = await _client.QueryProductsAsync(
            IdentifierType.PURL, "pkg:maven/log4j", pageOffset: 0, pageSize: 10);

        result.TotalResults.Should().Be(1);
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetProductReleasesAsync_WithPagination()
    {
        const string productUuid = "09e8c73b-ac45-4475-acac-33e6a7314e6d";
        _mockHttp.When($"https://api.example.com/tea/v1/product/{productUuid}/releases?pageOffset=0&pageSize=5")
            .Respond("application/json", """
            {
              "timestamp": "2024-03-20T15:30:00Z",
              "pageStartIndex": 0,
              "pageSize": 5,
              "totalResults": 2,
              "results": [
                {
                  "uuid": "123e4567-e89b-12d3-a456-426614174000",
                  "version": "2.24.3",
                  "createdDate": "2025-04-01T15:43:00Z",
                  "components": []
                }
              ]
            }
            """);

        var result = await _client.GetProductReleasesAsync(productUuid, pageOffset: 0, pageSize: 5);

        result.TotalResults.Should().Be(2);
        result.Results.Should().HaveCount(1);
        result.Results[0].Version.Should().Be("2.24.3");
    }

    [Fact]
    public async Task GetProductReleaseAsync_ReturnsRelease()
    {
        const string uuid = "123e4567-e89b-12d3-a456-426614174000";
        _mockHttp.When($"https://api.example.com/tea/v1/productRelease/{uuid}")
            .Respond("application/json", """
            {
              "uuid": "123e4567-e89b-12d3-a456-426614174000",
              "version": "2.24.3",
              "createdDate": "2025-04-01T15:43:00Z",
              "components": []
            }
            """);

        var release = await _client.GetProductReleaseAsync(uuid);
        release.Version.Should().Be("2.24.3");
    }

    [Fact]
    public async Task GetComponentAsync_ReturnsComponent()
    {
        const string uuid = "3910e0fd-aff4-48d6-b75f-8bf6b84687f0";
        _mockHttp.When($"https://api.example.com/tea/v1/component/{uuid}")
            .Respond("application/json", """
            {
              "uuid": "3910e0fd-aff4-48d6-b75f-8bf6b84687f0",
              "name": "Apache Log4j API",
              "identifiers": []
            }
            """);

        var component = await _client.GetComponentAsync(uuid);
        component.Name.Should().Be("Apache Log4j API");
    }

    [Fact]
    public async Task GetComponentReleasesAsync_ReturnsList()
    {
        const string componentUuid = "3910e0fd-aff4-48d6-b75f-8bf6b84687f0";
        _mockHttp.When($"https://api.example.com/tea/v1/component/{componentUuid}/releases")
            .Respond("application/json", """
            [
              {
                "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
                "version": "11.0.7",
                "createdDate": "2025-05-07T18:08:00Z"
              }
            ]
            """);

        var releases = await _client.GetComponentReleasesAsync(componentUuid);
        releases.Should().HaveCount(1);
        releases[0].Version.Should().Be("11.0.7");
    }

    [Fact]
    public async Task GetComponentReleaseAsync_ReturnsWithCollection()
    {
        const string uuid = "605d0ecb-1057-40e4-9abf-c400b10f0345";
        _mockHttp.When($"https://api.example.com/tea/v1/componentRelease/{uuid}")
            .Respond("application/json", """
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
                "artifacts": []
              }
            }
            """);

        var result = await _client.GetComponentReleaseAsync(uuid);
        result.Release.Version.Should().Be("11.0.7");
        result.LatestCollection.Version.Should().Be(2);
    }

    [Fact]
    public async Task GetComponentReleaseLatestCollectionAsync_ReturnsCollection()
    {
        const string uuid = "605d0ecb-1057-40e4-9abf-c400b10f0345";
        _mockHttp.When($"https://api.example.com/tea/v1/componentRelease/{uuid}/collection/latest")
            .Respond("application/json", """
            {
              "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
              "version": 3,
              "date": "2025-06-01T00:00:00Z"
            }
            """);

        var collection = await _client.GetComponentReleaseLatestCollectionAsync(uuid);
        collection.Version.Should().Be(3);
    }

    [Fact]
    public async Task GetComponentReleaseCollectionsAsync_ReturnsList()
    {
        const string uuid = "605d0ecb-1057-40e4-9abf-c400b10f0345";
        _mockHttp.When($"https://api.example.com/tea/v1/componentRelease/{uuid}/collections")
            .Respond("application/json", """
            [
              { "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345", "version": 1 },
              { "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345", "version": 2 }
            ]
            """);

        var collections = await _client.GetComponentReleaseCollectionsAsync(uuid);
        collections.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetComponentReleaseCollectionAsync_ReturnsSpecificVersion()
    {
        const string uuid = "605d0ecb-1057-40e4-9abf-c400b10f0345";
        _mockHttp.When($"https://api.example.com/tea/v1/componentRelease/{uuid}/collection/2")
            .Respond("application/json", """
            {
              "uuid": "605d0ecb-1057-40e4-9abf-c400b10f0345",
              "version": 2
            }
            """);

        var collection = await _client.GetComponentReleaseCollectionAsync(uuid, 2);
        collection.Version.Should().Be(2);
    }

    [Fact]
    public async Task GetProductReleaseLatestCollectionAsync_ReturnsCollection()
    {
        const string uuid = "123e4567-e89b-12d3-a456-426614174000";
        _mockHttp.When($"https://api.example.com/tea/v1/productRelease/{uuid}/collection/latest")
            .Respond("application/json", """
            {
              "uuid": "123e4567-e89b-12d3-a456-426614174000",
              "version": 1
            }
            """);

        var collection = await _client.GetProductReleaseLatestCollectionAsync(uuid);
        collection.Version.Should().Be(1);
    }

    [Fact]
    public async Task GetArtifactAsync_ReturnsArtifact()
    {
        const string uuid = "1cb47b95-8bf8-3bad-a5a4-0d54d86e10ce";
        _mockHttp.When($"https://api.example.com/tea/v1/artifact/{uuid}")
            .Respond("application/json", """
            {
              "uuid": "1cb47b95-8bf8-3bad-a5a4-0d54d86e10ce",
              "name": "Build SBOM",
              "type": "BOM",
              "formats": [
                {
                  "mediaType": "application/vnd.cyclonedx+xml",
                  "url": "https://example.com/sbom.xml"
                }
              ]
            }
            """);

        var artifact = await _client.GetArtifactAsync(uuid);
        artifact.Name.Should().Be("Build SBOM");
        artifact.Type.Should().Be(Tea.Client.Models.ArtifactType.BOM);
    }

    [Fact]
    public async Task DiscoverByTeiAsync_EncodesAndReturnsResults()
    {
        var tei = "urn:tei:uuid:products.example.com:d4d9f54a-abcf-11ee-ac79-1a52914d44b";
        var encodedTei = Uri.EscapeDataString(tei);
        _mockHttp.When($"https://api.example.com/tea/v1/discovery?tei={encodedTei}")
            .Respond("application/json", """
            [
              {
                "productReleaseUuid": "d4d9f54a-abcf-11ee-ac79-1a52914d44b",
                "servers": [
                  {
                    "rootUrl": "https://api.teaexample.com",
                    "versions": ["0.3.0-beta.2"],
                    "priority": 0.8
                  }
                ]
              }
            ]
            """);

        var results = await _client.DiscoverByTeiAsync(tei);
        results.Should().HaveCount(1);
        results[0].ProductReleaseUuid.Should().Be("d4d9f54a-abcf-11ee-ac79-1a52914d44b");
    }

    [Fact]
    public async Task GetProductAsync_404_ThrowsTeaNotFoundException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/nonexistent")
            .Respond(HttpStatusCode.NotFound, "application/json", """{ "error": "OBJECT_UNKNOWN" }""");

        var act = () => _client.GetProductAsync("nonexistent");
        var ex = await act.Should().ThrowAsync<TeaNotFoundException>();
        ex.Which.ErrorResponse.Should().NotBeNull();
        ex.Which.ErrorResponse!.Error.Should().Be(Tea.Client.Models.ErrorType.OBJECT_UNKNOWN);
    }

    [Fact]
    public async Task GetProductAsync_404_NotShareable_ThrowsTeaNotFoundException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/restricted")
            .Respond(HttpStatusCode.NotFound, "application/json", """{ "error": "OBJECT_NOT_SHAREABLE" }""");

        var act = () => _client.GetProductAsync("restricted");
        var ex = await act.Should().ThrowAsync<TeaNotFoundException>();
        ex.Which.ErrorResponse!.Error.Should().Be(Tea.Client.Models.ErrorType.OBJECT_NOT_SHAREABLE);
    }

    [Fact]
    public async Task GetProductAsync_400_ThrowsTeaBadRequestException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/bad")
            .Respond(HttpStatusCode.BadRequest, "application/json", "{}");

        var act = () => _client.GetProductAsync("bad");
        await act.Should().ThrowAsync<TeaBadRequestException>();
    }

    [Fact]
    public async Task GetProductAsync_401_ThrowsTeaAuthenticationException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/secret")
            .Respond(HttpStatusCode.Unauthorized, "application/json", "{}");

        var act = () => _client.GetProductAsync("secret");
        var ex = await act.Should().ThrowAsync<TeaAuthenticationException>();
        ex.Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProductAsync_403_ThrowsTeaAuthenticationException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/forbidden")
            .Respond(HttpStatusCode.Forbidden, "application/json", "{}");

        var act = () => _client.GetProductAsync("forbidden");
        var ex = await act.Should().ThrowAsync<TeaAuthenticationException>();
        ex.Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProductAsync_500_ThrowsTeaApiException()
    {
        _mockHttp.When("https://api.example.com/tea/v1/product/error")
            .Respond(HttpStatusCode.InternalServerError, "application/json", "{}");

        var act = () => _client.GetProductAsync("error");
        await act.Should().ThrowAsync<TeaApiException>();
    }

    [Fact]
    public async Task GetProductCleAsync_ReturnsCleData()
    {
        const string uuid = "09e8c73b-ac45-4475-acac-33e6a7314e6d";
        _mockHttp.When($"https://api.example.com/tea/v1/product/{uuid}/cle")
            .Respond("application/json", """
            {
              "events": [
                {
                  "id": 1,
                  "type": "released",
                  "effective": "2024-01-01T00:00:00Z",
                  "published": "2024-01-01T00:00:00Z",
                  "version": "1.0.0"
                }
              ]
            }
            """);

        var cle = await _client.GetProductCleAsync(uuid);
        cle.Events.Should().HaveCount(1);
    }

    [Fact]
    public async Task QueryProductReleasesAsync_WithIdentifier()
    {
        _mockHttp.When("https://api.example.com/tea/v1/productReleases?idType=TEI&idValue=tei%3Avendor%3Aproduct")
            .Respond("application/json", """
            {
              "timestamp": "2024-03-20T15:30:00Z",
              "pageStartIndex": 0,
              "pageSize": 100,
              "totalResults": 0,
              "results": []
            }
            """);

        var result = await _client.QueryProductReleasesAsync(IdentifierType.TEI, "tei:vendor:product");
        result.Results.Should().BeEmpty();
    }
}
