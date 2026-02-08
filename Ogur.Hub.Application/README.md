# Ogur.Hub.Application

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)

Application layer implementing business logic using CQRS pattern with MediatR.

## Structure

### Commands
- **ApplicationsCommands**: `CreateApplication`, `UpdateApplication`
- **LicensesCommands**: `CreateLicense`, `ValidateLicense`, `ExtendLicense`, `RevokeLicense`, `ActivateLicense`, `UpdateLicense`, `DeleteLicense`
- **DevicesCommands**: `BlockDevice`, `UnblockDevice`, `SendDeviceCommand`, `AssignUserToDevice`, `RemoveUserFromDevice`, `UpdateDevice`
- **UsersCommands**: `CreateUser`, `RegisterUser`, `UpdateUser`, `DeleteUser`
- **TelemetryCommands**: `ReceiveTelemetry`

### Queries
- **Applications**: `GetApplicationsQuery`
- **Licenses**: `GetLicensesQuery`
- **Devices**: `GetDevicesQuery`
- **Users**: `GetUsersQuery`, `GetUserByIdQuery`
- **Telemetry**: `GetTelemetryQuery`
- **Dashboard**: `GetDashboardStatsQuery`
- **Updates**: `CheckForUpdatesQuery`

### DTOs
- `ApplicationDto`, `LicenseDto`, `DeviceDto`, `UserDto`, `TelemetryDto`
- `DashboardStatsDto`, `VpsContainerDto`, `VpsResourceDto`, `VpsWebsiteDto`

### Interfaces
- `IApplicationDbContext` - EF Core DbContext contract
- `IRepository<T>` - Generic repository pattern
- `IUnitOfWork` - Transaction management
- `ICommandDispatcher` - Command routing
- `IVpsMonitorService`, `IDockerMonitorService`, `ISystemMonitorService` - VPS monitoring

### Services
- `VpsMonitorService` - VPS resource monitoring orchestration

## Dependencies
- `Ogur.Hub.Domain`
- `MediatR` - CQRS pattern
- `Microsoft.EntityFrameworkCore` - EF Core abstractions
