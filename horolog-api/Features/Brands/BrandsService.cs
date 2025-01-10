namespace horolog_api.Features.Brands;

public class BrandsService(IBrandsRepository repository) : IBrandsService
{
    public async Task<IEnumerable<Brand>> GetBrands()
    {
        return await repository.GetBrands();
    }

    public async Task<Brand> GetBrandById(int id)
    {
        return await repository.GetBrandById(id);
    }
}