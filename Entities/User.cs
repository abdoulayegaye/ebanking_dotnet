namespace Ebank.API.Entities;
public class User : BaseEntity
{
    public long Id { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public bool IsEnabled { get; set; } = true;
    public bool AccountIsEnabled { get; set; } = true;
    public bool AccountIsNotExpired { get; set; } = true;
    public bool AccountIsNotLocked { get; set; } = true;
    public bool CredentialNotExpired { get; set; } = true;
    public long RoleId { get; set; }                           // ← FK explicite
    public Role Role { get; set; } = null!;                    // ← @ManyToOne EAGER
}
