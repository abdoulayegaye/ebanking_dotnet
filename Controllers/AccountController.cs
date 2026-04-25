[ApiController]
[Authorize]
[Route("api/v1/accounts")]
[Tags("Accounts")]
[Produces("application/json")]
public class AccountController
    : GenericController<Account, AccountDto, CreateAccountDto, UpdateAccountDto>
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService service) : base(service)
    {
        _accountService = service;
    }

    [HttpGet("numero/{numero}")]
    [EndpointSummary("Rechercher un compte par numéro")]
    [EndpointDescription("Retourne le compte bancaire correspondant au numéro fourni. Retourne 404 si introuvable.")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetByNumero(string numero)
    {
        var account = await _accountService.GetByNumeroAsync(numero);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpGet("{id:long}/operations")]
    [EndpointSummary("Lister les opérations d'un compte")]
    [EndpointDescription("Retourne l'historique complet des opérations (débits/crédits) associées au compte identifié par son ID.")]
    [ProducesResponseType(typeof(List<OperationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<OperationDto>>> GetOperations(long id) =>
        Ok(await _accountService.GetOperationsAsync(id));
}