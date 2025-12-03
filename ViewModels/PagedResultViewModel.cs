namespace EFEnergiaAPI.ViewModels;

public class PagedResultViewModel<T>
{
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = new();
}

