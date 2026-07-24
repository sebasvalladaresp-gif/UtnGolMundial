# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copiar el archivo de solución y los proyectos
COPY ["UtnGolMundial.sln", "./"]
COPY ["UtnGolMundial.Web/UtnGolMundial.Web.csproj", "UtnGolMundial.Web/"]
# Restaurar dependencias
RUN dotnet restore "UtnGolMundial.Web/UtnGolMundial.Web.csproj"
# Copiar el resto del código fuente
COPY . .
WORKDIR "/src/UtnGolMundial.Web"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
# Etapa final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UtnGolMundial.Web.dll"]
