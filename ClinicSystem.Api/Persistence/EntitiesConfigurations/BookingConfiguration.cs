using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.Api.Persistence.EntitiesConfigurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(30);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(11);
    }
}