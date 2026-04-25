[ApiController]
public abstract class GenericController<TEntity, TDto, TCreateDto, TUpdateDto>
    : ControllerBase
    where TEntity : BaseEntity
{
    protected readonly IGenericService<TEntity, TDto, TCreateDto, TUpdateDto> _service;

    protected GenericController(IGenericService<TEntity, TDto, TCreateDto, TUpdateDto> service)
    {
        _service = service;
    }

    [HttpGet]
    [EndpointSummary("Lister tous les éléments")]
    [EndpointDescription("Retourne la liste complète des éléments sans pagination.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<List<TDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    [EndpointSummary("Récupérer un élément par ID")]
    [EndpointDescription("Retourne un élément unique identifié par son ID. Retourne 404 si introuvable.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> GetById(long id) =>
        Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [EndpointSummary("Créer un nouvel élément")]
    [EndpointDescription("Crée un nouvel élément et le persiste en base de données. Retourne l'élément créé.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Created(string.Empty, created);
    }

    [HttpPut("{id:long}")]
    [EndpointSummary("Mettre à jour un élément")]
    [EndpointDescription("Met à jour les informations d'un élément existant identifié par son ID.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> Update(long id, [FromBody] TUpdateDto dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:long}")]
    [EndpointSummary("Supprimer un élément")]
    [EndpointDescription("Supprime définitivement un élément identifié par son ID. Retourne 204 si succès.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}