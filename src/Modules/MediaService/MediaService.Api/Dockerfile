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
COPY ["src/Modules/MediaService/MediaService.Api/MediaService.Api.csproj", "src/Modules/MediaService/MediaService.Api/"]
COPY ["src/Shared/Shared.Api/Shared.Api.csproj", "src/Shared/Shared.Api/"]
COPY ["src/Shared/Shared.Application/Shared.Application.csproj", "src/Shared/Shared.Application/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/Shared/Shared.Core/Shared.Core.csproj", "src/Shared/Shared.Core/"]
COPY ["src/Shared/Shared.AuthZ/Shared.AuthZ.csproj", "src/Shared/Shared.AuthZ/"]
RUN dotnet restore "./src/Modules/MediaService/MediaService.Api/MediaService.Api.csproj"
COPY . .
WORKDIR "/src/src/Modules/MediaService/MediaService.Api"
RUN dotnet build "./MediaService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediaService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.Api.dll"]