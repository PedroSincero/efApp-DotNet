using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFEnergiaAPI.Data;
using Api.Models;


namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AlertasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Alertas?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAlertas(int page = 1, int pageSize = 10)
    {
        var query = _context.Alertas
            .Include(a => a.Equipamento)
            .AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { totalItems, page, pageSize, items });
    }

    // GET: api/Alertas/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlerta(int id)
    {
        var alerta = await _context.Alertas
            .Include(a => a.Equipamento)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alerta == null)
            return NotFound("Alerta não encontrado.");

        return Ok(alerta);
    }

    // POST: api/Alertas
    [HttpPost]
    public async Task<IActionResult> CreateAlerta([FromBody] Alerta alerta)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        alerta.DataCriacao = DateTime.UtcNow;

        _context.Alertas.Add(alerta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAlerta), new { id = alerta.Id }, alerta);
    }

    // PUT: api/Alertas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAlerta(int id, [FromBody] Alerta alerta)
    {
        if (id != alerta.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(alerta).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Alertas.Any(e => e.Id == id))
                return NotFound("Alerta não encontrado.");

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Alertas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlerta(int id)
    {
        var alerta = await _context.Alertas.FindAsync(id);

        if (alerta == null)
            return NotFound("Alerta não encontrado.");

        _context.Alertas.Remove(alerta);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
