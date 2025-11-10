# File: Ogur.Hub.Api/Dockerfile
# Project: Ogur.Hub.Api

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Install Docker CLI for container monitoring
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    gnupg \
    lsb-release && \
    install -m 0755 -d /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg && \
    chmod a+r /etc/apt/keyrings/docker.gpg && \
    echo \
    "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
    $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
    tee /etc/apt/sources.list.d/docker.list > /dev/null && \
    apt-get update && \
    apt-get install -y --no-install-recommends docker-ce-cli && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG VERSION=1.0.0-dev
WORKDIR /src

# Copy project files
COPY ["Ogur.Hub.Api/Ogur.Hub.Api.csproj", "Ogur.Hub.Api/"]
COPY ["Ogur.Hub.Application/Ogur.Hub.Application.csproj", "Ogur.Hub.Application/"]
COPY ["Ogur.Hub.Domain/Ogur.Hub.Domain.csproj", "Ogur.Hub.Domain/"]
COPY ["Ogur.Hub.Infrastructure/Ogur.Hub.Infrastructure.csproj", "Ogur.Hub.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Ogur.Hub.Api/Ogur.Hub.Api.csproj"

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish "Ogur.Hub.Api/Ogur.Hub.Api.csproj" \
    -c Release \
    -o /app/publish \
    -p:Version=${VERSION} \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV APP_VERSION=${VERSION}
LABEL version="${VERSION}"

ENTRYPOINT ["dotnet", "Ogur.Hub.Api.dll"]