namespace ClinicSystem.Api.Contracts;

public record MessageResponse(
    string SenderId,
    string ReceiverId,
    string MessageText,
    string MessageAnswer,
    DateTime Timestamp,
    bool IsRead
);

