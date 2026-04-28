using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Auth;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.Tests.Auth;

/// <summary>
/// Unit tests for AuthController covering PB-19 (US-1, US-2, US-3).
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_authService.Object);
    }

    // ─── Login ────────────────────────────────────────────────────────────────

    // US-1: nevažeći model vraća 400 bez poziva servisu
    [Fact]
    public async Task Login_InvalidModel_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Email", "The Email field is required.");

        var result = await _sut.Login(new LoginRequestDto());

        Assert.IsType<BadRequestObjectResult>(result);
        _authService.Verify(s => s.LoginAsync(It.IsAny<LoginRequestDto>()), Times.Never);
    }

    // US-1: ispravni kredencijali vraćaju 200 s payload-om
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithPayload()
    {
        var response = new LoginResponseDto
        {
            AccessToken = "jwt",
            RefreshToken = "rt",
            UserId = 1,
            FirstName = "Emir",
            LastName = "Hadzic",
            Email = "emir@example.com",
            Role = "CLIENT",
        };
        _authService.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync(response);

        var result = await _sut.Login(new LoginRequestDto { Email = "emir@example.com", Password = "pass" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, ok.Value);
    }

    // US-3: pogrešni kredencijali vraćaju 401 s generičkom porukom (ne otkriva koji je podatak pogrešan)
    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedWithGenericMessage()
    {
        _authService.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync((LoginResponseDto?)null);

        var result = await _sut.Login(new LoginRequestDto { Email = "x@x.com", Password = "wrong" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var body = unauthorized.Value as dynamic;
        Assert.NotNull(body);
        // Poruka ne smije razlikovati email od lozinke (US-3)
        var message = body!.GetType().GetProperty("message")?.GetValue(body)?.ToString();
        Assert.Equal("Invalid credentials.", message);
    }

    // ─── Refresh ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_InvalidModel_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("RefreshToken", "Required");

        var result = await _sut.Refresh(new RefreshRequestDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_InvalidToken_ReturnsUnauthorized()
    {
        _authService.Setup(s => s.RefreshAsync(It.IsAny<string>())).ReturnsAsync((RefreshResponseDto?)null);

        var result = await _sut.Refresh(new RefreshRequestDto { RefreshToken = "bad" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsOkWithNewTokens()
    {
        var response = new RefreshResponseDto { AccessToken = "new-jwt", RefreshToken = "new-rt" };
        _authService.Setup(s => s.RefreshAsync(It.IsAny<string>())).ReturnsAsync(response);

        var result = await _sut.Refresh(new RefreshRequestDto { RefreshToken = "valid-rt" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, ok.Value);
    }

    // ─── Logout ───────────────────────────────────────────────────────────────

    // US-2: odjava uvijek vraća 204 (session je obrisana na clientu bez obzira na rezultat)
    [Fact]
    public async Task Logout_ValidToken_ReturnsNoContent()
    {
        _authService.Setup(s => s.RevokeAsync(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _sut.Logout(new RefreshRequestDto { RefreshToken = "rt" });

        Assert.IsType<NoContentResult>(result);
        _authService.Verify(s => s.RevokeAsync("rt"), Times.Once);
    }
}
