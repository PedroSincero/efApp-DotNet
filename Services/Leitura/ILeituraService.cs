using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Leitura;

namespace EFEnergiaAPI.Services.Leitura;

public interface ILeituraService
{
    Task<PagedResultViewModel<LeituraViewModel>> GetLeiturasAsync(int page, int pageSize);
    Task<LeituraViewModel?> GetLeituraByIdAsync(int id);
    Task<LeituraViewModel> CreateLeituraAsync(LeituraCreateViewModel viewModel);
    Task<LeituraViewModel?> UpdateLeituraAsync(LeituraUpdateViewModel viewModel);
    Task<bool> DeleteLeituraAsync(int id);
}

