using Ebank.API.Entities;

public interface IRoleService
    : IGenericService<Role, RoleDto, CreateRoleDto, UpdateRoleDto>
{
    Task<RoleDto?> GetByNameAsync(string name);
}