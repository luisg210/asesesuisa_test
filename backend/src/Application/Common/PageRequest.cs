namespace Consultora.Application.Common;

/// <summary>
/// Parametros de paginacion + ordenamiento.
/// </summary>
public record PageRequest(int? Page = 1, int? PageSize = 10, string? SortBy = null, string? SortDir = null)
{
    public int SafePage => Page is null or < 1 ? 1 : Page.Value;
    public int SafePageSize => Math.Clamp(PageSize ?? 10, 1, 100);
    public string SafeSortBy => string.IsNullOrWhiteSpace(SortBy) ? "Id" : SortBy.Trim();
    public string SafeSortDir => SortDir?.Trim().ToLowerInvariant() == "desc" ? "desc" : "asc";
}