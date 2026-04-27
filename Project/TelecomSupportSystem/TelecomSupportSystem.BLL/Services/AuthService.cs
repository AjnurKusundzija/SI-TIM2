using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TelecomSupportSystem.BLL.DTOs.Auth;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            if (user.AccountStatus == AccountStatus.INACTIVE)
                return null;

            var accessToken = GenerateAccessToken(user);
            var refreshToken = await CreateRefreshTokenAsync(user.UserId);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<RefreshResponseDto?> RefreshAsync(string refreshToken)
        {
            var stored = await _refreshTokenRepository.GetByTokenAsync(HashToken(refreshToken));

            // token reuse detection — revoke entire session if a revoked token is presented
            if (stored != null && stored.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(stored.UserId);
                return null;
            }

            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                return null;

            if (stored.User.AccountStatus == AccountStatus.INACTIVE)
                return null;

            await _refreshTokenRepository.RevokeAsync(stored);

            var newAccessToken = GenerateAccessToken(stored.User);
            var newRefreshToken = await CreateRefreshTokenAsync(stored.UserId);

            return new RefreshResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> RevokeAsync(string refreshToken)
        {
            var stored = await _refreshTokenRepository.GetByTokenAsync(HashToken(refreshToken));
            if (stored == null || stored.IsRevoked)
                return false;

            await _refreshTokenRepository.RevokeAsync(stored);
            return true;
        }

        private string GenerateAccessToken(User user)
        {
            var jwtKey = _configuration["JWT_KEY"]
                ?? throw new InvalidOperationException("JWT_KEY is missing from configuration providers.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> CreateRefreshTokenAsync(int userId)
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = HashToken(rawToken),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            return rawToken;
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
