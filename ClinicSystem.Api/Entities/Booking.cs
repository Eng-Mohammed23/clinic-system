namespace ClinicSystem.Api.Entities;

public sealed class Booking
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Report {  get; set; }
    public bool HasWhats { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime TimeOfBook { get; set; }
}
