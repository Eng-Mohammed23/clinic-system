using ClinicSystem.Api.Entities;
using Microsoft.AspNetCore.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? CreatedBy {  get; set; }
    public DateTime? EdittingOn {  get; set; }
    public string? EdittingBy { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}
