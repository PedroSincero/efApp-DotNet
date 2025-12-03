using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Services.Equipamento;
using EFEnergiaAPI.ViewModels.Equipamento;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipamentosController : ControllerBase
{
    private readonly IEquipamentoService _equipamentoService;

    public EquipamentosController(IEquipamentoService equipamentoService)
    {
        _equipamentoService = equipamentoService;
    }

    // GET: api/Equipamentos?page=1&pageSize=10
    [HttpGet]
    [AllowAnonymous] // Listagem pode ser pública
    public async Task<IActionResult> GetEquipamentos(int page = 1, int pageSize = 10)
    {
        var result = await _equipamentoService.GetEquipamentosAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/Equipamentos/5
    [HttpGet("{id}")]
    [AllowAnonymous] // Detalhes podem ser públicos
    public async Task<IActionResult> GetEquipamento(int id)
    {
        var equipamento = await _equipamentoService.GetEquipamentoByIdAsync(id);

        if (equipamento == null)
            return NotFound("Equipamento não encontrado.");

        return Ok(equipamento);
    }

    // POST: api/Equipamentos
    [HttpPost]
    [Authorize] // Criar equipamento requer autenticação
    public async Task<IActionResult> CreateEquipamento([FromBody] EquipamentoCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var equipamento = await _equipamentoService.CreateEquipamentoAsync(viewModel);
            return CreatedAtAction(nameof(GetEquipamento), new { id = equipamento.Id }, equipamento);
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

    // PUT: api/Equipamentos/5
    [HttpPut("{id}")]
    [Authorize] // Atualizar equipamento requer autenticação
    public async Task<IActionResult> UpdateEquipamento(int id, [FromBody] EquipamentoUpdateViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest("ID da URL diferente do body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var equipamento = await _equipamentoService.UpdateEquipamentoAsync(viewModel);

            if (equipamento == null)
                return NotFound("Equipamento não encontrado.");

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

    // DELETE: api/Equipamentos/5
    [HttpDelete("{id}")]
    [Authorize] // Deletar equipamento requer autenticação
    public async Task<IActionResult> DeleteEquipamento(int id)
    {
        var deleted = await _equipamentoService.DeleteEquipamentoAsync(id);

        if (!deleted)
            return NotFound("Equipamento não encontrado.");

        return NoContent();
    }
}

