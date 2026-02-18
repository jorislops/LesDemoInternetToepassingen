using EFLesDemo.Repositories;
using EFLesDemo.Services;
using ScrumboardApi.Dtos.Request;
using ScrumboardApi.Dtos.Response;
using ScrumboardApi.Mappings;

namespace ScrumboardApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScrumboardCardController : ControllerBase
{
    private readonly IScrumCardRepository _repo;
    private readonly IScrumboardService _scrumboardService;

    public ScrumboardCardController(IScrumCardRepository repo, IScrumboardService scrumboardService)
    {
        _repo = repo;
        _scrumboardService = scrumboardService;
    }

    // GET: api/ScrumboardCard
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScrumboardCardResponseDto>>> GetAll(CancellationToken ct)
    {
        var cards = await _repo.ListAsync(ct);
        return Ok(cards.Select(c => c.ToResponseDto()));
    }

    // GET: api/ScrumboardCard/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScrumboardCardResponseDto>> GetById(int id, CancellationToken ct)
    {
        var spec = new ScrumboardSpecs.ScrumboardCardByIdSpec(id);
        var card = await _repo.FirstOrDefaultAsync(spec, ct);

        if (card == null)
            return NotFound();

        return Ok(card.ToResponseDto());
    }

    // POST: api/ScrumboardCard
    [HttpPost]
    public async Task<ActionResult<ScrumboardCardResponseDto>> Create(
        ScrumboardCardRequestPostDto postDto,
        CancellationToken ct)
    {
        var card = postDto.ToEntity();
        var addedCard = _scrumboardService.InsertOrUpdateCardPreserveOrder(card);
        return CreatedAtAction(nameof(GetById), new { id = addedCard.Id }, addedCard.ToResponseDto());
        
        // var entity = dto.ToEntity();
        // await _repo.AddAsync(entity, ct);
        // await _repo.SaveChangesAsync(ct);
        //
        // return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToResponseDto());
    }

    // PUT: api/ScrumboardCard/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        ScrumboardCardRequestUpdateDto updateDto,
        CancellationToken ct)
    {
        
        var spec = new ScrumboardSpecs.ScrumboardCardByIdSpec(id);
        var card = await _repo.FirstOrDefaultAsync(spec, ct);

        if (card == null)
            return NotFound();
        
        var entity = updateDto.ToEntity();
        var _ = _scrumboardService.InsertOrUpdateCardPreserveOrder(entity);

        return NoContent();
    }

    // DELETE: api/ScrumboardCard/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var spec = new ScrumboardSpecs.ScrumboardCardByIdSpec(id);
        var card = await _repo.FirstOrDefaultAsync(spec, ct);

        if (card == null)
            return NotFound();

        await _repo.DeleteAsync(card, ct);
        await _repo.SaveChangesAsync(ct);

        return NoContent();
    }
}