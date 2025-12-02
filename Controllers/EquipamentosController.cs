using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFEnergiaAPI.Data;
using Api.Models;


namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipamentosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EquipamentosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Equipamentos?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetEquipamentos(int page = 1, int pageSize = 10)
    {
        var query = _context.Equipamentos.Include(e => e.Setor).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { totalItems, page, pageSize, items });
    }

    // GET: api/Equipamentos/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEquipamento(int id)
    {
        var equipamento = await _context.Equipamentos
            .Include(e => e.Setor)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (equipamento == null)
            return NotFound("Equipamento não encontrado.");

        return Ok(equipamento);
    }

    // POST: api/Equipamentos
    [HttpPost]
    public async Task<IActionResult> CreateEquipamento([FromBody] Equipamento equipamento)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Equipamentos.Add(equipamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEquipamento), new { id = equipamento.Id }, equipamento);
    }

    // PUT: api/Equipamentos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEquipamento(int id, [FromBody] Equipamento equipamento)
    {
        if (id != equipamento.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(equipamento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Equipamentos.Any(e => e.Id == id))
                return NotFound("Equipamento não encontrado.");

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Equipamentos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipamento(int id)
    {
        var equipamento = await _context.Equipamentos.FindAsync(id);

        if (equipamento == null)
            return NotFound("Equipamento não encontrado.");

        _context.Equipamentos.Remove(equipamento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
