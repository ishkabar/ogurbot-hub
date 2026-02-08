# Ogur.Hub.Web

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-hub.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-hub)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)

DevExpress-based admin panel for Hub management with real-time SignalR dashboards.

## Structure

### Controllers (MVC)
- `HomeController` - Dashboard and settings
- `AccountController` - Login/logout
- `ApplicationsController` - Application management
- `LicensesController` - License management
- `DevicesController` - Device monitoring
- `UsersController` - User administration
- `VpsController` - VPS monitoring dashboard
- `TraefikController` - Traefik integration (planned)

### Services
- `HubApiClient` - HTTP client for Hub API
- `IHubApiClient` - Typed API client interface

### ViewModels
- `DashboardViewModel` - Statistics and charts
- `ApplicationsViewModel` - Application grid
- `LicensesViewModel` - License grid with actions
- `DevicesViewModel` - Device status and control
- `UsersViewModel` - User management
- `VpsMonitoringViewModel` - Real-time VPS stats

### Views
- **Home**: Dashboard, Settings, Privacy
- **Account**: Login, Register
- **Applications**: Index (grid)
- **Licenses**: Index (grid with create/edit)
- **Devices**: Index (grid with block/unblock)
- **Users**: Index (grid)
- **Vps**: Index (real-time monitoring)
- **Shared**: Layout, base templates

## Features
- DevExpress grids with sorting, filtering, paging
- Real-time VPS monitoring with Chart.js
- Bootstrap 5.3 responsive layout
- SignalR real-time updates
- JWT session management

## Dependencies
- `DevExpress.AspNetCore`
- `Microsoft.AspNetCore.SignalR.Client`
- Bootstrap 5.3, jQuery
