using EFEnergiaAPI.Data;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Leitura;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Services.Leitura;

public class LeituraService : ILeituraService
{
    private readonly ApplicationDbContext _context;

    public LeituraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultViewModel<LeituraViewModel>> GetLeiturasAsync(int page, int pageSize)
    {
        var query = _context.Leituras.Include(l => l.Equipamento).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.DataRegistro)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LeituraViewModel
            {
                Id = l.Id,
                EquipamentoId = l.EquipamentoId,
                EquipamentoNome = l.Equipamento != null ? l.Equipamento.Nome : null,
                Temperatura = l.Temperatura,
                DataRegistro = l.DataRegistro
            })
            .ToListAsync();

        return new PagedResultViewModel<LeituraViewModel>
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<LeituraViewModel?> GetLeituraByIdAsync(int id)
    {
        var leitura = await _context.Leituras
            .Include(l => l.Equipamento)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leitura == null)
            return null;

        return new LeituraViewModel
        {
            Id = leitura.Id,
            EquipamentoId = leitura.EquipamentoId,
            EquipamentoNome = leitura.Equipamento?.Nome,
            Temperatura = leitura.Temperatura,
            DataRegistro = leitura.DataRegistro
        };
    }

    public async Task<LeituraViewModel> CreateLeituraAsync(LeituraCreateViewModel viewModel)
    {
        // Validar se o equipamento existe
        var equipamentoExiste = await _context.Equipamentos.AnyAsync(e => e.Id == viewModel.EquipamentoId);
        if (!equipamentoExiste)
        {
            throw new InvalidOperationException($"Equipamento com ID {viewModel.EquipamentoId} não encontrado.");
        }

        // Validar temperatura (regra de negócio: alerta se temperatura > 80 ou < 0)
        if (viewModel.Temperatura > 80 || viewModel.Temperatura < 0)
        {
            // Criar alerta automático
            var alerta = new Api.Models.Alerta
            {
                EquipamentoId = viewModel.EquipamentoId,
                Mensagem = $"Temperatura crítica detectada: {viewModel.Temperatura}°C",
                DataCriacao = DateTime.UtcNow,
                Resolvido = false
            };
            _context.Alertas.Add(alerta);
        }

        var leitura = new Api.Models.Leitura
        {
            EquipamentoId = viewModel.EquipamentoId,
            Temperatura = viewModel.Temperatura,
            DataRegistro = DateTime.UtcNow
        };

        _context.Leituras.Add(leitura);
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento
        await _context.Entry(leitura).Reference(l => l.Equipamento).LoadAsync();

        return new LeituraViewModel
        {
            Id = leitura.Id,
            EquipamentoId = leitura.EquipamentoId,
            EquipamentoNome = leitura.Equipamento?.Nome,
            Temperatura = leitura.Temperatura,
            DataRegistro = leitura.DataRegistro
        };
    }

    public async Task<LeituraViewModel?> UpdateLeituraAsync(LeituraUpdateViewModel viewModel)
    {
        var leitura = await _context.Leituras.FindAsync(viewModel.Id);
        
        if (leitura == null)
            return null;

        // Validar se o equipamento existe
        var equipamentoExiste = await _context.Equipamentos.AnyAsync(e => e.Id == viewModel.EquipamentoId);
        if (!equipamentoExiste)
        {
            throw new InvalidOperationException($"Equipamento com ID {viewModel.EquipamentoId} não encontrado.");
        }

        leitura.EquipamentoId = viewModel.EquipamentoId;
        leitura.Temperatura = viewModel.Temperatura;
        
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento
        await _context.Entry(leitura).Reference(l => l.Equipamento).LoadAsync();

        return new LeituraViewModel
        {
            Id = leitura.Id,
            EquipamentoId = leitura.EquipamentoId,
            EquipamentoNome = leitura.Equipamento?.Nome,
            Temperatura = leitura.Temperatura,
            DataRegistro = leitura.DataRegistro
        };
    }

    public async Task<bool> DeleteLeituraAsync(int id)
    {
        var leitura = await _context.Leituras.FindAsync(id);
        
        if (leitura == null)
            return false;

        _context.Leituras.Remove(leitura);
        await _context.SaveChangesAsync();

        return true;
    }
}

