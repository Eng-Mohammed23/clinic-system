using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicSystem.Api.Entities;

public sealed class Message
{
    public int MessageId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public string? MessageAnswer { get; set; } 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    [ForeignKey(nameof(SenderId))]
    public ApplicationUser User { get; set; } = default!;
}
