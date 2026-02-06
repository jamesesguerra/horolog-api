namespace horolog_api.Features.Search;

public interface ISearchService
{
    Task<T?> SearchAsync<T>(string selectBy, string searchBy, string searchQuery);
}