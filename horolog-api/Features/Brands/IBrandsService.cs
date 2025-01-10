namespace horolog_api.Features.Brands;

public interface IBrandsService
{
    Task<IEnumerable<Brand>> GetBrands();
    Task<Brand> GetBrandById(int id);
}