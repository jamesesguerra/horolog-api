namespace horolog_api.Features.Brands;

public interface IBrandsRepository
{
    Task<IEnumerable<Brand>> GetBrands();
    Task<Brand> GetBrandById(int id);
    Task<Brand> AddBrand(Brand brand);
}