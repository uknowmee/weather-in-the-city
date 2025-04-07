FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

COPY src/WeatherInTheCity.Client/WeatherInTheCity.Client.csproj WeatherInTheCity.Client/
RUN dotnet restore WeatherInTheCity.Client/WeatherInTheCity.Client.csproj

COPY src/WeatherInTheCity/WeatherInTheCity.csproj WeatherInTheCity/
RUN dotnet restore WeatherInTheCity/WeatherInTheCity.csproj

COPY src/. .
RUN dotnet publish WeatherInTheCity/WeatherInTheCity.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeatherInTheCity.dll"]
