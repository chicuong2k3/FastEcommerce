# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Modules/InventoryService/InventoryService.Api/InventoryService.Api.csproj", "src/Modules/InventoryService/InventoryService.Api/"]
COPY ["src/Shared/Shared.Api/Shared.Api.csproj", "src/Shared/Shared.Api/"]
COPY ["src/Shared/Shared.Application/Shared.Application.csproj", "src/Shared/Shared.Application/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/Shared/Shared.Core/Shared.Core.csproj", "src/Shared/Shared.Core/"]
COPY ["src/Shared/Shared.AuthZ/Shared.AuthZ.csproj", "src/Shared/Shared.AuthZ/"]
COPY ["src/Modules/InventoryService/InventoryService.Infrastructure/InventoryService.Infrastructure.csproj", "src/Modules/InventoryService/InventoryService.Infrastructure/"]
COPY ["src/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "src/Shared/Shared.Infrastructure/"]
COPY ["src/Modules/InventoryService/InventoryService.Application/InventoryService.Application.csproj", "src/Modules/InventoryService/InventoryService.Application/"]
COPY ["src/Modules/Catalog/Catalog.Contracts/Catalog.Contracts.csproj", "src/Modules/Catalog/Catalog.Contracts/"]
COPY ["src/Modules/Catalog/Catalog.ReadModels/Catalog.ReadModels.csproj", "src/Modules/Catalog/Catalog.ReadModels/"]
COPY ["src/Modules/InventoryService/InventoryService.Contracts/InventoryService.Contracts.csproj", "src/Modules/InventoryService/InventoryService.Contracts/"]
COPY ["src/Modules/InventoryService/InventoryService.ReadModels/InventoryService.ReadModels.csproj", "src/Modules/InventoryService/InventoryService.ReadModels/"]
COPY ["src/Modules/InventoryService/InventoryService.Core/InventoryService.Core.csproj", "src/Modules/InventoryService/InventoryService.Core/"]
COPY ["src/Modules/InventoryService/InventoryService.Requests/InventoryService.Requests.csproj", "src/Modules/InventoryService/InventoryService.Requests/"]
RUN dotnet restore "./src/Modules/InventoryService/InventoryService.Api/InventoryService.Api.csproj"
COPY . .
WORKDIR "/src/src/Modules/InventoryService/InventoryService.Api"
RUN dotnet build "./InventoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InventoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.Api.dll"]