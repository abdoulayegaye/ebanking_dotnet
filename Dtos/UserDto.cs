public record UserDto(long Id, string Firstname, string Lastname, string Email, string Username, string Phone, bool IsAdmin, bool IsEnabled, long RoleId);
public record CreateUserDto(string Firstname, string Lastname, string Email, string Username, string Phone, string Password, long RoleId);
public record UpdateUserDto(string Firstname, string Lastname, string Email, string Phone);