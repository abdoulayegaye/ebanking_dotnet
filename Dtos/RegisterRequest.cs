public record RegisterRequest(
    string Firstname,
    string Lastname,
    string Email,
    string Phone,
    string Username,
    string Password,
    string ConfirmPassword,
    long RoleId              // ← le rôle est assigné à la création
);