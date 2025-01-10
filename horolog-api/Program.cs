using System.Text;
using horolog_api.Extensions;
using horolog_api.Features.Brands;
using horolog_api.Features.Files;
using horolog_api.Features.WatchModels;
using horolog_api.Features.WatchRecords;
using horolog_api.Features.Users;
using horolog_api.Features.WatchImages;
using horolog_api.Features.WatchReports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddProblemDetails();
    
    // azure
    services.AddAzureClients(azureBuilder =>
    {
        azureBuilder.AddBlobServiceClient(configuration.GetConnectionString("BlobStorage"));
    });
    
    // application services
    services.AddApplicationServices();
    
    // authentication and authorization
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot access token key");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    services.AddAuthorization();
    
    // CORS
    services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", 
            policy => policy
                .WithOrigins(configuration["AllowedOrigins"]!)
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
    
    // caching
    services.AddResponseCaching();
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/error");
    }
    
    app.UseHttpsRedirection();
    app.UseCors("AllowSpecificOrigins");
    app.UseResponseCaching();
    app.UseAuthentication();
    app.UseAuthorization();
}

static void ConfigureEndpoints(WebApplication app)
{
    // features
    app.MapBrands();
    app.MapWatchModels();
    app.MapWatchRecords();
    app.MapWatchReports();
    app.MapUsers();
    app.MapFiles();
    app.MapWatchImages();
    
    // error handling endpoint
    app.MapGet("/error", () => Results.Problem());
}
