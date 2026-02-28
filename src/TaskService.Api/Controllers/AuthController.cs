using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;

namespace TaskService.API.Controllers;

/// <summary>
/// Authentication endpoints — login, current-user profile, logout.
/// </summary>
[ApiController]
[Route("v1/api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────

    /// <summary>Authenticates a user and returns a JWT access token + refresh token.</summary> 
    /// <response code="200">Login successful — returns JWT + refresh token + user info.</response>
    /// <response code="401">Invalid credentials or account inactive.</response>
    /// <response code="422">Empty username or password.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("POST /api/auth/login – Username={Username}", request.Username);

        var response = await _authService.LoginAsync(request, cancellationToken);

        return Ok(response);
    }

    // ── GET /api/auth/me ──────────────────────────────────────────────────────

    /// <summary>Returns the profile of the currently authenticated user.</summary>
    /// <response code="200">User profile returned.</response>
    /// <response code="401">No token or token is invalid/expired.</response>
    /// <response code="404">User from token no longer exists in DB.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId.Value, cancellationToken);
        return Ok(user);
    }

    // ── POST /api/auth/logout ─────────────────────────────────────────────────

    /// <summary>
    /// Revokes all active refresh tokens for the current user.
    /// The access token itself remains valid until it expires — clients should discard it.
    /// </summary>
    /// <response code="204">Logged out successfully.</response>
    /// <response code="401">No token or token is invalid/expired.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await _authService.LogoutAsync(userId.Value, cancellationToken);

        _logger.LogInformation("User logged out. UserId={UserId}", userId.Value);

        return NoContent();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>Extracts the userId from the "uid" claim in the JWT.</summary>
    private int? GetUserIdFromToken()
    {
        var claim = User.FindFirstValue("uid");
        return int.TryParse(claim, out var id) ? id : null;
    }
}