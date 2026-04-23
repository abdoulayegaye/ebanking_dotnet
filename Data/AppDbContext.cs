using Microsoft.EntityFrameworkCore;
using Ebank.API.Entities;
namespace Ebank.API.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── BaseEntity columns sur toutes les entités ──────────────────────
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType, b =>
            {
                b.Property(nameof(BaseEntity.CreatedAt)).HasColumnName("created_at");
                b.Property(nameof(BaseEntity.UpdatedAt)).HasColumnName("updated_at");
                b.Property(nameof(BaseEntity.UserCreated)).HasColumnName("user_created");
                b.Property(nameof(BaseEntity.UserUpdated)).HasColumnName("user_updated");
            });
        }

        // ── Customer ───────────────────────────────────────────────────────
        modelBuilder.Entity<Customer>(b =>
        {
            b.ToTable("customers");
            b.HasKey(e => e.Id);
            b.Property(e => e.State).HasDefaultValue(true);
            b.HasMany(e => e.Accounts)
             .WithOne(a => a.Customer)
             .HasForeignKey(a => a.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Account ────────────────────────────────────────────────────────
        modelBuilder.Entity<Account>(b =>
        {
            b.ToTable("accounts");
            b.HasKey(e => e.Id);
            b.HasMany(e => e.Operations)
             .WithOne(o => o.Account)
             .HasForeignKey(o => o.AccountId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Operation ──────────────────────────────────────────────────────
        modelBuilder.Entity<Operation>(b =>
        {
            b.ToTable("operations");
            b.HasKey(e => e.Id);
            b.Property(e => e.Type)
             .HasConversion<string>();                          // ← EnumType.STRING
        });

        // ── Role ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles");
            b.HasKey(e => e.Id);
            b.HasIndex(e => e.Name).IsUnique();                // ← @UniqueConstraint
        });

        // ── User ───────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(e => e.Id);
            b.HasIndex(e => e.Email).IsUnique();
            b.HasIndex(e => e.Username).IsUnique();
            b.HasIndex(e => e.Phone).IsUnique();
            b.Property(e => e.IsAdmin).HasColumnName("is_admin").HasDefaultValue(false);
            b.Property(e => e.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true);
            b.Property(e => e.AccountIsEnabled).HasColumnName("account_is_enabled").HasDefaultValue(true);
            b.Property(e => e.AccountIsNotExpired).HasColumnName("account_is_not_expired").HasDefaultValue(true);
            b.Property(e => e.AccountIsNotLocked).HasColumnName("account_is_not_locked").HasDefaultValue(true);
            b.Property(e => e.CredentialNotExpired).HasColumnName("credential_not_expired").HasDefaultValue(true);
            b.HasOne(e => e.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(e => e.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ── @PrePersist / @PreUpdate ───────────────────────────────────────────
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UserCreated = username;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UserUpdated = username;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}