namespace Domain.Entities;

/// <summary>
/// Represents a user entity.
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public Guid RoleId { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; }
}
