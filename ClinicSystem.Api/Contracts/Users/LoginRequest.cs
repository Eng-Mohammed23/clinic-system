namespace ClinicSystem.Api.Contracts.Users;

public record LoginRequest(
    string Email, string Password
    );

