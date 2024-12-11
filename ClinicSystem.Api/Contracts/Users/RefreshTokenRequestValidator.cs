namespace ClinicSystem.Api.Contracts.Users;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
