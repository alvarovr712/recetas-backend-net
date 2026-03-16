# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar lo publicado
COPY --from=build /app/publish .

# Exponer el puerto que usará la API
EXPOSE 8080

# Forzar a .NET a escuchar en 0.0.0.0:8080 (necesario en Docker)
ENV ASPNETCORE_URLS=http://+:8080

# Ejecutar la API
ENTRYPOINT ["dotnet", "recetasAPINet.dll"]
