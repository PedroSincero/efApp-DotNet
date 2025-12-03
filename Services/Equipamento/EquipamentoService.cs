using EFEnergiaAPI.Data;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Equipamento;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Services.Equipamento;

public class EquipamentoService : IEquipamentoService
{
    private readonly ApplicationDbContext _context;

    public EquipamentoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultViewModel<EquipamentoViewModel>> GetEquipamentosAsync(int page, int pageSize)
    {
        var query = _context.Equipamentos.Include(e => e.Setor).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EquipamentoViewModel
            {
                Id = e.Id,
                Nome = e.Nome,
                SetorId = e.SetorId,
                SetorNome = e.Setor != null ? e.Setor.Nome : null
            })
            .ToListAsync();

        return new PagedResultViewModel<EquipamentoViewModel>
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<EquipamentoViewModel?> GetEquipamentoByIdAsync(int id)
    {
        var equipamento = await _context.Equipamentos
            .Include(e => e.Setor)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (equipamento == null)
            return null;

        return new EquipamentoViewModel
        {
            Id = equipamento.Id,
            Nome = equipamento.Nome,
            SetorId = equipamento.SetorId,
            SetorNome = equipamento.Setor?.Nome
        };
    }

    public async Task<EquipamentoViewModel> CreateEquipamentoAsync(EquipamentoCreateViewModel viewModel)
    {
        // Validar se o setor existe
        var setorExiste = await _context.Setores.AnyAsync(s => s.Id == viewModel.SetorId);
        if (!setorExiste)
        {
            throw new InvalidOperationException($"Setor com ID {viewModel.SetorId} não encontrado.");
        }

        var equipamento = new Api.Models.Equipamento
        {
            Nome = viewModel.Nome,
            SetorId = viewModel.SetorId
        };

        _context.Equipamentos.Add(equipamento);
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento para retornar nome do setor
        await _context.Entry(equipamento).Reference(e => e.Setor).LoadAsync();

        return new EquipamentoViewModel
        {
            Id = equipamento.Id,
            Nome = equipamento.Nome,
            SetorId = equipamento.SetorId,
            SetorNome = equipamento.Setor?.Nome
        };
    }

    public async Task<EquipamentoViewModel?> UpdateEquipamentoAsync(EquipamentoUpdateViewModel viewModel)
    {
        var equipamento = await _context.Equipamentos.FindAsync(viewModel.Id);
        
        if (equipamento == null)
            return null;

        // Validar se o setor existe
        var setorExiste = await _context.Setores.AnyAsync(s => s.Id == viewModel.SetorId);
        if (!setorExiste)
        {
            throw new InvalidOperationException($"Setor com ID {viewModel.SetorId} não encontrado.");
        }

        equipamento.Nome = viewModel.Nome;
        equipamento.SetorId = viewModel.SetorId;
        
        await _context.SaveChangesAsync();

        // Recarregar com relacionamento
        await _context.Entry(equipamento).Reference(e => e.Setor).LoadAsync();

        return new EquipamentoViewModel
        {
            Id = equipamento.Id,
            Nome = equipamento.Nome,
            SetorId = equipamento.SetorId,
            SetorNome = equipamento.Setor?.Nome
        };
    }

    public async Task<bool> DeleteEquipamentoAsync(int id)
    {
        var equipamento = await _context.Equipamentos.FindAsync(id);
        
        if (equipamento == null)
            return false;

        _context.Equipamentos.Remove(equipamento);
        await _context.SaveChangesAsync();

        return true;
    }
}

