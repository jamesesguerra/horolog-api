using horolog_api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace horolog_api.Features.Brands;

public static class BrandsEndpoints
{
    public static void MapBrands(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/brands")
            .WithTags("Brands")
            .WithOpenApi()
            .AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/", async (IBrandsService service) => await service.GetBrands());

        group.MapGet("/{id}", async (IBrandsService service, int id) =>  await service.GetBrandById(id));
    }
}