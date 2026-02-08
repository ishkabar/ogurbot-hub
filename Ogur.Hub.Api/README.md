# Ogur.Hub.Api

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger)

REST API exposing Hub functionality with Swagger documentation and SignalR endpoints.

## Structure

### Controllers
- `AuthController` - JWT authentication (`/api/auth`)
- `ApplicationsController` - Application management (`/api/applications`)
- `LicensesController` - License operations (`/api/licenses`)
- `DevicesController` - Device control (`/api/devices`)
- `UsersController` - User management (`/api/users`)
- `TelemetryController` - Telemetry ingestion (`/api/telemetry`)
- `UpdatesController` - Version checking (`/api/updates`)
- `AuditController` - Audit log queries (`/api/audit`)
- `DashboardController` - Statistics (`/api/dashboard`)
- `VpsController` - VPS monitoring (`/api/vps`)

### Middleware
- `ApiKeyAuthenticationMiddleware` - API key validation for applications
- `ExceptionHandlingMiddleware` - Global error handling
- `RequestLoggingMiddleware` - Structured request logging

### Services
- `TokenService` - JWT generation and validation

### Models
**Requests**: All DTOs for POST/PUT endpoints
**Responses**: Standard API responses (`ApiResponse<T>`, `ErrorResponse`, `LoginResponse`, `LicenseValidationResponse`)

## Endpoints
See main README for full API documentation.

## Dependencies
- `Ogur.Hub.Application`, `Ogur.Hub.Infrastructure`
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `Serilog.AspNetCore` - Structured logging
- `BCrypt.Net-Next` - Password hashing
