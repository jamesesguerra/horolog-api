namespace horolog_api.Features.Search;

public static class SearchEndpoints
{
    public static void MapSearch(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/search")
            .WithTags("Search")
            .WithOpenApi();

        group.MapGet("/", async (
            string selectBy,
            string searchBy,
            string query,
            ISearchRepository repository
        ) =>
        {
            if (string.IsNullOrWhiteSpace(selectBy) ||
                string.IsNullOrWhiteSpace(searchBy) ||
                string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest("Invalid search parameters.");
            }

            var result = await repository.SearchAsync<string>(
                selectBy,
                searchBy,
                query
            );

            return Results.Ok(result);
        });
    }
}