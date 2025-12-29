namespace DataTouch.Domain.Entities;

public class LeadNote
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    
    public Lead Lead { get; set; } = default!;
    public User CreatedBy { get; set; } = default!;
}
