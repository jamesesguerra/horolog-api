namespace horolog_api.Features.Brands;

public static class BrandsEndpoints
{
    public static IEndpointRouteBuilder MapBrands(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/brands")
            .WithTags("Brands")
            .WithOpenApi();

        group.MapGet("/", (IBrandsService service) => service.GetBrands());

        group.MapGet("/{id}", (IBrandsService service, int id) => service.GetBrandById(id));

        group.MapPost("/", async (IBrandsService service, Brand brand) => await service.AddBrand(brand));
        
        return endpoints;
    }
}