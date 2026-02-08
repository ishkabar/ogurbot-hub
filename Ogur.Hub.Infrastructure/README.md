# Ogur.Hub.Infrastructure

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![MariaDB](https://img.shields.io/badge/MariaDB-10.6+-003545?style=flat-square&logo=mariadb)

Infrastructure layer implementing EF Core persistence, SignalR hubs, and background services.

## Structure

### Persistence
- `ApplicationDbContext` - EF Core DbContext with entity configurations
- `Repository<T>` - Generic repository implementation
- `UnitOfWork` - Transaction coordination
- **Configurations**: EF Core fluent API for all entities
- **Converters**: Value object converters (`LicenseKey`, `ApiKey`, `DeviceFingerprint`)

### SignalR Hubs
- `DevicesHub` - Real-time device communication (`/hubs/devices`)
- `VpsHub` - Real-time VPS monitoring (`/hubs/vps`)
- `IDevicesHubClient` - Typed hub client interface

### Background Services
- `CommandProcessingService` - Processes pending hub commands
- `VpsMonitoringBackgroundService` - Real-time VPS stats broadcasting

### Services
- `CommandDispatcher` - MediatR command routing
- `DockerMonitorService` - Docker container stats via Docker API
- `SystemMonitorService` - CPU, RAM, disk monitoring
- `FakeVpsDataGenerator` - Mock data for development

### Repositories
- `VpsRepository` - VPS entities persistence

## Dependencies
- `Ogur.Hub.Domain`, `Ogur.Hub.Application`
- `Pomelo.EntityFrameworkCore.MySql` - MariaDB provider
- `Microsoft.AspNetCore.SignalR` - Real-time communication
- `Docker.DotNet` - Docker API client
