namespace DataTouch.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = "OrgUser";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Organization Organization { get; set; } = default!;
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
