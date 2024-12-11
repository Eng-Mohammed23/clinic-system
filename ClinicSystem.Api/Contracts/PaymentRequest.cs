namespace ClinicSystem.Api.Contracts;

public record PaymentRequest(
    string PhoneNumber,
    decimal Amount
);
