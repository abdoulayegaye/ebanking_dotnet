using Ebank.API.Entities;

public class OperationService
    : GenericService<Operation, OperationDto, CreateOperationDto, UpdateOperationDto>,
      IOperationService
{
    private readonly IOperationRepository _operationRepository;

    public OperationService(IOperationRepository repository) : base(repository)
    {
        _operationRepository = repository;
    }

    public async Task<List<OperationDto>> GetByTypeAsync(TypeOperation type)
    {
        var operations = await _operationRepository.GetByTypeAsync(type);
        return operations.Select(MapToDto).ToList();
    }

    protected override OperationDto MapToDto(Operation e) =>
        new(e.Id, e.Numero, e.Type, e.Amount, e.AccountId);

    protected override Operation MapToEntity(CreateOperationDto dto) =>
        new() { Numero = dto.Numero, Type = dto.Type, Amount = dto.Amount, AccountId = dto.AccountId };

    protected override void ApplyUpdate(Operation entity, UpdateOperationDto dto)
    {
        entity.Type = dto.Type;
        entity.Amount = dto.Amount;
    }
}