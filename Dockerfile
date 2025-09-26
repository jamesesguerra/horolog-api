FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5228

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["horolog-api/horolog-api.csproj", "horolog-api/"]
RUN dotnet restore "horolog-api/horolog-api.csproj"
COPY . .
WORKDIR /src/horolog-api
RUN dotnet build "horolog-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/horolog-api
RUN dotnet publish "horolog-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN mkdir ./AppData
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "horolog-api.dll"]
