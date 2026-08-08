# BloodBond 🩸

> **Smart Blood Donation Network** — A .NET 8 Web API platform that connects blood donors with patients in need, intelligently matching compatible blood types across blood banks.

---

## 📋 Project Overview

BloodBond is a backend system for a smart blood donation network. It allows users (donors & requesters) to register, search for compatible blood across blood banks, request blood in urgent cases, schedule donations, and track their contribution through a gamified badge system.

This repository contains the **Week 2 delivery** of the project:
- ✅ Database configuration (Code-First)
- ✅ Project structure (3-Layer Architecture)
- ✅ SOLID Principles + Repository Pattern
- ✅ Identity / Authentication Area
- ✅ Seeding initial data (Roles)

---

## 🏗️ Architecture

The project follows a clean **3-Layer Architecture** pattern:

```
BloodBond/
├── BloodBond.DAL/         # Data Access Layer (Models, DbContext, Repositories, Seeding)
├── BloodBond.BLL/         # Business Logic Layer (Services, DTOs, Mappings)
└── BloodBond/             # Presentation Layer (Controllers, Middleware, Extensions)
```

### Why 3-Layer?
- **Separation of concerns** — each layer has one clear responsibility
- **Independent testability** — business logic doesn't depend on EF Core
- **Maintainability** — changes in one layer don't ripple to others
- **SOLID-compliant** — Dependency Inversion is naturally achieved

---

## 🧰 Tech Stack

| Category | Technology |
|----------|-----------|
| Framework | ASP.NET Core 9 Web API |
| ORM | Entity Framework Core (Code-First) |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Mapping | Mapster |
| Patterns | Repository + Generic Repository + Dependency Injection |

---

## 🧱 SOLID Principles Applied

### ✅ S — Single Responsibility
Each class has one reason to change:
- `ApplicationDbContext` → only handles DB operations
- `AuthenticationService` → only handles auth logic
- `AccountController` → only handles HTTP routing
- `GenericRepository<T>` → only generic CRUD for any entity

### ✅ O — Open/Closed
- `IGenericRepository<T>` defines a contract; any new repository extends it (e.g., `IBloodBankRepository` would extend `IGenericRepository<BloodBank>`)
- Services expose `I*Service` interfaces — clients depend on abstractions, not concrete classes

### ✅ L — Liskov Substitution
- `GenericRepository<T>` is a complete implementation of `IGenericRepository<T>` — any subclass can be swapped without breaking the contract
- `AuthenticationService` fully substitutes `IAuthenticationService` wherever the interface is required

### ✅ I — Interface Segregation
- `I*Service` interfaces are split by feature (IAuthenticationService, IEmailSender, etc.)
- `IGenericRepository<T>` is the only generic contract; specific repositories add their own methods when needed
- No client is forced to depend on methods it doesn't use

### ✅ D — Dependency Inversion
- High-level modules (Controllers) depend on `IAuthenticationService`, not `AuthenticationService`
- DI container registers `IAuthenticationService → AuthenticationService` in `ApplicationServicesExtensions`
- DbContext is injected, not instantiated

---

## 📦 Design Patterns

| Pattern | Where |
|---------|-------|
| Repository | `IGenericRepository<T>` + `GenericRepository<T>` |
| Generic Repository | Shared CRUD operations for any entity |
| Dependency Injection | All services + DbContext registered in `ApplicationServicesExtensions` |
| Strategy | `IEmailSender` allows swapping the email provider |
| Seeding | `ISeedData` interface + `RoleSeedData` implementation |

---

## 🔐 Identity & Authentication

- **ASP.NET Core Identity** is the user store (table: `Users`)
- **JWT Bearer** is used for stateless authentication
- Default role: **`User`** (assigned at registration)
- `AccountController` exposes:
  - `POST /api/account/register` — register a new user (assigns "User" role)
  - `POST /api/account/login` — login and receive JWT
  - `POST /api/account/forgot-password` — request a password reset email
  - `POST /api/account/reset-password` — complete the password reset
  - `GET /api/account/me` — get the current authenticated user

---

## 🗃️ Database (Code-First)

The database is created via EF Core migrations. The connection string is configured in `appsettings.json` under `ConnectionStrings:DefaultConnection`.

Currently seeded data:
- Roles: `Admin`, `User`

Tables created by Identity:
- `Users`, `Roles`, `UserRoles`, `UserClaims`, `RoleClaims`, `UserLogins`, `UserTokens`

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or any instance)
- Visual Studio 2022 / VS Code

### Run
```bash
# Apply migrations
dotnet ef database update --project BloodBond.DAL --startup-project BloodBond

# Run the API
dotnet run --project BloodBond
```

The API will be available at `https://localhost:7xxx`.

---

## 🧪 Testing

Test the auth endpoints using Postman or Swagger:
- `POST /api/account/register`
- `POST /api/account/login`
- `GET /api/account/me` (requires Bearer token)

---

## 📅 Delivery Status

| Week | Scope | Status |
|------|-------|--------|
| Week 2 | DB Config + Structure + SOLID + Identity + Seed | ✅ Current |
| Week 3+ | Blood Banks, Requests, Donations, Events, Badges, etc. | 🔜 Upcoming |

---

## 📄 License

This is an academic project for course delivery.
