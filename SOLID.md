# SOLID Principles — BloodBond

This document explains how each SOLID principle is applied in the BloodBond project, with concrete file references.

---

## 1️⃣ Single Responsibility Principle (SRP)

> *"A class should have one, and only one, reason to change."*

| Class | Single Responsibility |
|-------|------------------------|
| `ApplicationDbContext` | Manage database connections, entity configurations, and audit fields |
| `GenericRepository<T>` | Generic CRUD operations for any entity (no business rules) |
| `AuthenticationService` | Registration, login, JWT generation, password reset |
| `EmailSender` | Sending emails only |
| `AccountController` | Handle HTTP requests/responses for the auth area |
| `RoleSeedData` | Seed roles into the database |
| `GlobalExceptionHandling` | Convert exceptions into standardized HTTP responses |

**Why this matters:** A change in JWT settings doesn't touch the DbContext. A change in email template doesn't touch the controller. Each class changes for one reason.

---

## 2️⃣ Open/Closed Principle (OCP)

> *"Software entities should be open for extension, but closed for modification."*

### Where it shows up:

- **`IGenericRepository<T>`** is the contract. New modules (e.g., `BloodBank`) can extend it through `IBloodBankRepository : IGenericRepository<BloodBank>` without modifying the base interface.

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

- **`ISeedData`** — new seeders (e.g., `AdminUserSeedData`, `BloodTypeSeedData`) can be added without touching existing ones.

```csharp
public interface ISeedData
{
    Task SeedAsync();
}
```

- **`IEmailSender`** — switch from SMTP to SendGrid by providing a new implementation. No code change in `AuthenticationService` is required.

---

## 3️⃣ Liskov Substitution Principle (LSP)

> *"Objects of a superclass shall be replaceable with objects of a subclass without breaking the application."*

### Where it shows up:

- `GenericRepository<T>` is a **complete implementation** of `IGenericRepository<T>`. Any place that depends on the interface works seamlessly with the concrete class.
- `AuthenticationService` is a full implementation of `IAuthenticationService` — the controller depends only on the interface.
- If we later introduce `CachedAuthenticationService : IAuthenticationService`, it will substitute the original without any client change.

The base class `AuditableEntity` does not violate LSP: every concrete entity (e.g., `ApplicationUser` in future) inherits shared audit properties but doesn't change the contract.

---

## 4️⃣ Interface Segregation Principle (ISP)

> *"Many client-specific interfaces are better than one general-purpose interface."*

### Where it shows up:

- **No fat interfaces.** Each feature has its own interface:
  - `IAuthenticationService` — auth only
  - `IEmailSender` — email only
  - `ISeedData` — seeding only
- **`IGenericRepository<T>`** is intentionally minimal (5 methods). Module-specific repositories (when added) will extend it with their own narrow contract — they won't bloat the generic one.
- **Clients depend only on what they use:** `AccountController` depends on `IAuthenticationService` and `IEmailSender`, not on a "god" service interface.

---

## 5️⃣ Dependency Inversion Principle (DIP)

> *"Depend on abstractions, not on concretions."*

### Where it shows up:

- **Controllers depend on interfaces:**
```csharp
public class AccountController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IEmailSender _emailSender;

    public AccountController(IAuthenticationService authService, IEmailSender emailSender)
    { ... }
}
```

- **High-level modules do not instantiate low-level modules.** Wiring happens once, in `ApplicationServicesExtensions`:

```csharp
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IEmailSender, EmailSender>();
services.AddScoped<ISeedData, RoleSeedData>();
```

- **`DbContext` is injected**, never `new`'d. This enables testing with an in-memory provider and centralizes configuration.

---

## 🧩 Bonus: Repository Pattern + Generic Repository

- **Repository Pattern** abstracts data access — the business layer doesn't know about EF Core.
- **Generic Repository** removes duplication — common CRUD lives once in `GenericRepository<T>`.
- **Module-specific repositories** (added in future weeks) extend the generic one and add feature-specific queries.

---

## Summary Table

| Principle | Evidence (file/interface) |
|-----------|---------------------------|
| SRP | `AuthenticationService`, `GenericRepository<T>`, `AccountController` |
| OCP | `IGenericRepository<T>`, `ISeedData`, `IEmailSender` |
| LSP | `GenericRepository<T> : IGenericRepository<T>` (full implementation) |
| ISP | Feature-split interfaces: `IAuthenticationService`, `IEmailSender`, `ISeedData` |
| DIP | `AccountController → IAuthenticationService`; DI registration in `ApplicationServicesExtensions` |
| Repository | `IGenericRepository<T>` + `GenericRepository<T>` |
