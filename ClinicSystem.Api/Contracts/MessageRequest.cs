namespace ClinicSystem.Api.Contracts;

public record MessageRequest(
    string SenderId,
    string ReceiverId ,
    string MessageText
);

