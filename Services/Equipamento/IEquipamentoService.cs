using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Equipamento;

namespace EFEnergiaAPI.Services.Equipamento;

public interface IEquipamentoService
{
    Task<PagedResultViewModel<EquipamentoViewModel>> GetEquipamentosAsync(int page, int pageSize);
    Task<EquipamentoViewModel?> GetEquipamentoByIdAsync(int id);
    Task<EquipamentoViewModel> CreateEquipamentoAsync(EquipamentoCreateViewModel viewModel);
    Task<EquipamentoViewModel?> UpdateEquipamentoAsync(EquipamentoUpdateViewModel viewModel);
    Task<bool> DeleteEquipamentoAsync(int id);
}

