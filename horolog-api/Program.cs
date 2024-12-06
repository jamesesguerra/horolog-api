using System.Text;
using horolog_api.Data;
using horolog_api.Features.Brands;
using horolog_api.Features.Tokens;
using horolog_api.Features.WatchModels;
using horolog_api.Features.WatchRecords;
using horolog_api.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbContext, DbContext>();
builder.Services.AddSingleton<IBrandsRepository, BrandsRepository>();
builder.Services.AddSingleton<IBrandsService, BrandsService>();
builder.Services.AddSingleton<IWatchModelsRepository, WatchModelsRepository>();
builder.Services.AddSingleton<IWatchModelsService, WatchModelsService>();
builder.Services.AddSingleton<IWatchRecordsRepository, WatchRecordsRepository>();
builder.Services.AddSingleton<IWatchRecordsService, WatchRecordsService>();
builder.Services.AddSingleton<IUsersRepository, UsersRepository>();
builder.Services.AddSingleton<IUsersService, UsersService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Cannot access token key");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:4200" }
    : new[] { "" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", 
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapBrands();
app.MapWatchModels();
app.MapWatchRecords();
app.MapUsers();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();
app.Run();
