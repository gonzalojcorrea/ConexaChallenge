namespace Domain.Entities;

/// <summary>
/// Represents a role entity.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
