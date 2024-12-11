
namespace ClinicSystem.Api.Persistence.EntitiesConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(x => x.MessageText).HasMaxLength(500);
        //throw new NotImplementedException();
    }
}
