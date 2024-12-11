namespace ClinicSystem.Api.Contracts.Users;

public record AuthResponse(
    string id,
    string Email,
    string FullName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration

);

