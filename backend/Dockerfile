# Stage 1: build + publish
# Nota: la solucion usa .slnx (requiere .NET SDK 9+), por lo que aqui se construye
# directamente el proyecto de la API con .NET SDK 8.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/src/ ./src/
RUN dotnet restore src/Api/Api.csproj
RUN dotnet publish src/Api/Api.csproj -c Release --no-restore -o /publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /publish .
# Directorio para el sink File de Serilog (logs/consultora-.log)
RUN mkdir -p /app/logs
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]