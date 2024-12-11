namespace ClinicSystem.Api.Contracts.Users;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);

