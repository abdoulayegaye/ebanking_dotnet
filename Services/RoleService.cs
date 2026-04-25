using Ebank.API.Entities;

public class RoleService
    : GenericService<Role, RoleDto, CreateRoleDto, UpdateRoleDto>,
      IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository repository) : base(repository)
    {
        _roleRepository = repository;
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await _roleRepository.GetByNameAsync(name);
        return role is null ? null : MapToDto(role);
    }

    protected override RoleDto MapToDto(Role e) =>
        new(e.Id, e.Name, e.Description);

    protected override Role MapToEntity(CreateRoleDto dto) =>
        new() { Name = dto.Name, Description = dto.Description };

    protected override void ApplyUpdate(Role entity, UpdateRoleDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}