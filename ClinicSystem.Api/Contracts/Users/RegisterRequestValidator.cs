namespace ClinicSystem.Api.Contracts.Users;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(5, 100);
        
        RuleFor(x => x.PhoneNum)
            .NotEmpty()
            .Length(11);
    }
}
