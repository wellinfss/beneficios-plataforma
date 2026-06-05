FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/BeneficiosPlataforma.API/BeneficiosPlataforma.API.csproj", "src/BeneficiosPlataforma.API/"]
COPY ["src/BeneficiosPlataforma.Application/BeneficiosPlataforma.Application.csproj", "src/BeneficiosPlataforma.Application/"]
COPY ["src/BeneficiosPlataforma.Domain/BeneficiosPlataforma.Domain.csproj", "src/BeneficiosPlataforma.Domain/"]
COPY ["src/BeneficiosPlataforma.Infrastructure/BeneficiosPlataforma.Infrastructure.csproj", "src/BeneficiosPlataforma.Infrastructure/"]

RUN dotnet restore "src/BeneficiosPlataforma.API/BeneficiosPlataforma.API.csproj"

COPY . .
WORKDIR "/src/src/BeneficiosPlataforma.API"

RUN dotnet build "BeneficiosPlataforma.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeneficiosPlataforma.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "BeneficiosPlataforma.API.dll"]
