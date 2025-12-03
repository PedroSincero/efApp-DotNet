using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFEnergiaAPI.Data;
using Api.Models;


namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LeiturasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Leituras?page=1&pageSize=10
    [HttpGet]
    [Authorize] // Visualizar leituras requer autenticação
    public async Task<IActionResult> GetLeituras(int page = 1, int pageSize = 10)
    {
        var query = _context.Leituras
            .Include(l => l.Equipamento)
            .AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.DataRegistro)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { totalItems, page, pageSize, items });
    }

    // GET: api/Leituras/5
    [HttpGet("{id}")]
    [Authorize] // Detalhes da leitura requerem autenticação
    public async Task<IActionResult> GetLeitura(int id)
    {
        var leitura = await _context.Leituras
            .Include(l => l.Equipamento)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leitura == null)
            return NotFound("Leitura não encontrada.");

        return Ok(leitura);
    }

    // POST: api/Leituras
    [HttpPost]
    [Authorize] // Criar leitura requer autenticação
    public async Task<IActionResult> CreateLeitura([FromBody] Leitura leitura)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Leituras.Add(leitura);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLeitura), new { id = leitura.Id }, leitura);
    }

    // PUT: api/Leituras/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar leitura requer autenticação
    public async Task<IActionResult> UpdateLeitura(int id, [FromBody] Leitura leitura)
    {
        if (id != leitura.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(leitura).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Leituras.Any(e => e.Id == id))
                return NotFound("Leitura não encontrada.");

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Leituras/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar leitura requer autenticação
    public async Task<IActionResult> DeleteLeitura(int id)
    {
        var leitura = await _context.Leituras.FindAsync(id);

        if (leitura == null)
            return NotFound("Leitura não encontrada.");

        _context.Leituras.Remove(leitura);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
