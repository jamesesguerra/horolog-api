using System.Data.Common;
using System.Text;
using horolog_api.Data;
using horolog_api.Extensions;
using horolog_api.Features.Brands;
using horolog_api.Features.Files;
using horolog_api.Features.WatchModels;
using horolog_api.Features.WatchRecords;
using horolog_api.Features.Users;
using horolog_api.Features.WatchImages;
using horolog_api.Features.WatchReports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Minio;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5228");

// logging
builder.Logging
    .ClearProviders()
    .AddSimpleConsole()
    .AddDebug()
    .AddApplicationInsights(
        telemetry => telemetry.ConnectionString
            = builder.Configuration["Azure:ApplicationInsights:ConnectionString"],
        _ => { });

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
DbInitializer.Initialize(connectionString);

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

    services.AddSingleton<IMinioClient>(sp =>
    {
        return new MinioClient()
            .WithEndpoint("minio", 9000)
            .WithCredentials(configuration["Minio:Username"], configuration["Minio:Password"])
            .WithSSL(false)
            .Build();
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
        options.AddPolicy("AllowAll", 
            policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
    
    // caching
    services.AddResponseCaching();
    services.AddMemoryCache();
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
        
        // HTTP security headers
        app.UseHsts();
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Frame-Options", "sameorigin");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Content-Security-Policy", "default-src ' self' ;");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin");
            await next();
        });
    }
    
    app.UseCors("AllowAll");
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
    app.Map("/error", (HttpContext context) =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
        var details = new ProblemDetails();
    
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
    
        if (exceptionHandler?.Error is TimeoutException)
        {
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.5";
            details.Status = StatusCodes.Status504GatewayTimeout;
        }
        else if (exceptionHandler?.Error is DbConnection)
        {
            app.Logger.LogError(context.Request.Body.ToString());
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            details.Status = StatusCodes.Status500InternalServerError;
        }
        else
        {
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            details.Status = StatusCodes.Status500InternalServerError;
        }
    
        app.Logger.LogError(
            exceptionHandler?.Error,
            "An unhandled error occured");
        
        return Results.Problem(details);
    });
}

