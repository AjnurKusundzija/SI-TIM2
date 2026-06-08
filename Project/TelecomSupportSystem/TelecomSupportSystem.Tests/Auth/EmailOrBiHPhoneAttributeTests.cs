using System.ComponentModel.DataAnnotations;
using Xunit;
using TelecomSupportSystem.BLL.Helpers;

namespace TelecomSupportSystem.Tests.Auth;

/// <summary>
/// Unit tests for EmailOrBiHPhoneAttribute covering US-119 (PB-67).
/// </summary>
public class EmailOrBiHPhoneAttributeTests
{
    private readonly EmailOrBiHPhoneAttribute _sut = new();
    private static ValidationContext Ctx => new(new object());

    // US-119: ispravna email adresa prolazi validaciju
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("korisnik@firma.ba")]
    [InlineData("support.agent@telecom.ba")]
    public void IsValid_ValidEmail_ReturnsSuccess(string input)
    {
        var result = _sut.GetValidationResult(input, Ctx);
        Assert.Equal(ValidationResult.Success, result);
    }

    // US-119: ispravan broj telefona u +387 formatu prolazi validaciju
    [Theory]
    [InlineData("+38761234567")]
    [InlineData("+38762987654")]
    [InlineData("+38733111222")]
    public void IsValid_ValidBiHPhone_ReturnsSuccess(string input)
    {
        var result = _sut.GetValidationResult(input, Ctx);
        Assert.Equal(ValidationResult.Success, result);
    }

    // US-119: broj telefona bez +387 prefiksa mora biti odbijen
    [Theory]
    [InlineData("061234567")]
    [InlineData("0038761234567")]
    [InlineData("38761234567")]
    public void IsValid_PhoneWithoutPlus387Prefix_ReturnsError(string input)
    {
        var result = _sut.GetValidationResult(input, Ctx);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // US-119: prekratak broj (manje od 8 cifara nakon +387) odbijen
    [Fact]
    public void IsValid_TooShortPhoneNumber_ReturnsError()
    {
        var result = _sut.GetValidationResult("+38761", Ctx);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // US-119: neispravan email format odbijen
    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void IsValid_InvalidEmailFormat_ReturnsError(string input)
    {
        var result = _sut.GetValidationResult(input, Ctx);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // US-119: null vrijednost odbijen
    [Fact]
    public void IsValid_NullValue_ReturnsError()
    {
        var result = _sut.GetValidationResult(null, Ctx);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // US-119: prazan string odbijen
    [Fact]
    public void IsValid_EmptyString_ReturnsError()
    {
        var result = _sut.GetValidationResult(string.Empty, Ctx);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // US-119: poruka greške za pogrešan unos nije null/prazna
    [Fact]
    public void IsValid_InvalidInput_ErrorMessageIsNotEmpty()
    {
        var result = _sut.GetValidationResult("invalid-input", Ctx);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.ErrorMessage));
    }
}
