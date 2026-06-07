FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/MetricsHub.Domain/MetricsHub.Domain.csproj", "src/MetricsHub.Domain/"]
COPY ["src/MetricsHub.Infrastructure/MetricsHub.Infrastructure.csproj", "src/MetricsHub.Infrastructure/"]
COPY ["src/MetricsHub.Application/MetricsHub.Application.csproj", "src/MetricsHub.Application/"]
COPY ["src/MetricsHub.Api/MetricsHub.Api.csproj", "src/MetricsHub.Api/"]
RUN dotnet restore "src/MetricsHub.Api/MetricsHub.Api.csproj"

COPY . .
RUN dotnet build "src/MetricsHub.Api/MetricsHub.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/MetricsHub.Api/MetricsHub.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetricsHub.Api.dll"]
