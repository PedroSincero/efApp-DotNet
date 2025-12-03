using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Services.Leitura;
using EFEnergiaAPI.ViewModels.Leitura;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _leituraService;

    public LeiturasController(ILeituraService leituraService)
    {
        _leituraService = leituraService;
    }

    // GET: api/Leituras?page=1&pageSize=10
    [HttpGet]
    [Authorize] // Visualizar leituras requer autenticação
    public async Task<IActionResult> GetLeituras(int page = 1, int pageSize = 10)
    {
        var result = await _leituraService.GetLeiturasAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/Leituras/5
    [HttpGet("{id}")]
    [Authorize] // Detalhes da leitura requerem autenticação
    public async Task<IActionResult> GetLeitura(int id)
    {
        var leitura = await _leituraService.GetLeituraByIdAsync(id);

        if (leitura == null)
            return NotFound("Leitura não encontrada.");

        return Ok(leitura);
    }

    // POST: api/Leituras
    [HttpPost]
    [Authorize] // Criar leitura requer autenticação
    public async Task<IActionResult> CreateLeitura([FromBody] LeituraCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var leitura = await _leituraService.CreateLeituraAsync(viewModel);
            return CreatedAtAction(nameof(GetLeitura), new { id = leitura.Id }, leitura);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Leituras/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar leitura requer autenticação
    public async Task<IActionResult> UpdateLeitura(int id, [FromBody] LeituraUpdateViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var leitura = await _leituraService.UpdateLeituraAsync(viewModel);

            if (leitura == null)
                return NotFound("Leitura não encontrada.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Leituras/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar leitura requer autenticação
    public async Task<IActionResult> DeleteLeitura(int id)
    {
        var deleted = await _leituraService.DeleteLeituraAsync(id);

        if (!deleted)
            return NotFound("Leitura não encontrada.");

        return NoContent();
    }
}

