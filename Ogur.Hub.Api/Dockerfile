# File: Dockerfile
# Project: Root

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Ogur.Hub.Api/Ogur.Hub.Api.csproj", "src/Ogur.Hub.Api/"]
COPY ["src/Ogur.Hub.Infrastructure/Ogur.Hub.Infrastructure.csproj", "src/Ogur.Hub.Infrastructure/"]
COPY ["src/Ogur.Hub.Application/Ogur.Hub.Application.csproj", "src/Ogur.Hub.Application/"]
COPY ["src/Ogur.Hub.Domain/Ogur.Hub.Domain.csproj", "src/Ogur.Hub.Domain/"]

RUN dotnet restore "src/Ogur.Hub.Api/Ogur.Hub.Api.csproj"

COPY . .
WORKDIR "/src/src/Ogur.Hub.Api"
RUN dotnet build "Ogur.Hub.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ogur.Hub.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ogur.Hub.Api.dll"]