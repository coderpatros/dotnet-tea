# Tea.Client

A C# .NET client library for the [Transparency Exchange API (TEA)](https://github.com/CycloneDX/transparency-exchange-api) — the OWASP/ECMA TC54 standard for automating the exchange of software supply chain transparency artifacts (SBOMs, VEX, attestations, and more).

Targets the TEA specification **v0.3.0-beta.2**.

## Packages

| Package | Description |
| --- | --- |
| `Tea.Client` | Core client library |
| `Tea.Client.Extensions.DependencyInjection` | DI integration with `IHttpClientFactory` |

## Requirements

- .NET 8.0 or later

## Getting Started

### Direct usage

```csharp
using Tea.Client;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://tea.example.com/tea/v1/")
};

var client = new TeaClient(httpClient);

// Get a product by UUID
var product = await client.GetProductAsync("09e8c73b-ac45-4475-acac-33e6a7314e6d");
Console.WriteLine($"{product.Name} has {product.Identifiers.Count} identifier(s)");

// Query products by PURL
var results = await client.QueryProductsAsync(
    idType: IdentifierType.PURL,
    idValue: "pkg:maven/org.apache.logging.log4j/log4j-api");
```

### With dependency injection

```csharp
using Tea.Client.Extensions.DependencyInjection;

services.AddTeaClient(options =>
{
    options.BaseAddress = new Uri("https://tea.example.com/tea/v1/");
})
.AddBearerToken("your-api-token");
```

Then inject `ITeaClient` into your services:

```csharp
public class MyService(ITeaClient teaClient)
{
    public async Task CheckComponentAsync(string uuid)
    {
        var result = await teaClient.GetComponentReleaseAsync(uuid);
        var collection = result.LatestCollection;
        // ...
    }
}
```

## API Coverage

### Products

```csharp
await client.GetProductAsync(uuid);
await client.QueryProductsAsync(idType, idValue, pageOffset, pageSize);
await client.GetProductCleAsync(uuid);
```

### Product Releases

```csharp
await client.GetProductReleaseAsync(uuid);
await client.GetProductReleasesAsync(productUuid, pageOffset, pageSize);
await client.QueryProductReleasesAsync(idType, idValue, pageOffset, pageSize);
await client.GetProductReleaseCleAsync(uuid);
```

### Components

```csharp
await client.GetComponentAsync(uuid);
await client.GetComponentReleasesAsync(componentUuid);
await client.GetComponentCleAsync(uuid);
```

### Component Releases

```csharp
await client.GetComponentReleaseAsync(uuid);       // Returns release + latest collection
await client.GetComponentReleaseCleAsync(uuid);
```

### Collections

```csharp
// Product release collections
await client.GetProductReleaseLatestCollectionAsync(uuid);
await client.GetProductReleaseCollectionsAsync(uuid);
await client.GetProductReleaseCollectionAsync(uuid, collectionVersion);

// Component release collections
await client.GetComponentReleaseLatestCollectionAsync(uuid);
await client.GetComponentReleaseCollectionsAsync(uuid);
await client.GetComponentReleaseCollectionAsync(uuid, collectionVersion);
```

### Artifacts

```csharp
await client.GetArtifactAsync(uuid);
```

### Discovery

```csharp
await client.DiscoverByTeiAsync("urn:tei:uuid:products.example.com:d4d9f54a-abcf-11ee-ac79-1a52914d44b");
```

## TEI Parsing

Parse Transparency Exchange Identifiers (TEIs) directly:

```csharp
using Tea.Client.Discovery;

var tei = Tei.Parse("urn:tei:purl:cyclonedx.org:pkg:pypi/cyclonedx-python-lib@8.4.0");
// tei.Type     == TeiType.Purl
// tei.Domain   == "cyclonedx.org"
// tei.Identifier == "pkg:pypi/cyclonedx-python-lib@8.4.0"

// Safe parsing
if (Tei.TryParse(input, out var parsed))
{
    Console.WriteLine(parsed!.Domain);
}
```

Supported TEI types: `uuid`, `purl`, `hash`, `swid`, `eanupc`, `gtin`, `asin`, `udi`.

## TEI Resolution

The `TeiResolver` implements the full discovery flow: parse TEI, fetch `.well-known/tea`, select the best endpoint by version compatibility and priority, then call `/discovery`:

```csharp
using Tea.Client.Discovery;

var resolver = new TeiResolver(httpClient);
var results = await resolver.ResolveAsync("urn:tei:uuid:products.example.com:some-uuid");

foreach (var info in results)
{
    Console.WriteLine($"Product Release: {info.ProductReleaseUuid}");
    foreach (var server in info.Servers)
        Console.WriteLine($"  Server: {server.RootUrl} (versions: {string.Join(", ", server.Versions)})");
}
```

With DI, inject `ITeiResolver` directly (registered automatically by `AddTeaClient`).

## Authentication

### Bearer token

```csharp
// Static token
services.AddTeaClient(options => { ... })
    .AddBearerToken("my-token");

// Dynamic token provider
services.AddTeaClient(options => { ... })
    .AddBearerToken(async cancellationToken =>
    {
        // Fetch token from your auth provider
        return await GetTokenAsync(cancellationToken);
    });
```

### Mutual TLS

```csharp
services.AddTeaClient(options => { ... })
    .AddMutualTls("/path/to/client.pfx", "password");
```

Or pass an `X509Certificate2` instance directly:

```csharp
var cert = new X509Certificate2("/path/to/client.pfx", "password");
services.AddTeaClient(options => { ... })
    .AddMutualTls(cert);
```

## Error Handling

The client throws typed exceptions for different HTTP error responses:

| Exception | Status Code | Description |
| --- | --- | --- |
| `TeaBadRequestException` | 400 | Invalid request |
| `TeaAuthenticationException` | 401, 403 | Authentication or authorization failure |
| `TeaNotFoundException` | 404 | Object not found (includes parsed `ErrorResponse` with `OBJECT_UNKNOWN` or `OBJECT_NOT_SHAREABLE`) |
| `TeaApiException` | Other | Base exception for any other non-success status |

```csharp
try
{
    var product = await client.GetProductAsync(uuid);
}
catch (TeaNotFoundException ex) when (ex.ErrorResponse?.Error == ErrorType.OBJECT_NOT_SHAREABLE)
{
    Console.WriteLine("This object exists but is not shareable");
}
catch (TeaAuthenticationException)
{
    Console.WriteLine("Check your credentials");
}
```

## Building

```bash
dotnet build
dotnet test
dotnet pack
```

## License

Apache-2.0
