# Plant Care App — Backend API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![SQLite](https://img.shields.io/badge/database-SQLite-003B57?logo=sqlite)
![License](https://img.shields.io/badge/license-MIT-green)

A REST API for tracking houseplants and automating watering records based on real-time weather data. Built with .NET 10 Minimal APIs.

**Frontend:** [Plant-Care-App-Frontend](https://github.com/bogdanputintsev/Plant-Care-App-Frontend)

---

## Features

- **Plant management** — create, update, and delete plants with watering schedules
- **Manual watering** — mark a plant as watered with a single request
- **JWT authentication** — register/login with access + refresh token rotation
- **Weather integration** — fetches current conditions from [Open-Meteo](https://open-meteo.com/) based on the user's coordinates
- **Auto-watering** — background service checks weather every 15 minutes and automatically updates `LastWateredDate` for all user plants when it's raining

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| API style | Minimal APIs |
| Database | SQLite + EF Core 10 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Weather | Open-Meteo API (no key required) |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
git clone https://github.com/bogdanputintsev/Plant-Care-App-Backend.git
cd Plant-Care-App-Backend/PlantCareApp.Api
dotnet run
```

The API starts at **http://localhost:5050**. The SQLite database is created and migrated automatically on first run in Development mode.

---

## API Endpoints

### Auth — `/api/auth`

| Method | Route | Description | Auth required |
|---|---|---|---|
| `POST` | `/api/auth/register` | Create account | No |
| `POST` | `/api/auth/login` | Get access + refresh tokens | No |
| `POST` | `/api/auth/refresh` | Rotate refresh token | No |
| `POST` | `/api/auth/logout` | Revoke refresh token | No |

### Plants — `/api/plants`

All plant routes require a Bearer token.

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/plants` | List all plants for the current user |
| `GET` | `/api/plants/{id}` | Get a single plant |
| `POST` | `/api/plants` | Create a plant |
| `PUT` | `/api/plants/{id}` | Update a plant |
| `PATCH` | `/api/plants/{id}/water` | Mark plant as watered today |
| `DELETE` | `/api/plants/{id}` | Delete a plant |

### Weather — `/api/weather`

| Method | Route | Description | Auth required |
|---|---|---|---|
| `GET` | `/api/weather` | Current weather at the user's coordinates | Yes |

---

## Database Migrations

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```