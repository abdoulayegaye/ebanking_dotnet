public class Account : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public double Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool Active { get; set; }
    public long CustomerId { get; set; }                        // ← FK explicite
    public Customer Customer { get; set; } = null!;            // ← @ManyToOne
    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}
