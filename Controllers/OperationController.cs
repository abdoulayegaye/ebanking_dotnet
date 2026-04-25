[ApiController]
[Authorize]
[Route("api/v1/operations")]
[Tags("Operations")]
[Produces("application/json")]
public class OperationController
    : GenericController<Operation, OperationDto, CreateOperationDto, UpdateOperationDto>
{
    private readonly IOperationService _operationService;

    public OperationController(IOperationService service) : base(service)
    {
        _operationService = service;
    }

    [HttpGet("type/{type}")]
    [EndpointSummary("Filtrer les opérations par type")]
    [EndpointDescription("Retourne la liste des opérations filtrées par type (DEBIT ou CREDIT).")]
    [ProducesResponseType(typeof(List<OperationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OperationDto>>> GetByType(TypeOperation type) =>
        Ok(await _operationService.GetByTypeAsync(type));
}