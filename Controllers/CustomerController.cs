[ApiController]
[Authorize]
[Route("api/v1/customers")]
[Tags("Customers")]
[Produces("application/json")]
public class CustomerController
    : GenericController<Customer, CustomerDto, CreateCustomerDto, UpdateCustomerDto>
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService service) : base(service)
    {
        _customerService = service;
    }

    [HttpGet("email/{email}")]
    [EndpointSummary("Rechercher un client par email")]
    [EndpointDescription("Retourne le client correspondant à l'adresse email fournie. Retourne 404 si aucun client trouvé.")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetByEmail(string email)
    {
        var customer = await _customerService.GetByEmailAsync(email);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet("{id:long}/accounts")]
    [EndpointSummary("Lister les comptes d'un client")]
    [EndpointDescription("Retourne la liste de tous les comptes bancaires associés au client identifié par son ID.")]
    [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AccountDto>>> GetAccounts(long id) =>
        Ok(await _customerService.GetAccountsAsync(id));
}