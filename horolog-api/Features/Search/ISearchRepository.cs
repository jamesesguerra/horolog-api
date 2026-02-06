namespace horolog_api.Features.Search;

public interface ISearchRepository
{
    Task<T?> SearchAsync<T>(string selectBy, string searchBy, string searchQuery);
}