namespace TaskService.Application.DTOs
{
    public sealed record LoginRequestDto(
        string Username,
        string Password     // Plaintext — compared against BCrypt hash, never stored
    );
}
