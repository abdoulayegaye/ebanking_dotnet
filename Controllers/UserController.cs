[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/v1/users")]
[Tags("Users")]
[Produces("application/json")]
public class UserController
    : GenericController<User, UserDto, CreateUserDto, UpdateUserDto>
{
    private readonly IUserService _userService;

    public UserController(IUserService service) : base(service)
    {
        _userService = service;
    }

    [HttpGet("username/{username}")]
    [EndpointSummary("Rechercher un utilisateur par username")]
    [EndpointDescription("Retourne l'utilisateur correspondant au username fourni. Retourne 404 si introuvable.")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("email/{email}")]
    [EndpointSummary("Rechercher un utilisateur par email")]
    [EndpointDescription("Retourne l'utilisateur correspondant à l'adresse email fournie. Retourne 404 si introuvable.")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        return user is null ? NotFound() : Ok(user);
    }
}