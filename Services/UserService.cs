using Ebank.API.Entities;

public class UserService
    : GenericService<User, UserDto, CreateUserDto, UpdateUserDto>,
      IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository repository) : base(repository)
    {
        _userRepository = repository;
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user is null ? null : MapToDto(user);
    }

    protected override UserDto MapToDto(User e) =>
        new(e.Id, e.Firstname, e.Lastname, e.Email, e.Username, e.Phone, e.IsAdmin, e.IsEnabled, e.RoleId);

    protected override User MapToEntity(CreateUserDto dto) => new()
    {
        Firstname = dto.Firstname,
        Lastname = dto.Lastname,
        Email = dto.Email,
        Username = dto.Username,
        Phone = dto.Phone,
        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),  // ← PasswordEncoder
        RoleId = dto.RoleId
    };

    protected override void ApplyUpdate(User entity, UpdateUserDto dto)
    {
        entity.Firstname = dto.Firstname;
        entity.Lastname = dto.Lastname;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
    }
}