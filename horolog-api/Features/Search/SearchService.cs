namespace horolog_api.Features.Search;

public class SearchService(ISearchRepository searchRepository) : ISearchService
{
    public async Task<T?> SearchAsync<T>(string selectBy, string searchBy, string searchQuery)
    {
        return await searchRepository.SearchAsync<T>(selectBy, searchBy, searchQuery);
    }
}