# Ogur.Hub

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![MariaDB](https://img.shields.io/badge/MariaDB-10.6+-003545?style=flat-square&logo=mariadb)
![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4?style=flat-square)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat-square&logo=docker)
![License](https://img.shields.io/badge/license-Proprietary-red?style=flat-square)

Centralized management hub for monitoring, controlling, and licensing all Ogur applications with REST API and real-time SignalR communication.

## Features

### Core Capabilities
- **Application Management**: Register and track Ogur applications with version control
- **License Management**: 1 license = 1 account = N devices (configurable), HWID + GUID tracking
- **Device Management**: Track, block, and remotely control connected devices
- **Real-time Communication**: SignalR hub for pushing commands to applications
- **User Management**: JWT-based authentication with role-based access control (Admin/User)
- **Audit Logging**: Complete audit trail of all operations
- **Telemetry**: Receive and analyze application event data
- **VPS Monitoring**: Docker container stats, resource usage, website management

### Security
- **JWT Authentication**: Web panel users (24h token expiration)
- **API Key Authentication**: Applications (SHA256 hashed)
- **Role-Based Access Control**: Admin vs. User permissions
- **Device Fingerprinting**: HWID + GUID for unique device identification

### Monitoring & Control
- **Health Checks**: Application version tracking and update notifications
- **Remote Commands**: Logout, block device, send notifications, force updates
- **Session Tracking**: SignalR connection monitoring with IP address logging
- **Usage Statistics**: Device sessions, validation counts, telemetry events
- **VPS Dashboard**: Real-time CPU, RAM, disk usage, container management

## Tech Stack

### Backend
- **.NET 8** with C# 12
- **ASP.NET Core Web API** - REST endpoints
- **SignalR** - Real-time bidirectional communication
- **Entity Framework Core 8** - Code-First ORM
- **MariaDB** (MySQL) via Pomelo.EntityFrameworkCore.MySql
- **Serilog** - Structured logging
- **MediatR** - CQRS pattern

### Frontend (Web Panel)
- **ASP.NET Core MVC** - Server-side rendering
- **DevExpress** - Professional UI components
- **Bootstrap 5.3** - Responsive framework
- **SignalR Client** - Real-time dashboard updates
- **Chart.js** - VPS monitoring visualizations

### Architecture
- **Clean Architecture** - Domain, Application, Infrastructure, API layers
- **CQRS** - Command Query Responsibility Segregation
- **DDD** - Domain-Driven Design patterns
- **Repository Pattern** - Generic `IRepository<T>` with EF Core
- **Value Objects** - `LicenseKey`, `ApiKey`, `DeviceFingerprint`

### Client Libraries
- **Ogur.Core** - Client-side integration library
- **Ogur.Abstractions** - Shared interfaces and contracts

## Solution Structure
```
Ogur.Hub.sln
├── Ogur.Hub.Domain/           # Domain entities, value objects, enums
├── Ogur.Hub.Application/      # Business logic, CQRS handlers, DTOs
├── Ogur.Hub.Infrastructure/   # EF Core, repositories, SignalR hubs
├── Ogur.Hub.Api/              # REST API + SignalR endpoints
├── Ogur.Hub.Web/              # DevExpress web panel (MVC)
└── Ogur.Hub.Tests/            # Unit & integration tests
```

## Database Schema

### Core Tables
| Table | Purpose |
|-------|---------|
| `Applications` | Registry of all Ogur applications |
| `Users` | Web panel user accounts |
| `Licenses` | License management (1:1 with users, 1:N with devices) |
| `Devices` | Device registrations with HWID + GUID |
| `DeviceSessions` | SignalR connection tracking |
| `DeviceUsers` | Many-to-many device-user assignments |
| `ApplicationVersions` | Version history per application |
| `Telemetry` | Event logs from applications |
| `AuditLogs` | Complete audit trail |
| `HubCommands` | Command history sent to devices |
| `VpsContainers` | Docker container monitoring |
| `VpsResourceSnapshots` | Historical resource usage |
| `VpsWebsites` | Managed websites (Traefik integration) |

## Getting Started

### Prerequisites
- .NET 8 SDK
- MariaDB 10.6+ (or MySQL 8.0+)
- Docker & Docker Compose (optional)

### Installation

1. **Clone repository**
```bash
git clone https://github.com/ishkabar/ogurbot-hub.git
cd ogurbot-hub
```

2. **Configure database**

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
cd Ogur.Hub.Api
dotnet ef database update -p ../Ogur.Hub.Infrastructure
```

4. **Create admin user**
```sql
-- Generate BCrypt hash for your password first
INSERT INTO Users (Username, Email, PasswordHash, IsAdmin, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@ogurhub.local', 'YOUR_BCRYPT_HASH', 1, 1, NOW(), NOW());
```

5. **Run services**
```bash
# API
dotnet run --project Ogur.Hub.Api

# Web Panel (separate terminal)
dotnet run --project Ogur.Hub.Web
```

6. **Access**
- API: `http://localhost:5180/swagger`
- Web Panel: `http://localhost:5001`

## Docker Deployment
```bash
# Build and run
docker-compose up -d

# View logs
docker-compose logs -f api web

# Stop services
docker-compose down
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - JWT login for web panel users
- `POST /api/auth/register` - Register new user

### Applications
- `GET /api/applications` - List all applications
- `POST /api/applications` - Register new application
- `GET /api/applications/{id}` - Get application details
- `PUT /api/applications/{id}` - Update application

### Licenses
- `POST /api/licenses/validate` - Validate license + register/update device
- `GET /api/licenses` - List licenses
- `POST /api/licenses` - Create new license
- `PATCH /api/licenses/{id}` - Update license
- `POST /api/licenses/{id}/extend` - Extend expiration
- `POST /api/licenses/{id}/revoke` - Revoke license

### Devices
- `GET /api/devices` - List devices
- `POST /api/devices/{id}/block` - Block device
- `POST /api/devices/{id}/unblock` - Unblock device
- `POST /api/devices/{id}/logout` - Force logout
- `POST /api/devices/{id}/command` - Send custom command

### Updates
- `GET /api/updates/check` - Check for application updates

### Telemetry
- `POST /api/telemetry` - Receive telemetry batch
- `GET /api/telemetry` - Query telemetry logs

### Audit
- `GET /api/audit` - Query audit logs

### VPS Monitoring
- `GET /api/vps/resources` - Current resource usage
- `GET /api/vps/containers` - Docker container list
- `GET /api/vps/websites` - Managed websites

## SignalR Hubs

### DevicesHub (`/hubs/devices`)

**Client → Server:**
- `Heartbeat()` - Periodic ping from client

**Server → Client:**
- `ReceiveCommand(HubCommand)` - Push command to client
- Command types: `Logout`, `BlockDevice`, `Notify`, `ForceUpdate`, `RefreshLicense`, `Custom`

**Client → Server (Acknowledgment):**
- `AcknowledgeCommand(commandId)` - Confirm execution

### VpsHub (`/hubs/vps`)

**Server → Client:**
- `ReceiveResourceUpdate(VpsResourceDto)` - Real-time resource stats
- `ReceiveContainerUpdate(List<VpsContainerDto>)` - Container status changes

## Sub-Projects

### [Ogur.Hub.Domain](./Ogur.Hub.Domain)
Domain entities, value objects, and business rules.

### [Ogur.Hub.Application](./Ogur.Hub.Application)
Business logic, CQRS handlers, and DTOs.

### [Ogur.Hub.Infrastructure](./Ogur.Hub.Infrastructure)
EF Core, repositories, SignalR hubs, and background services.

### [Ogur.Hub.Api](./Ogur.Hub.Api)
REST API and SignalR endpoints.

### [Ogur.Hub.Web](./Ogur.Hub.Web)
DevExpress-based admin panel.

## Environment Variables

### API (Ogur.Hub.Api)
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Database=ogurhub;User=root;Password=pass;
Jwt__SecretKey=your-secret-key
Jwt__Issuer=OgurHub
Jwt__Audience=OgurHubClients
```

### Web (Ogur.Hub.Web)
```bash
ASPNETCORE_ENVIRONMENT=Development
HubApi__BaseUrl=http://localhost:5180
```

## Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Roadmap
- Redis caching layer
- Rate limiting per API key
- Advanced telemetry analytics
- Multi-tenancy support
- Two-factor authentication (2FA)
- Webhook notifications
- GraphQL API
- Mobile monitoring app

## License
Proprietary - All rights reserved © 2025 Dominik Karczewski (ogur.dev)

## Author
**Dominik Karczewski**
- Website: [ogur.dev](https://ogur.dev)
- GitHub: [@ishkabar](https://github.com/ishkabar)
