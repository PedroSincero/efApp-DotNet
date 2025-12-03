using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFEnergiaAPI.Data;
using Api.Models;


namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SetoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Setores?page=1&pageSize=10
    [HttpGet]
    [AllowAnonymous] // Listagem de setores pode ser pública
    public async Task<IActionResult> GetSetores(int page = 1, int pageSize = 10)
    {
        var query = _context.Setores.AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { totalItems, page, pageSize, items });
    }

    // GET: api/Setores/5
    [HttpGet("{id}")]
    [AllowAnonymous] // Detalhes do setor podem ser públicos
    public async Task<IActionResult> GetSetor(int id)
    {
        var setor = await _context.Setores.FindAsync(id);

        if (setor == null)
            return NotFound("Setor não encontrado.");

        return Ok(setor);
    }

    // POST: api/Setores
    [HttpPost]
    [Authorize] // Criar setor requer autenticação
    public async Task<IActionResult> CreateSetor([FromBody] Setor setor)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Setores.Add(setor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSetor), new { id = setor.Id }, setor);
    }

    // PUT: api/Setores/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar setor requer autenticação
    public async Task<IActionResult> UpdateSetor(int id, [FromBody] Setor setor)
    {
        if (id != setor.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(setor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Setores.Any(e => e.Id == id))
                return NotFound("Setor não encontrado.");

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Setores/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar setor requer autenticação
    public async Task<IActionResult> DeleteSetor(int id)
    {
        var setor = await _context.Setores.FindAsync(id);

        if (setor == null)
            return NotFound("Setor não encontrado.");

        _context.Setores.Remove(setor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
