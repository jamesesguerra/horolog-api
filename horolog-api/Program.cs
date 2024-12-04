using horolog_api.Data;
using horolog_api.Features.Brands;
using horolog_api.Features.WatchModels;
using horolog_api.Features.WatchRecords;

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

app.UseCors("AllowSpecificOrigins");
app.Run();
