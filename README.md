# Ogur.Hub - Centralized Application Management Platform

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-Proprietary-red)
![Status](https://img.shields.io/badge/status-active-success)

**Ogur.Hub** is a centralized management hub for monitoring, controlling, and licensing all Ogur applications. It provides a unified web panel with REST API and SignalR for real-time communication with client applications.

---

## Features

### Core Capabilities
- **Application Management**: Register and track all Ogur applications with version control
- **License Management**: 1 license = 1 account = N devices (configurable), HWID + GUID tracking
- **Device Management**: Track, block, and remotely control connected devices
- **Real-time Communication**: SignalR hub for pushing commands to applications
- **User Management**: JWT-based authentication with role-based access control
- **Audit Logging**: Complete audit trail of all operations
- **Telemetry**: Receive and analyze application telemetry data

### Security
- **JWT Authentication**: For web panel users (24h token expiration)
- **API Key Authentication**: For applications (SHA256 hashed)
- **Role-Based Access Control**: Admin vs. User permissions
- **Device Fingerprinting**: HWID + GUID for unique device identification

### Monitoring & Control
- **Health Checks**: Application version tracking and update notifications
- **Remote Commands**: Logout, block device, send notifications, force updates
- **Session Tracking**: SignalR connection monitoring with IP address logging
- **Usage Statistics**: Device sessions, validation counts, telemetry events

---

## Tech Stack

### Backend
- **.NET 8** - C# 12
- **ASP.NET Core Web API** - REST endpoints
- **SignalR** - Real-time bidirectional communication
- **Entity Framework Core 8** - ORM with Code-First migrations
- **MariaDB** (MySQL) - Primary database via Pomelo.EntityFrameworkCore.MySql
- **Serilog** - Structured logging
- **MediatR** - CQRS pattern implementation

### Frontend (Web Panel)
- **ASP.NET Core MVC** - Server-side rendering
- **DevExpress** - UI components
- **Bootstrap 5.3** - Responsive framework
- **SignalR Client** - Real-time updates

### Architecture
- **Clean Architecture** - Domain, Application, Infrastructure, API layers
- **CQRS** - Command Query Responsibility Segregation
- **DDD** - Domain-Driven Design patterns
- **Repository Pattern** - Generic IRepository<T> with EF Core
- **Value Objects** - LicenseKey, ApiKey, DeviceFingerprint

### Client Libraries
- **Ogur.Core** - Client-side integration library
- **Ogur.Abstractions** - Shared interfaces and contracts

---

## Project Structure
```
/src
  Ogur.Hub.Domain/              # Domain entities, value objects, enums
  Ogur.Hub.Application/          # Business logic, CQRS handlers, DTOs
  Ogur.Hub.Infrastructure/       # EF Core, repositories, SignalR hubs
  Ogur.Hub.Api/                  # REST API + SignalR endpoints
  Ogur.Hub.Web/                  # DevExpress web panel (MVC)
  Ogur.Hub.Tests/                # Unit & integration tests

/client-libraries
  Ogur.Core/                     # Client integration library
  Ogur.Abstractions/             # Shared abstractions
```

---

## Database Schema

### Core Tables
- **Applications** - Registry of all Ogur applications
- **Users** - Web panel user accounts
- **Licenses** - License management (1:1 with users, 1:N with devices)
- **Devices** - Device registrations with HWID + GUID
- **DeviceSessions** - SignalR connection tracking
- **ApplicationVersions** - Version history per application
- **Telemetry** - Event logs from applications
- **AuditLogs** - Complete audit trail
- **HubCommands** - Command history sent to devices

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- MariaDB 10.6+ (or MySQL 8.0+)
- (Optional) Docker & Docker Compose

### Installation

1. **Clone repository**
```bash
git clone https://github.com/yourusername/Ogur.Hub.git
cd Ogur.Hub
```

2. **Configure database connection**

Edit `Ogur.Hub.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ogurhub;User=root;Password=yourpassword;"
  }
}
```

3. **Run migrations**
```bash
cd src/Ogur.Hub.Api
dotnet ef database update -p ../Ogur.Hub.Infrastructure
```

4. **Create admin user**
```sql
-- Generate BCrypt hash for your password first
INSERT INTO Users (Username, Email, PasswordHash, IsAdmin, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@ogurhub.local', 'YOUR_BCRYPT_HASH', 1, 1, NOW(), NOW());
```

5. **Run API**
```bash
dotnet run --project src/Ogur.Hub.Api
```

6. **Run Web Panel** (in separate terminal)
```bash
dotnet run --project src/Ogur.Hub.Web
```

7. **Access**
- API: `http://localhost:5180/swagger`
- Web Panel: `http://localhost:5001`

---

## API Endpoints

### Authentication
- `POST /api/auth/login` - JWT login for web panel users

### Applications
- `GET /api/applications` - List all applications
- `POST /api/applications` - Register new application
- `GET /api/applications/{id}` - Get application details

### Licenses
- `POST /api/licenses/validate` - Validate license + register/update device
- `GET /api/licenses` - List licenses
- `POST /api/licenses` - Create new license
- `PATCH /api/licenses/{id}` - Update license

### Devices
- `GET /api/devices` - List devices
- `POST /api/devices/{id}/block` - Block device
- `POST /api/devices/{id}/logout` - Force logout device

### Updates
- `GET /api/updates/check` - Check for application updates

### Telemetry
- `POST /api/telemetry` - Receive telemetry batch

### Audit
- `GET /api/audit` - Query audit logs

---

## SignalR Hub

**Endpoint:** `/hubs/devices`

### Client → Server
- `Heartbeat()` - Periodic ping from client

### Server → Client
- `ReceiveCommand(HubCommand)` - Server pushes command
- Commands: `Logout`, `BlockDevice`, `Notify`, `ForceUpdate`, `RefreshLicense`, `Custom`

### Client → Server (Acknowledgment)
- `AcknowledgeCommand(commandId)` - Client confirms execution

---

## Docker Deployment
```bash
# Build and run
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

---

## Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## Environment Variables

### API (Ogur.Hub.Api)
- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ConnectionStrings__DefaultConnection` - Database connection string
- `Jwt__SecretKey` - JWT signing key
- `Jwt__Issuer` - JWT issuer
- `Jwt__Audience` - JWT audience

### Web (Ogur.Hub.Web)
- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `HubApi__BaseUrl` - API base URL (http://localhost:5180 or production URL)

---

## License

Proprietary - All rights reserved © 2025 Dominik (ogur.dev)

---

## Author

**Dominik**
- Organization: ogur.dev

---

## Contributing

This is a private/proprietary project. Contributions are restricted to authorized team members only.

---

## Support

For access requests or support, contact: `admin@ogurhub.local`

---

## Roadmap

- [ ] Redis caching layer
- [ ] Rate limiting per API key
- [ ] Advanced telemetry analytics
- [ ] Multi-tenancy support
- [ ] Two-factor authentication (2FA)
- [ ] Webhook notifications
- [ ] GraphQL API
- [ ] Mobile app for monitoring

---

**Built with ❤️ using .NET 8 and Clean Architecture**