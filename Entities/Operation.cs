namespace Ebank.API.Entities;
public class Operation : BaseEntity
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public TypeOperation Type { get; set; }                    // ← @Enumerated(STRING)
    public double Amount { get; set; }
    public long AccountId { get; set; }                        // ← FK explicite
    public Account Account { get; set; } = null!;              // ← @ManyToOne
}