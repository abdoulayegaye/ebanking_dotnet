namespace Ebank.API.Entities;
public class Customer : BaseEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool State { get; set; } = true;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}