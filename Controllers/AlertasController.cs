using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Services.Alerta;
using EFEnergiaAPI.ViewModels.Alerta;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertasController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    // GET: api/Alertas?page=1&pageSize=10
    [HttpGet]
    [Authorize] // Visualizar alertas requer autenticação
    public async Task<IActionResult> GetAlertas(int page = 1, int pageSize = 10)
    {
        var result = await _alertaService.GetAlertasAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/Alertas/5
    [HttpGet("{id}")]
    [Authorize] // Detalhes do alerta requerem autenticação
    public async Task<IActionResult> GetAlerta(int id)
    {
        var alerta = await _alertaService.GetAlertaByIdAsync(id);

        if (alerta == null)
            return NotFound("Alerta não encontrado.");

        return Ok(alerta);
    }

    // POST: api/Alertas
    [HttpPost]
    [Authorize] // Criar alerta requer autenticação
    public async Task<IActionResult> CreateAlerta([FromBody] AlertaCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var alerta = await _alertaService.CreateAlertaAsync(viewModel);
            return CreatedAtAction(nameof(GetAlerta), new { id = alerta.Id }, alerta);
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

    // PUT: api/Alertas/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar alerta requer autenticação
    public async Task<IActionResult> UpdateAlerta(int id, [FromBody] AlertaUpdateViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var alerta = await _alertaService.UpdateAlertaAsync(viewModel);

            if (alerta == null)
                return NotFound("Alerta não encontrado.");

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

    // DELETE: api/Alertas/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar alerta requer autenticação
    public async Task<IActionResult> DeleteAlerta(int id)
    {
        var deleted = await _alertaService.DeleteAlertaAsync(id);

        if (!deleted)
            return NotFound("Alerta não encontrado.");

        return NoContent();
    }
}

