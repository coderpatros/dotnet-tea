// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using FluentAssertions;
using CoderPatros.Tea.Client.Discovery;

namespace CoderPatros.Tea.Client.Tests.Discovery;

public class TeiParserTests
{
    [Fact]
    public void Parse_UuidTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:uuid:cyclonedx.org:d4d9f54a-abcf-11ee-ac79-1a52914d44b1");
        tei.Type.Should().Be(TeiType.Uuid);
        tei.Domain.Should().Be("cyclonedx.org");
        tei.Identifier.Should().Be("d4d9f54a-abcf-11ee-ac79-1a52914d44b1");
    }

    [Fact]
    public void Parse_PurlTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:purl:cyclonedx.org:pkg:pypi/cyclonedx-python-lib@8.4.0?extension=whl&qualifier=py3-none-any");
        tei.Type.Should().Be(TeiType.Purl);
        tei.Domain.Should().Be("cyclonedx.org");
        tei.Identifier.Should().Be("pkg:pypi/cyclonedx-python-lib@8.4.0?extension=whl&qualifier=py3-none-any");
    }

    [Fact]
    public void Parse_HashTei_PreservesFullIdentifier()
    {
        var tei = Tei.Parse("urn:tei:hash:cyclonedx.org:SHA256:fd44efd601f651c8865acf0dfeacb0df19a2b50ec69ead0262096fd2f67197b9");
        tei.Type.Should().Be(TeiType.Hash);
        tei.Domain.Should().Be("cyclonedx.org");
        tei.Identifier.Should().Be("SHA256:fd44efd601f651c8865acf0dfeacb0df19a2b50ec69ead0262096fd2f67197b9");
    }

    [Fact]
    public void Parse_SwidTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:swid:example.com:some-swid-tag-id");
        tei.Type.Should().Be(TeiType.Swid);
        tei.Domain.Should().Be("example.com");
        tei.Identifier.Should().Be("some-swid-tag-id");
    }

    [Fact]
    public void Parse_EanUpcTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:eanupc:cyclonedx.org:1234567890123");
        tei.Type.Should().Be(TeiType.EanUpc);
        tei.Identifier.Should().Be("1234567890123");
    }

    [Fact]
    public void Parse_GtinTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:gtin:cyclonedx.org:0234567890123");
        tei.Type.Should().Be(TeiType.Gtin);
    }

    [Fact]
    public void Parse_AsinTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:asin:cyclonedx.org:B07FZ8S74R");
        tei.Type.Should().Be(TeiType.Asin);
        tei.Identifier.Should().Be("B07FZ8S74R");
    }

    [Fact]
    public void Parse_UdiTei_Succeeds()
    {
        var tei = Tei.Parse("urn:tei:udi:cyclonedx.org:00123456789012");
        tei.Type.Should().Be(TeiType.Udi);
    }

    [Fact]
    public void ToUrn_RoundTrips()
    {
        const string urn = "urn:tei:uuid:products.example.com:d4d9f54a-abcf-11ee-ac79-1a52914d44b1";
        var tei = Tei.Parse(urn);
        tei.ToUrn().Should().Be(urn);
    }

    [Fact]
    public void ToUrn_PurlRoundTrips()
    {
        const string urn = "urn:tei:purl:cyclonedx.org:pkg:pypi/cyclonedx-python-lib@8.4.0";
        var tei = Tei.Parse(urn);
        tei.ToUrn().Should().Be(urn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-urn")]
    [InlineData("urn:other:uuid:domain:id")]
    [InlineData("urn:tei:invalid_type:domain:id")]
    [InlineData("urn:tei:uuid:")]
    [InlineData("urn:tei:uuid:domain:")]
    [InlineData("urn:tei::domain:id")]
    public void TryParse_InvalidInput_ReturnsFalse(string? input)
    {
        Tei.TryParse(input, out var result).Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_InvalidInput_ThrowsFormatException()
    {
        var act = () => Tei.Parse("not-a-urn");
        act.Should().Throw<FormatException>().WithMessage("*Invalid TEI URN*");
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        Tei.TryParse("urn:tei:uuid:example.com:some-uuid", out var result).Should().BeTrue();
        result.Should().NotBeNull();
        result!.Type.Should().Be(TeiType.Uuid);
    }

    [Fact]
    public void Parse_CaseInsensitive_Succeeds()
    {
        var tei = Tei.Parse("URN:TEI:UUID:example.com:some-uuid");
        tei.Type.Should().Be(TeiType.Uuid);
    }
}
