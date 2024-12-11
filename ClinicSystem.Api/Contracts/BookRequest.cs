namespace ClinicSystem.Api.Contracts;

public record BookRequest(
    string Name,
    string PhoneNumber,
    DateTime TimeOfBook,
    int Price,
    string Title,
    string? Description,
    bool HasWhats
);
