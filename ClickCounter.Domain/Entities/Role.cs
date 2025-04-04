namespace ClickCounter.Domain.Entities;

public partial class Role {
    public int RoleId { get; set; }

    public string? Name { get; set; }

    public int Active { get; set; }

    public int Trash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
