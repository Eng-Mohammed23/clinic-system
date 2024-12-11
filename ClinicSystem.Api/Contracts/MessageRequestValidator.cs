namespace ClinicSystem.Api.Contracts;

public class MessageRequestValidator : AbstractValidator<MessageRequest>
{
    public MessageRequestValidator()
    {
        RuleFor(x => x.SenderId).NotEmpty();

        RuleFor(x => x.ReceiverId).NotEmpty();

        RuleFor(x => x.MessageText).NotEmpty().MaximumLength(500);
    }
}
