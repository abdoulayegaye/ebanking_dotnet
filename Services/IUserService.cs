using Ebank.API.Entities;

public interface IUserService
    : IGenericService<User, UserDto, CreateUserDto, UpdateUserDto>
{
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<UserDto?> GetByEmailAsync(string email);
}