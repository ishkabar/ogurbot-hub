# File: Ogur.Hub.Web/Dockerfile
# Project: Ogur.Hub.Web

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG VERSION=1.0.0-dev
ARG DEVEXPRESS_KEY
WORKDIR /src

# Setup NuGet config for DevExpress
RUN mkdir -p /root/.nuget/NuGet && \
    echo "<?xml version=\"1.0\" encoding=\"utf-8\"?> \
    <configuration> \
      <packageSources> \
        <clear /> \
        <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" /> \
        <add key=\"DevExpress\" value=\"https://nuget.devexpress.com/${DEVEXPRESS_KEY}/api\" /> \
      </packageSources> \
    </configuration>" > /root/.nuget/NuGet/NuGet.Config

# Copy project file
COPY ["Ogur.Hub.Web/Ogur.Hub.Web.csproj", "Ogur.Hub.Web/"]

# Restore dependencies
RUN dotnet restore "Ogur.Hub.Web/Ogur.Hub.Web.csproj"

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish "Ogur.Hub.Web/Ogur.Hub.Web.csproj" \
    -c Release \
    -o /app/publish \
    -p:Version=${VERSION} \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV APP_VERSION=${VERSION}
LABEL version="${VERSION}"

ENTRYPOINT ["dotnet", "Ogur.Hub.Web.dll"]