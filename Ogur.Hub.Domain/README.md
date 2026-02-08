# Ogur.Hub.Domain

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)

Domain layer containing entities, value objects, and business rules using Domain-Driven Design patterns.

## Structure

### Entities
- `Application` - Registered Ogur applications
- `ApplicationVersion` - Version history per application
- `User` - Web panel user accounts
- `License` - License management (1:1 with users)
- `Device` - Device registrations (HWID + GUID)
- `DeviceSession` - SignalR connection tracking
- `DeviceUser` - Many-to-many device-user assignments
- `HubCommand` - Command history sent to devices
- `Telemetry` - Event logs from applications
- `AuditLog` - Complete audit trail
- `VpsContainer` - Docker container monitoring
- `VpsResourceSnapshot` - Historical resource usage
- `VpsWebsite` - Managed websites

### Value Objects
- `LicenseKey` - Immutable license key representation
- `ApiKey` - SHA256-hashed API key for applications
- `DeviceFingerprint` - HWID + GUID composite

### Enums
- `LicenseStatus` - Active, Expired, Revoked, Suspended
- `DeviceStatus` - Active, Blocked, Inactive
- `CommandType` - Logout, BlockDevice, Notify, ForceUpdate, RefreshLicense, Custom
- `CommandStatus` - Pending, Acknowledged, Failed
- `UserRole` - Admin, User

### Base Classes
- `Entity` - Base entity with `Id` and timestamps
- `AggregateRoot` - Root entity for aggregate boundaries
- `ValueObject` - Immutable value object base
- `IDomainEvent` - Domain event marker interface

## Dependencies
None - Pure domain layer with zero external dependencies.
