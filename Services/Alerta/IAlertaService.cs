using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Alerta;

namespace EFEnergiaAPI.Services.Alerta;

public interface IAlertaService
{
    Task<PagedResultViewModel<AlertaViewModel>> GetAlertasAsync(int page, int pageSize);
    Task<AlertaViewModel?> GetAlertaByIdAsync(int id);
    Task<AlertaViewModel> CreateAlertaAsync(AlertaCreateViewModel viewModel);
    Task<AlertaViewModel?> UpdateAlertaAsync(AlertaUpdateViewModel viewModel);
    Task<bool> DeleteAlertaAsync(int id);
}

