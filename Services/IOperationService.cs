using Ebank.API.Entities;

public interface IOperationService
    : IGenericService<Operation, OperationDto, CreateOperationDto, UpdateOperationDto>
{
    Task<List<OperationDto>> GetByTypeAsync(TypeOperation type);
}