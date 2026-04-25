[ApiController]
[Authorize]
[Route("api/v1/roles")]
[Tags("Roles")]
[Produces("application/json")]
public class RoleController
    : GenericController<Role, RoleDto, CreateRoleDto, UpdateRoleDto>
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService service) : base(service)
    {
        _roleService = service;
    }

    [HttpGet("name/{name}")]
    [EndpointSummary("Rechercher un rôle par nom")]
    [EndpointDescription("Retourne le rôle correspondant au nom fourni. Le nom est unique. Retourne 404 si introuvable.")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetByName(string name)
    {
        var role = await _roleService.GetByNameAsync(name);
        return role is null ? NotFound() : Ok(role);
    }
}