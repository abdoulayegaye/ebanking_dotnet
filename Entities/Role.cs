namespace Ebank.API.Entities;
using Ebank.API.Entities;
public class Role : BaseEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}