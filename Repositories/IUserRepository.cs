using Ebank.API.Entities;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
}