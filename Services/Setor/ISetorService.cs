using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Setor;

namespace EFEnergiaAPI.Services.Setor;

public interface ISetorService
{
    Task<PagedResultViewModel<SetorViewModel>> GetSetoresAsync(int page, int pageSize);
    Task<SetorViewModel?> GetSetorByIdAsync(int id);
    Task<SetorViewModel> CreateSetorAsync(SetorCreateViewModel viewModel);
    Task<SetorViewModel?> UpdateSetorAsync(SetorUpdateViewModel viewModel);
    Task<bool> DeleteSetorAsync(int id);
}

