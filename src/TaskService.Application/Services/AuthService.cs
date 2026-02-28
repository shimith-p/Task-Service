using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskService.Application.DTOs;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Application.Settings;
using TaskService.Domain.Interfaces;
using TaskService.Domain.Models;

using ValidationException = TaskService.Application.Exceptions.ValidationException;

namespace TaskService.Application.Services;

/// <summary>
/// Handles login credential validation, BCrypt password verification,
/// and JWT token generation with full claims and expiry. 
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IOptions<JwtSettings> jwtSettings,
        IValidator<LoginRequestDto> loginValidator,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _jwtSettings = jwtSettings.Value;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {  
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult);
         
        var user = await _authRepository.FindByUsernameAsync(request.Username, cancellationToken);  

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning(
                "Failed login attempt for username: {Username}", request.Username);

            throw UnauthorizedException.InvalidCredentials();
        }
         
        if (!user.IsActive)
        {
            _logger.LogWarning(
                "Login attempt on inactive account. UserId={UserId}", user.Id);

            throw UnauthorizedException.AccountInactive();
        }
         
        var (token, expiresAt) = GenerateJwtToken(user);

        _logger.LogInformation(
            "User logged in. UserId={UserId} Username={Username} ExpiresAt={ExpiresAt}",
            user.Id, user.Username, expiresAt);

        return new LoginResponseDto(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: _jwtSettings.ExpiryMinutes * 60,  // seconds
            ExpiresAt: expiresAt,
            User: MapToUserDto(user)
        );
    }

    // ── Get current user ──────────────────────────────────────────────────────

    public async Task<UserDto> GetCurrentUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(UsersModel), userId);

        return MapToUserDto(user);
    }

    // ── JWT token builder ─────────────────────────────────────────────────────

    private (string token, DateTime expiresAt) GenerateJwtToken(UsersModel user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // ── Claims ────────────────────────────────────────────────────────────
        // Standard JWT claims (sub, jti, iat) + app-specific claims
        var claims = new[]
        {
            // Standard claims
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),  // unique token ID
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            // Identity claims — read by controllers via User.Identity
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Email,          user.Email),

            // Custom app claims — available via User.FindFirst("uid") etc.
            new Claim("uid",      user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("email",    user.Email),
        };

        // ── Build and sign token ──────────────────────────────────────────────
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static UserDto MapToUserDto(UsersModel user) => new(
        Id: user.Id,
        Username: user.Username,
        Email: user.Email,
        IsActive: user.IsActive,
        CreatedAt: user.CreatedAt
    );

    public Task LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}