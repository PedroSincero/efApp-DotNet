using EFEnergiaAPI.Data;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Alerta;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Services.Alerta;

public class AlertaService : IAlertaService
{
    private readonly ApplicationDbContext _context;

    public AlertaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultViewModel<AlertaViewModel>> GetAlertasAsync(int page, int pageSize)
    {
        var query = _context.Alertas.Include(a => a.Equipamento).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AlertaViewModel
            {
                Id = a.Id,
                EquipamentoId = a.EquipamentoId,
                EquipamentoNome = a.Equipamento != null ? a.Equipamento.Nome : null,
                Mensagem = a.Mensagem,
                DataCriacao = a.DataCriacao,
                Resolvido = a.Resolvido
            })
            .ToListAsync();

        return new PagedResultViewModel<AlertaViewModel>
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<AlertaViewModel?> GetAlertaByIdAsync(int id)
    {
        var alerta = await _context.Alertas
            .Include(a => a.Equipamento)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alerta == null)
            return null;

        return new AlertaViewModel
        {
            Id = alerta.Id,
            EquipamentoId = alerta.EquipamentoId,
            EquipamentoNome = alerta.Equipamento?.Nome,
            Mensagem = alerta.Mensagem,
            DataCriacao = alerta.DataCriacao,
            Resolvido = alerta.Resolvido
        };
    }

    public async Task<AlertaViewModel> CreateAlertaAsync(AlertaCreateViewModel viewModel)
    {
        // Validar se o equipamento existe
        var equipamentoExiste = await _context.Equipamentos.AnyAsync(e => e.Id == viewModel.EquipamentoId);
        if (!equipamentoExiste)
        {
            throw new InvalidOperationException($"Equipamento com ID {viewModel.EquipamentoId} não encontrado.");
        }

        var alerta = new Api.Models.Alerta
        {
            EquipamentoId = viewModel.EquipamentoId,
            Mensagem = viewModel.Mensagem,
            DataCriacao = DateTime.UtcNow,
            Resolvido = false
        };

        _context.Alertas.Add(alerta);
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento
        await _context.Entry(alerta).Reference(a => a.Equipamento).LoadAsync();

        return new AlertaViewModel
        {
            Id = alerta.Id,
            EquipamentoId = alerta.EquipamentoId,
            EquipamentoNome = alerta.Equipamento?.Nome,
            Mensagem = alerta.Mensagem,
            DataCriacao = alerta.DataCriacao,
            Resolvido = alerta.Resolvido
        };
    }

    public async Task<AlertaViewModel?> UpdateAlertaAsync(AlertaUpdateViewModel viewModel)
    {
        var alerta = await _context.Alertas.FindAsync(viewModel.Id);
        
        if (alerta == null)
            return null;

        // Validar se o equipamento existe
        var equipamentoExiste = await _context.Equipamentos.AnyAsync(e => e.Id == viewModel.EquipamentoId);
        if (!equipamentoExiste)
        {
            throw new InvalidOperationException($"Equipamento com ID {viewModel.EquipamentoId} não encontrado.");
        }

        alerta.EquipamentoId = viewModel.EquipamentoId;
        alerta.Mensagem = viewModel.Mensagem;
        alerta.Resolvido = viewModel.Resolvido;
        
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento
        await _context.Entry(alerta).Reference(a => a.Equipamento).LoadAsync();

        return new AlertaViewModel
        {
            Id = alerta.Id,
            EquipamentoId = alerta.EquipamentoId,
            EquipamentoNome = alerta.Equipamento?.Nome,
            Mensagem = alerta.Mensagem,
            DataCriacao = alerta.DataCriacao,
            Resolvido = alerta.Resolvido
        };
    }

    public async Task<bool> DeleteAlertaAsync(int id)
    {
        var alerta = await _context.Alertas.FindAsync(id);
        
        if (alerta == null)
            return false;

        _context.Alertas.Remove(alerta);
        await _context.SaveChangesAsync();

        return true;
    }
}

