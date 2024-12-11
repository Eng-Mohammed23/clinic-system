namespace ClinicSystem.Api.Contracts;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.Amount).NotEmpty();

        RuleFor(x =>x.PhoneNumber).NotEmpty().Length(11);
    }
}
