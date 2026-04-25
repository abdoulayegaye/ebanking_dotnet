public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Vérifier si des rôles existent déjà
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Name = "ADMIN", Description = "Administrateur système" },
            new() { Name = "USER",  Description = "Utilisateur standard" },
            new() { Name = "AGENT", Description = "Agent bancaire" }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}