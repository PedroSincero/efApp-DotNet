using EFEnergiaAPI.Data;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Setor;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Services.Setor;

public class SetorService : ISetorService
{
    private readonly ApplicationDbContext _context;

    public SetorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultViewModel<SetorViewModel>> GetSetoresAsync(int page, int pageSize)
    {
        var query = _context.Setores.AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SetorViewModel
            {
                Id = s.Id,
                Nome = s.Nome
            })
            .ToListAsync();

        return new PagedResultViewModel<SetorViewModel>
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<SetorViewModel?> GetSetorByIdAsync(int id)
    {
        var setor = await _context.Setores.FindAsync(id);
        
        if (setor == null)
            return null;

        return new SetorViewModel
        {
            Id = setor.Id,
            Nome = setor.Nome
        };
    }

    public async Task<SetorViewModel> CreateSetorAsync(SetorCreateViewModel viewModel)
    {
        var setor = new Api.Models.Setor
        {
            Nome = viewModel.Nome
        };

        _context.Setores.Add(setor);
        await _context.SaveChangesAsync();

        return new SetorViewModel
        {
            Id = setor.Id,
            Nome = setor.Nome
        };
    }

    public async Task<SetorViewModel?> UpdateSetorAsync(SetorUpdateViewModel viewModel)
    {
        var setor = await _context.Setores.FindAsync(viewModel.Id);
        
        if (setor == null)
            return null;

        setor.Nome = viewModel.Nome;
        
        await _context.SaveChangesAsync();

        return new SetorViewModel
        {
            Id = setor.Id,
            Nome = setor.Nome
        };
    }

    public async Task<bool> DeleteSetorAsync(int id)
    {
        var setor = await _context.Setores.FindAsync(id);
        
        if (setor == null)
            return false;

        _context.Setores.Remove(setor);
        await _context.SaveChangesAsync();

        return true;
    }
}

