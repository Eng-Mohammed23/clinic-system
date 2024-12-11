namespace ClinicSystem.Api.Contracts.Users;

public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
