namespace Ebank.API.Entities;
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UserCreated { get; set; }
    public string? UserUpdated { get; set; }
}