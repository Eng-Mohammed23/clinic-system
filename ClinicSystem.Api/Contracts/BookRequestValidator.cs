namespace ClinicSystem.Api.Contracts;

public class BookRequestValidator : AbstractValidator<BookRequest>
{
    public BookRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(5,100);

        RuleFor(x => x.Price).NotEmpty();

        RuleFor(x => x.TimeOfBook).NotEmpty();

        RuleFor(x => x.Title).NotEmpty().Length(5,30);

        RuleFor(x => x.PhoneNumber).NotEmpty().Length(11);
    }
}
