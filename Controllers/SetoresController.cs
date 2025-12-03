using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Services.Setor;
using EFEnergiaAPI.ViewModels.Setor;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetoresController : ControllerBase
{
    private readonly ISetorService _setorService;

    public SetoresController(ISetorService setorService)
    {
        _setorService = setorService;
    }

    // GET: api/Setores?page=1&pageSize=10
    [HttpGet]
    [AllowAnonymous] // Listagem de setores pode ser pública
    public async Task<IActionResult> GetSetores(int page = 1, int pageSize = 10)
    {
        var result = await _setorService.GetSetoresAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/Setores/5
    [HttpGet("{id}")]
    [AllowAnonymous] // Detalhes do setor podem ser públicos
    public async Task<IActionResult> GetSetor(int id)
    {
        var setor = await _setorService.GetSetorByIdAsync(id);

        if (setor == null)
            return NotFound("Setor não encontrado.");

        return Ok(setor);
    }

    // POST: api/Setores
    [HttpPost]
    [Authorize] // Criar setor requer autenticação
    public async Task<IActionResult> CreateSetor([FromBody] SetorCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var setor = await _setorService.CreateSetorAsync(viewModel);
            return CreatedAtAction(nameof(GetSetor), new { id = setor.Id }, setor);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Setores/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar setor requer autenticação
    public async Task<IActionResult> UpdateSetor(int id, [FromBody] SetorUpdateViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var setor = await _setorService.UpdateSetorAsync(viewModel);

            if (setor == null)
                return NotFound("Setor não encontrado.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Setores/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar setor requer autenticação
    public async Task<IActionResult> DeleteSetor(int id)
    {
        var deleted = await _setorService.DeleteSetorAsync(id);

        if (!deleted)
            return NotFound("Setor não encontrado.");

        return NoContent();
    }
}
