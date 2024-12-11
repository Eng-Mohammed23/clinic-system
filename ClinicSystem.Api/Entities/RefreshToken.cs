using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Api.Entities;

[Owned]
public class RefreshToken
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }
    public bool IsExpired => ExpiresOn <= DateTime.UtcNow;

    public bool IsActive => RevokedOn is null && !IsExpired;
}