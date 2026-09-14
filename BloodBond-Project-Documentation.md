# 🩸 Blood Bond — التوثيق الكامل للمشروع

## جدول المحتويات

1. [نظرة عامة على المشروع](#1-نظرة-عامة-على-المشروع)
2. [البنية التقنية (Tech Stack)](#2-البنية-التقنية-tech-stack)
3. [الـ Backend — هيكل المشروع](#3-ال-backend--هيكل-المشروع)
4. [الـ Frontend — هيكل المشروع](#4-ال-frontend--هيكل-المشروع)
5. [قائمة كل الـ API Endpoints مع الشرح](#5-قائمة-كل-ال-api-endpoints-مع-الشرح)
6. [سيناريوهات الاستخدام بالـ Postman](#6-سيناريوهات-الاستخدام-بالـ-postman)
7. [الـ Roles والصلاحيات](#7-ال-roles-والصلاحيات)
8. [الـ Enums والحالات](#8-ال-enums-والحالات)
9. [الـ Deployment والنشر](#9-ال-deployment-والنشر)
10. [حل المشاكل الشائعة (Troubleshooting)](#10-حل-المشاكل-الشائعة-troubleshooting)

---

## 1. نظرة عامة على المشروع

**Blood Bond** هو نظام لإدارة التبرع بالدم، يربط بين:
- **المتبرعين** (Donors / Users)
- **بنوك الدم** (Blood Banks) مع مخزون لكل فصيلة
- **المرضى** اللي يطلبوا دم (Blood Requests)
- **البنوك المديرين** (Blood Bank Managers) اللي يديروا بنوكهم
- **الـ Admins** اللي يوافقوا على البنوك والـ donations والـ users

### الواجهة الكاملة:
- ✅ Authentication (Login / Register / Forgot Password)
- ✅ User Roles (Admin / BloodBankManager / User)
- ✅ Blood Banks Management (Create / Edit / Approve / Reject / Inventory)
- ✅ Blood Requests (Patient requests → Manager fulfills)
- ✅ Donations (Schedule → Approve → Complete → Award Points)
- ✅ Monetary Donations (Stripe Checkout)
- ✅ Events (Blood drives / Community meetups)
- ✅ Badges (Gamification / Leaderboard)
- ✅ Ratings & Reviews

---

## 2. البنية التقنية (Tech Stack)

### Backend:
| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core | .NET 9.0 | Web API framework |
| Entity Framework Core | 9.0 | ORM for database |
| SQL Server | Latest | Database (on MonsterASP) |
| ASP.NET Identity | Built-in | User management |
| JWT Bearer | Built-in | Authentication tokens |
| Stripe.net | Latest | Payment processing |
| BCrypt | Built-in | Password hashing |

### Frontend:
| Technology | Version | Purpose |
|------------|---------|---------|
| React | 18.3 | UI framework |
| Vite | 5.4 | Build tool & dev server |
| React Router | 6.27 | Client-side routing |
| React Bootstrap | 2.10 | UI components |
| Bootstrap | 5.3 | CSS framework |
| Axios | 1.7 | HTTP client |
| React Hot Toast | 2.4 | Notifications |

### Infrastructure:
- **Backend Host**: MonsterASP (`https://blood-bond.runasp.net`)
- **Database Host**: MonsterASP Database (`db65353`)
- **Frontend Host**: Vercel (deployed via Vercel CLI / Git)
- **Payment**: Stripe (Test mode: `sk_test_51TOl82B...`)

---

## 3. الـ Backend — هيكل المشروع

```
BloodBond/
├── BloodBond.sln                              # Solution file
├── BloodBond/                                  # Main API project
│   ├── Program.cs                              # Entry point
│   ├── appsettings.json                        # Config (empty for secrets)
│   ├── Properties/launchSettings.json          # Dev environment = "Development"
│   ├── Controllers/                            # API endpoints
│   │   ├── AccountController.cs                # Auth (login/register/etc)
│   │   ├── AdminController.cs                  # Admin operations
│   │   ├── BloodBanksController.cs             # Blood banks CRUD
│   │   ├── BloodRequestsController.cs          # Patient requests
│   │   ├── DonationsController.cs              # Donor schedules
│   │   ├── EligibilityController.cs            # Eligibility check
│   │   ├── EventsController.cs                 # Blood drives / events
│   │   ├── MonetaryDonationsController.cs       # Stripe payments
│   │   ├── RatingsController.cs                 # Bank reviews
│   │   ├── BadgesController.cs                  # Gamification
│   │   └── DebugController.cs                  # Diagnostic endpoint
│   ├── Extensinos/                             # Service registration
│   │   ├── ApplicationServicesExtensions.cs    # AddApplicationServices()
│   │   ├── CorsPolicyExtensions.cs             # AddCorsPolicy()
│   │   ├── IdentityExtensions.cs                # AddIdentityServices()
│   │   └── BuilderExtensions.cs                 # Middleware pipeline
│   └── Middleware/
│       └── GlobalExceptionHandling.cs          # Maps exceptions to HTTP codes
├── BloodBond.BLL/                              # Business Logic Layer
│   ├── Service/
│   │   ├── BloodBankService.cs                 # + IsAdminOrManager()
│   │   ├── BloodRequestService.cs              # + GetForBankAsync()
│   │   ├── DonationService.cs
│   │   ├── EligibilityService.cs
│   │   ├── EventService.cs                     # BloodDriveEventService
│   │   ├── MonetaryDonationService.cs           # Stripe Checkout + GetByBankAsync + GetAllAsync
│   │   ├── RatingService.cs
│   │   ├── BadgeService.cs
│   │   ├── UserManagementService.cs             # Admin operations
│   │   ├── IStripeSettings.cs                  # StripeSettings class
│   │   └── ...
│   ├── DTO/
│   │   ├── Request/                            # Input DTOs
│   │   │   ├── BloodDriveEventRequest.cs        # { BloodBankId, Title, Location, EventDate, Description?, Capacity }
│   │   │   ├── BloodRequestRequest.cs           # { BloodType, UnitsNeeded, UrgencyLevel, City, Notes }
│   │   │   ├── DonationRequest.cs               # { BloodBankId, ScheduledDate, Notes? }
│   │   │   ├── MonetaryDonationRequest.cs       # { Amount, Currency, BloodBankId? }
│   │   │   └── ChangeRoleRequest.cs             # { Role }
│   │   └── Response/                           # Output DTOs
│   │       ├── BloodDriveEventResponse.cs       # { Id, BloodBankId, BloodBankName, Title, Location, EventDate, Capacity, RegisteredCount }
│   │       ├── BloodRequestResponse.cs           # { Id, BloodType, UnitsNeeded, UrgencyLevel, Status, City }
│   │       ├── DonationResponse.cs              # { Id, BloodBankId, BloodBankName, ScheduledDate, Status, UnitsDonated }
│   │       ├── MonetaryDonationResponse.cs       # { Id, DonorId, DonorName, BloodBankId, BloodBankName, Amount, Currency, Status }
│   │       ├── PaymentIntentResponse.cs         # { ClientSecret, PaymentIntentId, CheckoutUrl, IsMock, Amount, Currency }
│   │       ├── UserListResponse.cs               # { Id, FullName, Email, IsBlocked, Roles[] }
│   │       └── EligibilityResponse.cs
│   └── Service/
│       └── IDonationService.cs
└── BloodBond.DAL/                              # Data Access Layer
    ├── Data/
    │   └── ApplicationDbContext.cs              # EF Core context
    ├── Models/
    │   ├── ApplicationUser.cs                    # Custom user with BloodType, City, etc
    │   ├── BloodBank.cs                          # Blood bank entity
    │   ├── BloodInventory.cs                     # Blood units by type
    │   ├── BloodRequest.cs                       # Patient request
    │   ├── Donation.cs                           # Donor appointment
    │   ├── BloodDriveEvent.cs                    # Event
    │   ├── EventAttendance.cs                    # Event RSVP
    │   ├── MonetaryDonation.cs                   # Stripe payment
    │   ├── Rating.cs                             # Bank review
    │   ├── Badge.cs                              # Gamification
    │   ├── Notification.cs                       # System notifications
    │   └── ...
    ├── Enums/
    │   ├── BloodType.cs                          # 0..7 (A+, A-, B+, B-, AB+, AB-, O+, O-)
    │   ├── UrgencyLevel.cs                       # 0..3 (Low, Normal, High, Critical)
    │   ├── DonationStatus.cs                     # 0..4 (Scheduled, Approved, Rejected, Completed, Cancelled)
    │   ├── RequestStatus.cs                      # 0..4 (Pending, InProgress, Fulfilled, Cancelled, Expired)
    │   ├── BloodBankStatus.cs                    # 0..3 (Pending, Verified, Rejected, Suspended)
    │   └── CheckInStatus.cs                      # 0..3 (Registered, CheckedIn, Cancelled, NoShow)
    └── Repository/
        └── ...
```

---

## 4. الـ Frontend — هيكل المشروع

```
BloodBond.Web/
├── package.json
├── vite.config.js                           # Dev server + proxy
├── vercel.json                              # SPA rewrite for Vercel
├── .env                                     # VITE_API_BASE_URL
├── public/
│   └── favicon.svg
├── dist/                                    # Production build output
└── src/
    ├── main.jsx                             # Entry: BrowserRouter + AuthProvider + Bootstrap
    ├── App.jsx                              # All routes
    ├── config.js                            # API_BASE_URL + token keys
    ├── styles.css                           # Global styles
    ├── api/                                 # Typed wrappers around every endpoint
    │   ├── client.js                        # Axios instance + JWT interceptor + 401 redirect
    │   ├── auth.js                          # /api/Account/*
    │   ├── admin.js                         # /api/admin/*
    │   ├── bloodBanks.js                    # /api/bloodbanks/*
    │   ├── bloodRequests.js                 # /api/bloodrequests/*
    │   ├── donations.js                     # /api/donations/* + /api/eligibility/*
    │   ├── events.js                        # /api/events/* + /api/Events/upcoming
    │   ├── monetary.js                      # /api/monetarydonations/*
    │   ├── ratings.js                       # /api/ratings/*
    │   ├── badges.js                        # /api/badges/*
    │   └── index.js                         # Barrel exports
    ├── components/
    │   ├── AppLayout.jsx                    # Navbar + Outlet + Footer
    │   ├── AppNavbar.jsx                    # Top navigation (role-based links)
    │   ├── ProtectedRoute.jsx                # Auth guard + role guard
    │   ├── PageHeader.jsx                   # Page title + subtitle + actions
    │   ├── Loading.jsx                       # Spinner
    │   ├── EmptyState.jsx                    # Empty list placeholder
    │   └── CodeModal.jsx                     # (in AdminDiagnosticPage)
    ├── context/
    │   ├── AuthContext.jsx                  # user, token, role, login(), register(), logout()
    │   └── constants.js                     # BLOOD_TYPES, URGENCY_LEVELS, ROLES, status enums
    ├── pages/
    │   ├── HomePage.jsx                     # Public landing
    │   ├── LoginPage.jsx                    # /login
    │   ├── RegisterPage.jsx                 # /register
    │   ├── ForgotPasswordPage.jsx            # /forgot
    │   ├── DashboardPage.jsx                # /dashboard (role-based: Admin/Manager/Donor)
    │   ├── BloodBanksPage.jsx               # /blood-banks
    │   ├── BloodRequestsPage.jsx            # /requests (manager view by default)
    │   ├── DonationsPage.jsx                # /donations
    │   ├── MonetaryDonationsPage.jsx        # /monetary (with mock mode fallback)
    │   ├── AdminMonetaryPage.jsx            # /admin/monetary (admin: all donations)
    │   ├── EligibilityPage.jsx              # /eligibility
    │   ├── EventsPage.jsx                   # /events (3 tabs: Upcoming / Mine / Manage)
    │   ├── BadgesPage.jsx                   # /badges (leaderboard)
    │   ├── RatingsPage.jsx                  # /ratings
    │   ├── ProfilePage.jsx                  # /profile
    │   ├── AdminUsersPage.jsx               # /admin/users
    │   ├── AdminDiagnosticPage.jsx          # /admin/diagnostic (status of all features)
    │   ├── ContactUsPage.jsx                # /contact (mailto fallback)
    │   ├── ForbiddenPage.jsx                # /forbidden (403)
    │   └── NotFoundPage.jsx                 # /404
    └── utils/
        └── error.js                         # apiErrorMessage(err) → friendly message
```

---

## 5. قائمة كل الـ API Endpoints مع الشرح

### 🔐 Auth Endpoints (`/api/Account/*`)

#### `POST /api/Account/Register`
- **Auth**: Public (AllowAnonymous)
- **Body**:
  ```json
  {
    "fullName": "John Doe",
    "email": "john@example.com",
    "password": "Pass@1234",
    "confirmPassword": "Pass@1234"
  }
  ```
- **Response**: `200 OK` (no body) | `400 Bad Request` if validation fails
- **C#**: `AccountController.Register`

#### `POST /api/Account/login`
- **Auth**: Public
- **Body**:
  ```json
  {
    "email": "admin@bloodbond.com",
    "password": "Admin@123456"
  }
  ```
- **Response**: `200 OK` with token payload:
  ```json
  {
    "userId": "e5f3abfe-f505-42a4-b8b6-9f4102ff6c15",
    "email": "admin@bloodbond.com",
    "fullName": "System Admin",
    "roles": ["Admin"],
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-09-14T09:19:04.0566991Z"
  }
  ```
- **Notes**: Token is a JWT. Frontend stores in `localStorage.bb_token`.

#### `GET /api/Account/me`
- **Auth**: Required (any role)
- **Headers**: `Authorization: Bearer <token>`
- **Response**: `200 OK` with user info | `401 Unauthorized` if invalid token

#### `POST /api/Account/forgot-password`
- **Auth**: Public
- **Body**: `{ "email": "user@example.com" }`
- **Response**: `200 OK` + email containing reset token

#### `POST /api/Account/reset-password`
- **Auth**: Public
- **Body**:
  ```json
  {
    "email": "user@example.com",
    "token": "CfDJ8KOC0RNyT9JFoSbYJxYc5gP2JG6u3JqTSriy...",
    "newPassword": "NewPass@123",
    "confirmPassword": "NewPass@123"
  }
  ```

### 👑 Admin Endpoints (`/api/admin/*`)

#### `POST /api/admin/register-first`
- **Auth**: Public (only works once — refuses if any admin exists)
- **Body**:
  ```json
  {
    "secretKey": "dev-key-12345-abcde",
    "fullName": "Bootstrap Admin",
    "email": "bootstrap.admin@bloodbond.com",
    "password": "Bootstrap@2026"
  }
  ```

#### `POST /api/admin/create`
- **Auth**: Admin only
- **Body**:
  ```json
  {
    "fullName": "Test Manager",
    "email": "test.manager@example.com",
    "password": "Manager@123",
    "role": "BloodBankManager"
  }
  ```
- **role**: `User` | `BloodBankManager` | `Admin`

#### `POST /api/admin/change-password`
- **Auth**: Admin only
- **Body**:
  ```json
  {
    "currentPassword": "Admin@123456",
    "newPassword": "NewAdmin@123",
    "confirmPassword": "NewAdmin@123"
  }
  ```

#### `GET /api/admin/users`
- **Auth**: Admin only
- **Response**: Array of `UserListResponse`:
  ```json
  [
    {
      "id": "e5f3abfe-f505-42a4-b8b6-9f4102ff6c15",
      "fullName": "System Admin",
      "email": "admin@bloodbond.com",
      "phoneNumber": "123456",
      "isBlocked": false,
      "createdAt": "2026-08-01T00:00:00Z",
      "roles": ["Admin"]
    }
  ]
  ```

#### `GET /api/admin/users/{id}`
- **Auth**: Admin only
- **Response**: Single `UserListResponse`

#### `PATCH /api/admin/users/{id}/block`
- **Auth**: Admin only
- **Response**: Updated user

#### `PATCH /api/admin/users/{id}/unblock`
- **Auth**: Admin only

#### `PATCH /api/admin/users/{id}/role`
- **Auth**: Admin only
- **Body**: `{ "role": "User" }` (or `BloodBankManager`, `Admin`)
- **Response**: Updated user

#### `GET /api/admin/analytics`
- **Auth**: Admin only
- **Response**:
  ```json
  {
    "Users":    { "Total": 6 },
    "BloodBanks": { "Total": 1, "Verified": 1, "Pending": 0 },
    "Requests":  { "Total": 5, "Pending": 4, "Fulfilled": 0, "CriticalPending": 0 },
    "Donations": { "Total": 0, "Completed": 0 },
    "Monetary":  { "TotalDonatedUSD": 0 },
    "Inventory": { "LowStockItems": 0 },
    "BloodTypeDistribution": []
  }
  ```

### 🏥 Blood Banks Endpoints (`/api/bloodbanks/*`)

#### `GET /api/bloodbanks`
- **Auth**: Public (AllowAnonymous)
- **Response**: Array of `BloodBankResponse`

#### `GET /api/bloodbanks/verified`
- **Auth**: Public
- **Response**: Array (only Verified banks)

#### `GET /api/bloodbanks/{id}`
- **Auth**: Public
- **Response**: Single `BloodBankResponse`

#### `POST /api/bloodbanks`
- **Auth**: User, Admin
- **Body**:
  ```json
  {
    "name": "Ramallah Central Blood Bank",
    "cityAddress": "Main Street, Ramallah",
    "latitude": 31.9038,
    "longitude": 35.2030,
    "contactPhone": "+970599000000"
  }
  ```
- **Behavior**: Creates a bank with status = `Pending`. Auto-promotes User → `BloodBankManager`.

#### `GET /api/bloodbanks/mine`
- **Auth**: BloodBankManager, Admin
- **Response**: The bank managed by current user

#### `PUT /api/bloodbanks/{id}`
- **Auth**: BloodBankManager, Admin (with **IsAdminOrManager** check)
- **Body**: Same as POST
- **Backend fix**: `BloodBankService.IsAdminOrManager()` allows admins to edit any bank.

#### `PUT /api/bloodbanks/{id}/inventory`
- **Auth**: BloodBankManager, Admin
- **Body**:
  ```json
  [
    { "bloodType": 0, "unitsAvailable": 15 },
    { "bloodType": 1, "unitsAvailable": 8 },
    { "bloodType": 6, "unitsAvailable": 22 }
  ]
  ```
- **Backend fix**: `SetInventoryAsync` uses **upsert** (find by BloodBankId+BloodType, update if exists, insert otherwise). No more duplicate key errors.

#### `PATCH /api/bloodbanks/{id}/approve`
- **Auth**: Admin only
- **Response**: Bank with status = `Verified`

#### `PATCH /api/bloodbanks/{id}/reject`
- **Auth**: Admin only
- **Response**: Bank with status = `Rejected`

#### `GET /api/bloodbanks/low-stock`
- **Auth**: Public
- **Response**: Array of `BloodInventoryResponse` where units < 5

### 🩸 Blood Requests Endpoints (`/api/bloodrequests/*`)

#### `POST /api/bloodrequests`
- **Auth**: Required (any user)
- **Body**:
  ```json
  {
    "bloodType": 0,
    "unitsNeeded": 2,
    "urgencyLevel": 2,
    "city": "Ramallah",
    "notes": "Urgent surgery needed"
  }
  ```
- **Behavior**: Creates request with status = `Pending`. Auto-notifies compatible donors.

#### `GET /api/bloodrequests/mine`
- **Auth**: Required
- **Response**: Requests created by current user

#### `GET /api/bloodrequests/active?city=Ramallah`
- **Auth**: Public
- **Query**: `city` (required)
- **Response**: Active requests (Pending or InProgress) in the city

#### `GET /api/bloodrequests/by-bank/{bankId}`
- **Auth**: BloodBankManager, Admin
- **Response**: Active requests in the city of the bank

#### `GET /api/bloodrequests/{id}`
- **Auth**: Required

#### `PATCH /api/bloodrequests/{id}/cancel`
- **Auth**: Required (owner only)
- **Response**: Request with status = `Cancelled`

#### `PATCH /api/bloodrequests/{id}/fulfill`
- **Auth**: BloodBankManager, Admin
- **Response**: Request with status = `Fulfilled`

#### `POST /api/bloodrequests/{id}/notify`
- **Auth**: Admin, BloodBankManager
- **Response**: `{ "notified": 5 }` (count of compatible donors notified)

### 💉 Donations Endpoints (`/api/donations/*`)

#### `POST /api/donations`
- **Auth**: Required (must pass eligibility)
- **Body**:
  ```json
  {
    "bloodBankId": 1,
    "scheduledDate": "2026-12-15T10:00:00",
    "notes": "First time donor"
  }
  ```

#### `GET /api/donations/mine`
- **Auth**: Required
- **Response**: My donations (any status)

#### `GET /api/donations/by-bank/{bankId}`
- **Auth**: BloodBankManager, Admin
- **Response**: Donations to that bank

#### `GET /api/donations/{id}`

#### `PATCH /api/donations/{id}/cancel`
- **Auth**: Required (owner)

#### `PATCH /api/donations/{id}/approve`
- **Auth**: BloodBankManager, Admin
- **Status change**: `Scheduled` → `Approved`

#### `PATCH /api/donations/{id}/reject`
- **Auth**: BloodBankManager, Admin
- **Status change**: `Scheduled` → `Rejected`

#### `PATCH /api/donations/{id}/complete`
- **Auth**: BloodBankManager, Admin
- **Body**: `{ "unitsDonated": 1, "notes": "Successful" }`
- **Status change**: → `Completed`. Updates inventory. Awards points to donor.

### ✅ Eligibility Endpoints (`/api/eligibility/*`)

#### `POST /api/eligibility`
- **Auth**: Required
- **Body**:
  ```json
  {
    "weight": 75,
    "age": 28,
    "hasChronicDisease": false,
    "lastSurgeryDate": null
  }
  ```
- **Response**: `{ "passed": true, "reasons": [] }` or `{"passed": false, "reasons": ["..."]}`

#### `GET /api/eligibility/latest`
- **Auth**: Required
- **Response**: Latest eligibility check result

### 📅 Events Endpoints (`/api/events/*`)

#### `GET /api/events/upcoming`
- **Auth**: Public
- **Response**: Upcoming events list

#### `GET /api/events/mine`
- **Auth**: Required
- **Response**: Events I'm registered for

#### `GET /api/events/by-bank/{bankId}`
- **Auth**: BloodBankManager, Admin

#### `POST /api/events/{id}/register`
- **Auth**: Required
- **Body**: Empty (uses current user)

#### `POST /api/events/{id}/cancel`
- **Auth**: Required (cancel RSVP)

#### `POST /api/events/{id}/checkin`
- **Auth**: Required

#### `GET /api/events/{id}/attendees`
- **Auth**: BloodBankManager, Admin

#### `POST /api/events` / `PUT /api/events/{id}` / `DELETE /api/events/{id}`
- **Auth**: BloodBankManager, Admin
- **Body**:
  ```json
  {
    "bloodBankId": 1,
    "title": "Ramallah Blood Drive",
    "location": "Al-Watani Hospital",
    "eventDate": "2026-12-15T10:00:00",
    "description": "Open day, free t-shirts",
    "capacity": 80
  }
  ```

### 💰 Monetary Donations Endpoints (`/api/monetarydonations/*`)

#### `POST /api/monetarydonations/create-intent`
- **Auth**: Required
- **Body**:
  ```json
  {
    "amount": 25.00,
    "currency": "usd",
    "bloodBankId": 1
  }
  ```
- **Response** (real Stripe):
  ```json
  {
    "clientSecret": "cs_test_...",
    "paymentIntentId": "cs_test_...",
    "checkoutUrl": "https://checkout.stripe.com/c/pay/cs_test_...",
    "sessionId": "cs_test_...",
    "amount": 25.00,
    "currency": "usd",
    "status": "checkout_created",
    "isMock": false
  }
  ```
- **Response** (mock mode — no Stripe key):
  ```json
  {
    "clientSecret": "pi_mock_xxx_secret_mock",
    "paymentIntentId": "pi_mock_xxx",
    "isMock": true,
    "status": "requires_payment_method"
  }
  ```

#### `POST /api/monetarydonations/webhook`
- **Auth**: Public (called by Stripe)
- **Behavior**: Stripe sends `payment_intent.succeeded` → updates donation status to `Succeeded`

#### `POST /api/monetarydonations/confirm?paymentIntentId=X&status=Succeeded`
- **Auth**: Public (used for manual confirmation or testing)

#### `GET /api/monetarydonations/mine`
- **Auth**: Required
- **Response**: My donations

#### `GET /api/monetarydonations/total/mine`
- **Auth**: Required
- **Response**: `{ "total": 25, "currency": "USD" }`

#### `GET /api/monetarydonations/by-bank/{bankId}`
- **Auth**: BloodBankManager, Admin
- **Response**: `{ "total": 100, "currency": "USD" }` (total donations to that bank)

#### `GET /api/monetarydonations/by-bank-detail/{bankId}`
- **Auth**: BloodBankManager, Admin
- **Response**: Array of donations for that bank

#### `GET /api/monetarydonations/all`
- **Auth**: Admin only
- **Response**: All donations across the system

### ⭐ Ratings Endpoints (`/api/ratings/*`)

#### `POST /api/ratings`
- **Auth**: Required
- **Body**:
  ```json
  {
    "bloodBankId": 1,
    "rating": 5,
    "comment": "Excellent service!"
  }
  ```

#### `GET /api/ratings/by-bank/{bankId}`

#### `GET /api/ratings/mine/{bankId}`

#### `GET /api/ratings/stats/{bankId}`

### 🏅 Badges Endpoints (`/api/badges/*`)

#### `GET /api/badges`
- **Auth**: Required
- **Response**: All available badges

#### `GET /api/badges/mine`
- **Auth**: Required
- **Response**: Badges earned by current user

#### `GET /api/badges/me/rank`
- **Auth**: Required
- **Response**: `{ "rank": 5, "points": 120 }`

#### `GET /api/badges/leaderboard?top=10`
- **Auth**: Required

---

## 6. سيناريوهات الاستخدام بالـ Postman

### Setup الأولي:
1. **Login** as admin → احفظ الـ token
2. Environment variable: `token = <paste-token>`
3. Environment variable: `baseUrl = https://blood-bond.runasp.net`

### السيناريو 1: Admin يدير المستخدمين
```
1. POST /api/admin/create
   - Headers: Authorization: Bearer {{token}}
   - Body: { "fullName": "Test Manager", "email": "test@bb.com", "password": "Test@1234", "role": "BloodBankManager" }
   → ينشئ User جديد بدور BloodBankManager

2. GET /api/admin/users
   → يشوف كل الـ users + roles[]

3. PATCH /api/admin/users/{id}/role
   - Body: { "role": "Admin" }
   → يغير الـ role

4. PATCH /api/admin/users/{id}/block
   → يحظر الـ user

5. GET /api/admin/analytics
   → يشوف إحصائيات النظام
```

### السيناريو 2: Blood Bank flow كامل
```
1. User يسجل: POST /api/Account/register
   → Account جديد + Token

2. User ينشئ بنك: POST /api/bloodbanks
   - Body: { "name": "...", "cityAddress": "..." }
   → Bank مع status = Pending, User promoted to BloodBankManager

3. Admin يوافق: PATCH /api/bloodbanks/{id}/approve
   → Bank.status = Verified

4. Manager يحدث الـ inventory: PUT /api/bloodbanks/{id}/inventory
   - Body: [{ "bloodType": 0, "unitsAvailable": 10 }, ...]
   → Inventory محدّث

5. Admin يسجل دخول ويقدر يعمل Edit/Inventory لأي بنك
```

### السيناريو 3: Patient + Manager flow
```
1. Patient يسجل: POST /api/Account/register
2. Patient يطلب دم: POST /api/bloodrequests
   - Body: { "bloodType": 0, "unitsNeeded": 2, "urgencyLevel": 2, "city": "Ramallah" }
   → Request.Status = Pending
3. Manager يفتح: GET /api/bloodrequests/by-bank/1
   → يشوف الطلبات لبنكه
4. Manager يوافق: PATCH /api/bloodrequests/{id}/fulfill
   → Status = Fulfilled
```

### السيناريو 4: Donation flow كامل
```
1. Donor يعمل eligibility: POST /api/eligibility
   - Body: { "weight": 75, "age": 28, "hasChronicDisease": false }
   → { passed: true }

2. Donor يحجز: POST /api/donations
   - Body: { "bloodBankId": 1, "scheduledDate": "2026-12-15T10:00:00" }
   → Status = Scheduled

3. Manager يوافق: PATCH /api/donations/{id}/approve
   → Status = Approved

4. Manager يكمّل: PATCH /api/donations/{id}/complete
   - Body: { "unitsDonated": 1, "notes": "Successful" }
   → Status = Completed, inventory updated, points awarded
```

### السيناريو 5: Monetary Donation flow
```
1. Donor يعمل donate: POST /api/monetarydonations/create-intent
   - Body: { "amount": 25, "currency": "usd", "bloodBankId": 1 }
   → Response: { checkoutUrl, ... } (real Stripe) أو { isMock: true } (mock)

2. (Real Stripe) User يدخل معلومات الكارت في checkout.stripe.com
3. (Mock mode) Frontend يفتح modal فيه test card form
4. Stripe webhook يحدّث status: Succeeded
5. Manager يفتح: GET /api/monetarydonations/by-bank-detail/1
   → يشوف donations بنكه
```

---

## 7. الـ Roles والصلاحيات

| Role | الصلاحيات |
|------|----------|
| **User (Donor)** | Register/Login, Schedule donation, Create blood request, Donate money, Register for events, View badges/rank |
| **BloodBankManager** | كل شي User + Edit/Inventory لـ bankه, Approve/Reject donations/requests, Create events, View bank's donations |
| **Admin** | كل شي Manager + Manage users, Approve/Reject banks, View all donations, View analytics |

### كيف Admin Override:
الـ `BloodBankService` فيه method `IsAdminOrManager()`:
```csharp
private bool IsAdminOrManager(string userId, string? managerId)
{
    if (managerId == userId) return true;  // Same manager
    var role = _http.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    return role == "Admin";  // Admins can override
}
```

الـ `GlobalExceptionHandling` بيرجّع:
- `UnauthorizedAccessException` → `403 Forbidden`
- `KeyNotFoundException` → `404 Not Found`
- `InvalidOperationException` → `409 Conflict`
- `Exception` → `500 Internal Server Error`

---

## 8. الـ Enums والحالات

### `BloodType` (0..7):
```
0 = A+    4 = AB+
1 = A-    5 = AB-
2 = B+    6 = O+
3 = B-    7 = O-
```

### `UrgencyLevel` (0..3):
```
0 = Low       (secondary)
1 = Normal    (info)
2 = High      (warning)
3 = Critical  (danger)
```

### `DonationStatus` (0..4):
```
0 = Scheduled   (warning)   - awaiting bank approval
1 = Approved    (info)      - approved, awaiting donation day
2 = Rejected    (danger)    - rejected by bank
3 = Completed   (success)   - donated successfully
4 = Cancelled   (secondary) - cancelled by donor
```

### `RequestStatus` (0..4):
```
0 = Pending     (warning)
1 = InProgress  (info)
2 = Fulfilled   (success)
3 = Cancelled   (secondary)
4 = Expired     (danger)
```

### `BloodBankStatus` (0..3):
```
0 = Pending    (warning)
1 = Verified   (success)
2 = Rejected   (danger)
3 = Suspended  (secondary)
```

### `CheckInStatus` (0..3):
```
0 = Registered
1 = CheckedIn
2 = Cancelled
3 = NoShow
```

---

## 9. الـ Deployment والنشر

### Backend على MonsterASP:

1. **Publish**:
   ```powershell
   cd D:\BackEnd\testproject\Blood-Bond
   dotnet publish -c Release -o ./publish
   ```

2. **Upload** محتوى `publish/` لـ MonsterASP عبر FTP

3. **Environment Variables** (ضروري):
   ```
   ASPNETCORE_ENVIRONMENT = Production
   Stripe__SecretKey = sk_test_51TOl82B...
   Stripe__PublishableKey = pk_test_51TOl82B...
   Stripe__WebhookSecret = whsec_...
   Stripe__SuccessUrl = https://your-app.vercel.app/payment-success?session_id={CHECKOUT_SESSION_ID}
   Stripe__CancelUrl = https://your-app.vercel.app/payment-cancel
   ```

4. **Stripe Webhook** (في dashboard.stripe.com):
   - URL: `https://blood-bond.runasp.net/api/monetarydonations/webhook`
   - Events: `payment_intent.succeeded`, `payment_intent.payment_failed`

### Frontend على Vercel:

1. **Vercel Environment Variable**:
   ```
   VITE_API_BASE_URL = https://blood-bond.runasp.net
   ```

2. **Deploy**:
   - ارفع `BloodBond.Web/dist/` أو ادفع عبر Git
   - **Redeploy مع Clear build cache**

### Local Development:

```powershell
# Backend (مع secrets)
$env:ASPNETCORE_ENVIRONMENT = 'Development'
cd D:\BackEnd\testproject\Blood-Bond\BloodBond
dotnet run

# Frontend
cd D:\BackEnd\testproject\Blood-Bond\BloodBond.Web
npm install
npm run dev
```

### Secrets (local dev):
ملف: `%APPDATA%\microsoft\UserSecrets\ff37ed95-ec97-4de3-ab64-7faab9a587d2\secrets.json`
```json
{
  "Jwt": {
    "Key": "...",
    "Issuer": "BloodBondApi",
    "Audience": "BloodBondClient",
    "DurationInDays": "7"
  },
  "AdminBootstrap": {
    "SecretKey": "dev-key-12345-abcde"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": "whsec_...",
    "Currency": "usd",
    "SuccessUrl": "https://your-app.vercel.app/payment-success?session_id={CHECKOUT_SESSION_ID}",
    "CancelUrl": "https://your-app.vercel.app/payment-cancel"
  }
}
```

---

## 10. حل المشاكل الشائعة (Troubleshooting)

### ❌ "Server error (500)" على Update blood bank أو Set inventory
**السبب**: النسخة القديمة من الـ backend على MonsterASP
**الحل**: انشر الـ backend المحدّث (يحتوي `IsAdminOrManager`)

### ❌ Status يعرض "Unknown"
**السبب**: الـ frontend ما عنده enum الصحيح
**الحل**: محدّث — `constants.js` يحتوي `DONATION_STATUSES` و `REQUEST_STATUSES` بالـ enum الصحيح

### ❌ "Test payment (mock mode)" بدل Stripe Checkout
**السبب**: الـ `Stripe.SecretKey` فاضي أو يحتوي "REPLACE" أو ما اتحط بـ `appsettings.json` / `secrets.json`
**الحل**:
- تأكد من وضع الـ keys في `secrets.json` أو Environment Variables
- تأكد إن `ASPNETCORE_ENVIRONMENT = Development` لـ local (الـ secrets ما بتقرأ بـ Production)
- تأكد إن `appsettings.json` ما فيه Stripe section فاضي

### ❌ الـ Admin ما يشوف /admin/users في الـ Navbar
**السبب**: الـ roles[] ما بتقرأ (الـ backend بيرجع roles كـ array)
**الحل**: محدّث — `AuthContext` يقرأ `roles[]` ويختار أعلى privilege role

### ❌ "eventsApi.register is not a function"
**السبب**: الـ frontend كان يستخدم `rsvp` بدل `register`
**الحل**: محدّث — `events.js` يحتوي `register` و `cancel` و `checkIn` (الـ backend endpoints الصح)

### ❌ الـ Manager ما يشوف donations بنكه
**السبب**: الـ `load()` كان يجلب فقط `/mine`
**الحل**: محدّث — الـ Manager الآن يجلب `/mine` + `/by-bank-detail/{bankId}` ويجمعهم

### ❌ الـ Admin ما يقدر يعمل Approve/Reject blood bank
**السبب**: الـ frontend كان يبحث عن `status === 'Pending'` (string) بس الـ backend يرجع `0` (int)
**الحل**: محدّث — `BloodBanksPage` يطابق الـ int والـ string

### ❌ الـ Stripe Checkout يفتح بـ locale مختلف
**السبب**: الـ Stripe locale يتبع الـ browser
**الحل**: هذا default لـ Stripe، يمكن تغييره بـ Stripe Checkout settings

---

## ملخص سريع:

| الميزة | الـ URL | الـ Method | الـ Auth |
|--------|---------|-----------|----------|
| Login | `/api/Account/login` | POST | Public |
| Register | `/api/Account/Register` | POST | Public |
| Current User | `/api/Account/me` | GET | Auth |
| List Users | `/api/admin/users` | GET | Admin |
| Change Role | `/api/admin/users/{id}/role` | PATCH | Admin |
| Block User | `/api/admin/users/{id}/block` | PATCH | Admin |
| Analytics | `/api/admin/analytics` | GET | Admin |
| List Banks | `/api/bloodbanks` | GET | Public |
| Create Bank | `/api/bloodbanks` | POST | User/Admin |
| Approve Bank | `/api/bloodbanks/{id}/approve` | PATCH | Admin |
| Update Inventory | `/api/bloodbanks/{id}/inventory` | PUT | Manager/Admin |
| Create Request | `/api/bloodrequests` | POST | User |
| Fulfill Request | `/api/bloodrequests/{id}/fulfill` | PATCH | Manager/Admin |
| Bank Requests | `/api/bloodrequests/by-bank/{bankId}` | GET | Manager/Admin |
| Schedule Donation | `/api/donations` | POST | User |
| Approve Donation | `/api/donations/{id}/approve` | PATCH | Manager/Admin |
| Complete Donation | `/api/donations/{id}/complete` | PATCH | Manager/Admin |
| Eligibility Check | `/api/eligibility` | POST | User |
| Upcoming Events | `/api/events/upcoming` | GET | Public |
| Register for Event | `/api/events/{id}/register` | POST | User |
| Stripe Donation | `/api/monetarydonations/create-intent` | POST | User |
| Bank Donations | `/api/monetarydonations/by-bank-detail/{bankId}` | GET | Manager/Admin |
| All Donations | `/api/monetarydonations/all` | GET | Admin |
| Add Rating | `/api/ratings` | POST | User |
| My Badges | `/api/badges/mine` | GET | User |
| Leaderboard | `/api/badges/leaderboard` | GET | User |

---

**تم! 🎉 كل شي جاهز للشرح غداً.**
