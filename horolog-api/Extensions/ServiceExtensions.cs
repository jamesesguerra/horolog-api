using horolog_api.Data;
using horolog_api.Features.Brands;
using horolog_api.Features.Tokens;
using horolog_api.Features.WatchModels;
using horolog_api.Features.WatchRecords;
using horolog_api.Features.Users;
using horolog_api.Features.WatchImages;
using horolog_api.Features.WatchReports;

namespace horolog_api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton<IBrandsRepository, BrandsRepository>();
        services.AddSingleton<IBrandsService, BrandsService>();
        services.AddSingleton<IWatchModelsRepository, WatchModelsRepository>();
        services.AddSingleton<IWatchModelsService, WatchModelsService>();
        services.AddSingleton<IWatchRecordsRepository, WatchRecordsRepository>();
        services.AddSingleton<IWatchRecordsService, WatchRecordsService>();
        services.AddSingleton<IUsersRepository, UsersRepository>();
        services.AddSingleton<IUsersService, UsersService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IWatchReportsRepository, WatchReportsRepository>();
        services.AddSingleton<IWatchReportsService, WatchReportsService>();
        services.AddSingleton<IWatchImagesRepository, WatchImagesRepository>();
        services.AddSingleton<IWatchImagesService, WatchImagesService>();
        
        return services;
    }
}