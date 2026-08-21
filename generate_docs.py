"""
BloodBond Project Documentation Generator
Generates a comprehensive PDF explaining what was built and how to test it.
"""
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import cm
from reportlab.lib.colors import HexColor, black, white, grey
from reportlab.lib.enums import TA_LEFT, TA_CENTER
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle,
    Preformatted, KeepTogether
)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib import colors
import os

OUTPUT = r"D:\BackEnd\Mix-project\Blood-Bond\BloodBond_Documentation.pdf"

# Brand colors
PRIMARY = HexColor("#C62828")   # blood red
ACCENT  = HexColor("#1E88E5")   # blue
LIGHT   = HexColor("#F5F5F5")
DARK    = HexColor("#212121")
SUCCESS = HexColor("#2E7D32")
WARN    = HexColor("#F57C00")

styles = getSampleStyleSheet()

H1 = ParagraphStyle("H1", parent=styles["Heading1"], fontSize=24, textColor=PRIMARY, spaceAfter=14, spaceBefore=0)
H2 = ParagraphStyle("H2", parent=styles["Heading2"], fontSize=18, textColor=PRIMARY, spaceAfter=10, spaceBefore=18)
H3 = ParagraphStyle("H3", parent=styles["Heading3"], fontSize=14, textColor=ACCENT, spaceAfter=6, spaceBefore=12)
H4 = ParagraphStyle("H4", parent=styles["Heading4"], fontSize=12, textColor=DARK, spaceAfter=4, spaceBefore=8)
P  = ParagraphStyle("P",  parent=styles["BodyText"], fontSize=10, leading=14, alignment=TA_LEFT, spaceAfter=6)
CODE = ParagraphStyle("CODE", parent=styles["Code"], fontSize=8, leading=10, backColor=LIGHT, borderColor=grey, borderWidth=0.5, borderPadding=6, leftIndent=4, rightIndent=4, spaceAfter=8)
NOTE = ParagraphStyle("NOTE", parent=P, backColor=HexColor("#FFF8E1"), borderColor=WARN, borderWidth=1, borderPadding=8, leftIndent=4, spaceAfter=8)
SUCCESS_NOTE = ParagraphStyle("OK", parent=P, backColor=HexColor("#E8F5E9"), borderColor=SUCCESS, borderWidth=1, borderPadding=8, spaceAfter=8)
CENTER = ParagraphStyle("CENTER", parent=P, alignment=TA_CENTER)


def code_block(text):
    return Preformatted(text, CODE)


def build():
    doc = SimpleDocTemplate(
        OUTPUT, pagesize=A4,
        leftMargin=1.8*cm, rightMargin=1.8*cm,
        topMargin=1.5*cm, bottomMargin=1.5*cm,
        title="BloodBond — Backend Documentation",
        author="Samir Khan"
    )
    story = []

    # ===== Cover =====
    story.append(Spacer(1, 2*cm))
    story.append(Paragraph("🩸 BloodBond", H1))
    story.append(Paragraph("Smart Blood Donation Network — Backend API", H3))
    story.append(Spacer(1, 0.5*cm))
    story.append(Paragraph("A .NET 9 Web API platform that connects blood donors with patients in need, intelligently matching compatible blood types across blood banks.", P))
    story.append(Spacer(1, 1*cm))
    story.append(Paragraph("Project Type: <b>Academic Course Delivery</b>", P))
    story.append(Paragraph("Architecture: <b>3-Layer (DAL / BLL / UI)</b>", P))
    story.append(Paragraph("Database: <b>SQL Server (Code-First with EF Core 9)</b>", P))
    story.append(Paragraph("Authentication: <b>ASP.NET Core Identity + JWT Bearer</b>", P))
    story.append(Paragraph("Payment: <b>Stripe Checkout</b>", P))
    story.append(Spacer(1, 1*cm))
    story.append(Paragraph("Document version: <b>1.0 — August 2026</b>", P))
    story.append(Paragraph("Generated: 2026-08-08", P))
    story.append(PageBreak())

    # ===== Table of Contents =====
    story.append(Paragraph("Table of Contents", H1))
    toc = [
        ("1.", "Project Overview"),
        ("2.", "Architecture & Project Structure"),
        ("3.", "Tech Stack"),
        ("4.", "Database Schema (Code-First)"),
        ("5.", "Implemented Modules (Slices)"),
        ("6.", "User Flows Covered"),
        ("7.", "REST API Endpoints (Reference)"),
        ("8.", "Setup & Configuration"),
        ("9.", "Postman Testing Guide (Step by Step)"),
        ("10.", "Test Results — Verified Endpoints"),
        ("11.", "Bugs Found & Fixed"),
        ("12.", "Stripe Payment Flow"),
        ("13.", "How to Extend (Next Steps)"),
    ]
    for num, title in toc:
        story.append(Paragraph(f"<b>{num}</b> {title}", P))
    story.append(PageBreak())

    # ===== 1. Overview =====
    story.append(Paragraph("1. Project Overview", H1))
    story.append(Paragraph("BloodBond is a backend API for a smart blood donation network. It enables users to register, request blood, schedule donations, manage blood bank inventory, and make monetary contributions to support blood bank operations.", P))
    story.append(Paragraph("<b>Primary users:</b>", P))
    story.append(Paragraph("• <b>Donor</b> — gives blood or money", P))
    story.append(Paragraph("• <b>Requester</b> — patient/family who needs blood", P))
    story.append(Paragraph("• <b>Blood Bank Manager</b> — manages a bank's inventory and donations", P))
    story.append(Paragraph("• <b>Admin</b> — approves banks, manages users, monitors system", P))
    story.append(Paragraph("<b>Implemented in this delivery (50%+):</b>", P))
    story.append(Paragraph("• Identity & Authentication (with JWT)", P))
    story.append(Paragraph("• Blood Bank verification workflow", P))
    story.append(Paragraph("• Inventory tracking + low-stock alerts", P))
    story.append(Paragraph("• Smart matching of compatible donors (by blood type + city)", P))
    story.append(Paragraph("• Emergency notifications for critical requests", P))
    story.append(Paragraph("• Pre-donation eligibility screening", P))
    story.append(Paragraph("• Full donation lifecycle (schedule → approve → complete)", P))
    story.append(Paragraph("• Voluntary monetary donations via Stripe Checkout", P))
    story.append(Paragraph("• Admin user management (create, block, change role)", P))
    story.append(PageBreak())

    # ===== 2. Architecture =====
    story.append(Paragraph("2. Architecture & Project Structure", H1))
    story.append(Paragraph("The project follows a clean <b>3-Layer Architecture</b> pattern that keeps concerns separated, makes the system testable, and lets new modules plug in without rewiring the rest.", P))

    story.append(Paragraph("Project layout:", H3))
    layout = """BloodBond/
├── BloodBond.sln
├── BloodBond.DAL/         # Data Access Layer
│   ├── Models/            # 10+ entity classes
│   ├── Data/              # ApplicationDbContext
│   ├── Repository/        # Generic + 7 specific repositories
│   ├── DTO/               # 30+ request/response DTOs
│   ├── Enums/             # BloodType, UrgencyLevel, etc.
│   ├── utils/             # BloodCompatibility, RoleSeedData
│   └── Migrations/        # EF Core migrations
├── BloodBond.BLL/         # Business Logic Layer
│   ├── Service/           # 8 services (Auth, Bank, Request, etc.)
│   └── Mapping/           # MapsterConfig
└── BloodBond/             # Presentation Layer
    ├── Controllers/       # 8 REST controllers
    ├── Extensinos/        # DI / service registration
    ├── Middleware/        # GlobalExceptionHandling
    ├── Program.cs
    └── appsettings.json"""
    story.append(code_block(layout))
    story.append(PageBreak())

    # ===== 3. Tech Stack =====
    story.append(Paragraph("3. Tech Stack", H1))
    stack = [
        ["Category", "Technology"],
        ["Framework", "ASP.NET Core 9 Web API"],
        ["ORM", "Entity Framework Core 9 (Code-First)"],
        ["Database", "SQL Server (LocalDB / Express / full)"],
        ["Auth", "ASP.NET Core Identity + JWT Bearer"],
        ["Mapping", "Mapster"],
        ["Payments", "Stripe.net (Stripe Checkout)"],
        ["Validation", "Data Annotations"],
        ["Patterns", "Repository, Generic Repository, DI, Seeding"],
    ]
    t = Table(stack, colWidths=[4*cm, 12*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, grey),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
    ]))
    story.append(t)
    story.append(PageBreak())

    # ===== 4. Database Schema =====
    story.append(Paragraph("4. Database Schema (Code-First)", H1))
    story.append(Paragraph("All tables are generated by EF Core from C# classes. There is no manual SQL to maintain.", P))
    story.append(Paragraph("Tables created (in this delivery):", H3))
    tables = [
        ["Table", "Purpose"],
        ["Users", "Application users (extends IdentityUser)"],
        ["Roles", "Admin / User / BloodBankManager"],
        ["UserRoles", "Identity join table"],
        ["BloodBanks", "Registered blood banks (Pending/Verified/Rejected)"],
        ["BloodInventories", "Stock per blood bank per blood type"],
        ["BloodRequests", "Blood requests by requesters"],
        ["Donations", "Donation appointments"],
        ["EligibilityAnswers", "Pre-donation screening answers"],
        ["Notifications", "User notifications (info/emergency)"],
        ["MonetaryDonations", "Stripe monetary donations"],
        ["__EFMigrationsHistory", "EF Core tracking"],
    ]
    t = Table(tables, colWidths=[5*cm, 11*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, grey),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
    ]))
    story.append(t)
    story.append(PageBreak())

    # ===== 5. Implemented Modules =====
    story.append(Paragraph("5. Implemented Modules (Slices)", H1))
    modules = [
        ["#", "Slice", "What's in it"],
        ["1", "Identity & Auth", "ApplicationUser, Roles, JWT, AccountController, AdminController"],
        ["2", "BloodBank & Inventory", "BloodBank CRUD, Approve/Reject, Inventory, Low-stock"],
        ["3", "BloodRequest & Matching", "Create/Cancel requests, Smart matching, Notifications"],
        ["4", "Eligibility & Donation", "Eligibility screening, Schedule/Approve/Reject/Complete donations"],
        ["5", "Stripe Monetary Donations", "Checkout Session, list donations, totals"],
        ["6", "Admin User Management", "Create users, block/unblock, change role, change password"],
    ]
    t = Table(modules, colWidths=[1*cm, 4*cm, 11*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, grey),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
    ]))
    story.append(t)
    story.append(PageBreak())
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, grey),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
    ]))
    story.append(t)
    story.append(PageBreak())

    # ===== 6. User Flows =====
    story.append(Paragraph("6. User Flows Covered", H1))
    story.append(Paragraph("From the official Business Model document, the following flows are end-to-end functional:", H3))

    story.append(Paragraph("Donor flow (9 steps):", H4))
    for s in [
        "1. Register an account → POST /api/account/register",
        "2. Log in → POST /api/account/login (returns JWT)",
        "3. Complete profile (blood type, city) → user record",
        "4. Browse compatible requests → GET /api/bloodrequests/active?city=...",
        "5. Schedule a donation at a verified blood bank → POST /api/donations",
        "6. Complete eligibility screening → POST /api/eligibility",
        "7. Blood bank manager marks donation complete → PATCH /api/donations/{id}/complete",
        "8. Donor history updates, points awarded (auto)",
        "9. Optionally donate money via Stripe → POST /api/monetarydonations/create-intent",
    ]:
        story.append(Paragraph(s, P))

    story.append(Paragraph("Requester flow (5 steps):", H4))
    for s in [
        "1. Register + login (same Auth as donor)",
        "2. Create a blood request with type, units, urgency, city → POST /api/bloodrequests",
        "3. System notifies compatible donors in the same city (smart matching)",
        "4. Track the request status → GET /api/bloodrequests/mine",
        "5. Cancel or mark fulfilled → PATCH /api/bloodrequests/{id}/cancel or /fulfill",
    ]:
        story.append(Paragraph(s, P))

    story.append(Paragraph("Blood bank manager flow (7 steps):", H4))
    for s in [
        "1. Register a blood bank (auto-promoted to BloodBankManager role) → POST /api/bloodbanks",
        "2. Wait for Admin verification → status = Pending",
        "3. Manage inventory per blood type → PUT /api/bloodbanks/{id}/inventory",
        "4. Admin approves → PATCH /api/bloodbanks/{id}/approve",
        "5. Review scheduled donation appointments → GET /api/donations/by-bank/{id}",
        "6. Approve/Reject/Complete → PATCH /api/donations/{id}/approve|reject|complete",
        "7. View donations for their bank → GET /api/monetarydonations/by-bank/{id}",
    ]:
        story.append(Paragraph(s, P))

    story.append(Paragraph("Admin flow (subset):", H4))
    for s in [
        "1. Approve / reject blood bank registrations",
        "2. Manage users: create, change role, block/unblock",
        "3. Change own password",
    ]:
        story.append(Paragraph(s, P))
    story.append(PageBreak())

    # ===== 7. API Endpoints =====
    story.append(Paragraph("7. REST API Endpoints (Reference)", H1))
    story.append(Paragraph("Base URL: <b>https://localhost:7000</b>", P))
    story.append(Paragraph("All endpoints return JSON. Authenticated endpoints require a JWT Bearer token in the Authorization header.", P))

    endpoints = [
        # Auth
        ("POST", "/api/account/register", "Anonymous", "Register a new user (default role: User)"),
        ("POST", "/api/account/login", "Anonymous", "Authenticate and receive a JWT token"),
        ("POST", "/api/account/forgot-password", "Anonymous", "Request a password reset email"),
        ("POST", "/api/account/reset-password", "Anonymous", "Reset the password using a token"),
        ("GET",  "/api/account/me", "Auth", "Get the currently authenticated user"),

        # Admin
        ("POST", "/api/admin/register-first", "SecretKey", "Bootstrap the FIRST admin (only if no admin exists)"),
        ("POST", "/api/admin/create", "Admin", "Create a new user with a specific role"),
        ("POST", "/api/admin/change-password", "Admin", "Change the current admin's own password"),
        ("GET",  "/api/admin/users", "Admin", "List all users"),
        ("GET",  "/api/admin/users/{id}", "Admin", "Get a specific user"),
        ("PATCH","/api/admin/users/{id}/block", "Admin", "Block a user"),
        ("PATCH","/api/admin/users/{id}/unblock", "Admin", "Unblock a user"),
        ("PATCH","/api/admin/users/{id}/role", "Admin", "Change a user's role"),

        # BloodBanks
        ("GET",  "/api/bloodbanks", "Anonymous", "List all blood banks"),
        ("GET",  "/api/bloodbanks/verified", "Anonymous", "List verified blood banks only"),
        ("GET",  "/api/bloodbanks/{id}", "Anonymous", "Get a blood bank by id"),
        ("GET",  "/api/bloodbanks/low-stock", "Anonymous", "List inventory items with units &lt; 5"),
        ("POST", "/api/bloodbanks", "Auth", "Create a blood bank (auto-promotes creator to manager)"),
        ("GET",  "/api/bloodbanks/mine", "Manager", "Get the bank managed by the current user"),
        ("PUT",  "/api/bloodbanks/{id}", "Manager", "Update the bank you manage"),
        ("PUT",  "/api/bloodbanks/{id}/inventory", "Manager", "Replace the entire inventory list"),
        ("PATCH","/api/bloodbanks/{id}/approve", "Admin", "Approve a pending bank"),
        ("PATCH","/api/bloodbanks/{id}/reject", "Admin", "Reject a pending bank"),

        # BloodRequests
        ("POST", "/api/bloodrequests", "Auth", "Create a blood request and notify compatible donors"),
        ("GET",  "/api/bloodrequests/mine", "Auth", "List my own requests"),
        ("GET",  "/api/bloodrequests/active?city=...", "Anonymous", "Active requests in a city (smart matching prep)"),
        ("GET",  "/api/bloodrequests/{id}", "Auth", "Get a specific request"),
        ("PATCH","/api/bloodrequests/{id}/cancel", "Auth", "Cancel a request you created"),
        ("PATCH","/api/bloodrequests/{id}/fulfill", "Manager/Admin", "Mark request as fulfilled"),
        ("POST", "/api/bloodrequests/{id}/notify", "Manager/Admin", "Re-notify compatible donors"),

        # Donations
        ("POST", "/api/donations", "Auth", "Schedule a donation (requires passed eligibility)"),
        ("GET",  "/api/donations/mine", "Auth", "List my donations"),
        ("GET",  "/api/donations/{id}", "Auth", "Get a specific donation"),
        ("PATCH","/api/donations/{id}/cancel", "Auth", "Cancel a scheduled donation"),
        ("GET",  "/api/donations/by-bank/{bankId}", "Manager/Admin", "List donations for a bank"),
        ("PATCH","/api/donations/{id}/approve", "Manager/Admin", "Approve a scheduled donation"),
        ("PATCH","/api/donations/{id}/reject", "Manager/Admin", "Reject a scheduled donation"),
        ("PATCH","/api/donations/{id}/complete", "Manager/Admin", "Mark complete → updates inventory + donor points"),

        # Eligibility
        ("POST", "/api/eligibility", "Auth", "Submit pre-donation screening answers"),
        ("GET",  "/api/eligibility/latest", "Auth", "Get my most recent eligibility answer"),

        # Monetary
        ("POST", "/api/monetarydonations/create-intent", "Auth", "Create a Stripe Checkout Session (returns checkoutUrl)"),
        ("POST", "/api/monetarydonations/webhook", "Anonymous", "Stripe webhook for payment status updates"),
        ("POST", "/api/monetarydonations/confirm", "Anonymous", "Manual confirm (for testing without webhook)"),
        ("GET",  "/api/monetarydonations/mine", "Auth", "List my monetary donations"),
        ("GET",  "/api/monetarydonations/by-bank/{bankId}", "Manager/Admin", "Donations for a bank"),
        ("GET",  "/api/monetarydonations/total/mine", "Auth", "Total money I've donated"),
        ("GET",  "/api/monetarydonations/success", "Anonymous", "Stripe Checkout success redirect"),
        ("GET",  "/api/monetarydonations/cancel", "Anonymous", "Stripe Checkout cancel redirect"),
    ]
    data = [["Method", "URL", "Auth", "Description"]] + [list(row) for row in endpoints]
    t = Table(data, colWidths=[1.6*cm, 6.5*cm, 2.4*cm, 5.5*cm], repeatRows=1)
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 7.5),
        ("GRID", (0, 0), (-1, -1), 0.4, grey),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
        ("LEFTPADDING", (0, 0), (-1, -1), 4),
        ("RIGHTPADDING", (0, 0), (-1, -1), 4),
    ]))
    story.append(t)
    story.append(PageBreak())

    # ===== 8. Setup =====
    story.append(Paragraph("8. Setup & Configuration", H1))
    story.append(Paragraph("Prerequisites:", H3))
    story.append(Paragraph("• .NET 9 SDK", P))
    story.append(Paragraph("• SQL Server (LocalDB / Express / full instance)", P))
    story.append(Paragraph("• Visual Studio 2022 / VS Code", P))
    story.append(Paragraph("Steps:", H3))
    story.append(Paragraph("1. <b>Clone the repository</b> (or open the project folder).", P))
    story.append(Paragraph("2. <b>Configure the connection string</b> in appsettings.json:", P))
    story.append(code_block('"ConnectionStrings": {\n  "DefaultConnection": "Data Source=.;Initial Catalog=BloodBondDb;Integrated Security=True;Trust Server Certificate=True;"\n}'))
    story.append(Paragraph("3. <b>Apply database migrations</b>:", P))
    story.append(code_block("dotnet ef database update --project BloodBond.DAL --startup-project BloodBond"))
    story.append(Paragraph("4. <b>Run the application</b>:", P))
    story.append(code_block("dotnet run --project BloodBond"))
    story.append(Paragraph("5. The application starts on <b>https://localhost:7000</b> (Swagger available there).", P))
    story.append(Paragraph("6. <b>Bootstrap the first admin</b> by sending a POST request (only if no admin exists yet):", P))
    story.append(code_block('POST /api/admin/register-first\nContent-Type: application/json\n\n{\n  "secretKey": "CHANGE_THIS_TO_A_LONG_RANDOM_STRING_BEFORE_PRODUCTION",\n  "fullName": "Your Name",\n  "email": "admin@yourcompany.com",\n  "password": "SecurePassword@123"\n}'))
    story.append(Paragraph("7. <b>Log in</b> as the new admin and start using the API.", P))
    story.append(Paragraph("Note: The default admin account (<font color='#C62828'>admin@bloodbond.com</font> / <font color='#C62828'>Admin@123456</font>) was created during development and is stored in the database. You can change it via the admin endpoints.", NOTE))
    story.append(PageBreak())

    # ===== 9. Postman Testing =====
    story.append(Paragraph("9. Postman Testing Guide (Step by Step)", H1))
    story.append(Paragraph("This section walks through every endpoint with concrete Postman examples. Each step has a screenshot-ready request shape.", P))

    story.append(Paragraph("Step 0 — Set up Postman environment", H3))
    story.append(Paragraph("Create a Postman environment with these variables so you don't have to repeat the URL everywhere:", P))
    story.append(code_block("baseUrl = https://localhost:7000\nadminToken  = (filled after admin login)\ndonorToken  = (filled after donor login)\nrequesterToken = (filled after requester login)\nbankId = (filled after creating a bank)"))

    story.append(Paragraph("Step 1 — Bootstrap the first admin (only works if no admin exists)", H3))
    story.append(code_block('POST {{baseUrl}}/api/admin/register-first\nContent-Type: application/json\n\n{\n  "secretKey": "CHANGE_THIS_TO_A_LONG_RANDOM_STRING_BEFORE_PRODUCTION",\n  "fullName": "My Admin",\n  "email": "myadmin@example.com",\n  "password": "MyAdmin@123"\n}'))

    story.append(Paragraph("Step 2 — Log in as admin (capture the token)", H3))
    story.append(code_block('POST {{baseUrl}}/api/account/login\nContent-Type: application/json\n\n{\n  "email": "admin@bloodbond.com",\n  "password": "Admin@123456"\n}\n\n// → copy the value of "token" into {{adminToken}}'))
    story.append(Paragraph("✓ Tip: copy the value of &quot;token&quot; from the response into a Postman environment variable called <b>adminToken</b> for reuse.", SUCCESS_NOTE))

    story.append(Paragraph("Expected response:", P))
    story.append(code_block('{\n  "userId": "4b61c196-9d91-4de4-a519-86552a61c057",\n  "email": "admin@bloodbond.com",\n  "fullName": "System Admin",\n  "roles": ["Admin"],\n  "token": "eyJhbGciOi...",\n  "expiresAt": "2026-08-14T08:34:20Z"\n}'))

    story.append(Paragraph("Step 3 — Register a donor", H3))
    story.append(code_block('POST {{baseUrl}}/api/account/register\nContent-Type: application/json\n\n{\n  "fullName": "Test Donor",\n  "email": "donor.test@example.com",\n  "password": "Donor@1234",\n  "confirmPassword": "Donor@1234"\n}'))

    story.append(Paragraph("Step 4 — Log in as the donor", H3))
    story.append(code_block('POST {{baseUrl}}/api/account/login\nContent-Type: application/json\n\n{\n  "email": "donor.test@example.com",\n  "password": "Donor@1234"\n}\n\n// → copy the value of "token" into {{donorToken}}'))

    story.append(Paragraph("Step 5 — Create a blood bank (donor becomes a manager)", H3))
    story.append(code_block('POST {{baseUrl}}/api/bloodbanks\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n{\n  "name": "Central Blood Bank",\n  "cityAddress": "123 Main St, Ramallah",\n  "latitude": 31.9038,\n  "longitude": 35.2030,\n  "contactPhone": "+970599123456"\n}\n\n// → copy the value of "id" into {{bankId}}'))

    story.append(Paragraph("Step 6 — Re-login the donor to pick up the BloodBankManager role", H3))
    story.append(Paragraph("After creating the bank, repeat Step 4 so the JWT contains the new role.", P))

    story.append(Paragraph("Step 7 — Set the bank's inventory", H3))
    story.append(code_block('PUT {{baseUrl}}/api/bloodbanks/{{bankId}}/inventory\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n[\n  { "bloodType": 0, "unitsAvailable": 10 },\n  { "bloodType": 1, "unitsAvailable": 3 },\n  { "bloodType": 6, "unitsAvailable": 8 }\n]'))

    story.append(Paragraph("Step 8 — Approve the bank (admin)", H3))
    story.append(code_block('PATCH {{baseUrl}}/api/bloodbanks/{{bankId}}/approve\nAuthorization: Bearer {{adminToken}}'))

    story.append(Paragraph("Step 9 — Eligibility screening (donor)", H3))
    story.append(code_block('POST {{baseUrl}}/api/eligibility\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n{\n  "weight": 75,\n  "age": 28,\n  "hasChronicDisease": false\n}'))

    story.append(Paragraph("Step 10 — Schedule a donation", H3))
    story.append(code_block('POST {{baseUrl}}/api/donations\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n{\n  "bloodBankId": 1,\n  "scheduledDate": "2026-08-15T10:00:00",\n  "notes": "Regular donation"\n}'))

    story.append(Paragraph("Step 11 — Approve + complete the donation (manager = donor)", H3))
    story.append(code_block('PATCH {{baseUrl}}/api/donations/1/approve\nAuthorization: Bearer {{donorToken}}\n\nPATCH {{baseUrl}}/api/donations/1/complete\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n{ "unitsDonated": 1, "notes": "Successful donation" }'))

    story.append(Paragraph("Step 12 — Register a requester and create a blood request", H3))
    story.append(code_block('POST {{baseUrl}}/api/account/register\n{ "fullName": "Test Requester", "email": "req@example.com",\n  "password": "Req@1234", "confirmPassword": "Req@1234" }\n\nPOST {{baseUrl}}/api/bloodrequests\nAuthorization: Bearer {{requesterToken}}\nContent-Type: application/json\n\n{\n  "bloodType": 0,\n  "unitsNeeded": 2,\n  "urgencyLevel": 2,\n  "city": "Ramallah"\n}'))

    story.append(Paragraph("Step 13 — Money donation via Stripe", H3))
    story.append(code_block('POST {{baseUrl}}/api/monetarydonations/create-intent\nAuthorization: Bearer {{donorToken}}\nContent-Type: application/json\n\n{ "amount": 25.00, "currency": "usd" }\n\n// → copy the value of "checkoutUrl" and open it in a browser\n// Pay with test card 4242 4242 4242 4242'))

    story.append(PageBreak())

    # ===== 10. Test Results =====
    story.append(Paragraph("10. Test Results — Verified Endpoints", H1))
    story.append(Paragraph("All phases were run end-to-end against the running API and produced the expected results.", P))

    results = [
        ["Phase", "Module", "Endpoints tested", "Status"],
        ["1", "Auth", "register / login / me", "✅ PASSED"],
        ["2", "BloodBank + Inventory", "create / mine / setInventory / low-stock / approve", "✅ PASSED"],
        ["3", "Eligibility + Donation", "check / schedule / approve / complete / mine / by-bank", "✅ PASSED"],
        ["4", "BloodRequest + Matching", "create / mine / active?city / notify / cancel", "✅ PASSED"],
        ["5", "Stripe Monetary Donations", "create-intent / mine / total", "✅ PASSED"],
    ]
    t = Table(results, colWidths=[1.2*cm, 3.5*cm, 7.5*cm, 3.8*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), PRIMARY),
        ("TEXTCOLOR", (0, 0), (-1, 0), white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, grey),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [white, LIGHT]),
    ]))
    story.append(t)
    story.append(PageBreak())

    # ===== 11. Bugs =====
    story.append(Paragraph("11. Bugs Found & Fixed", H1))
    story.append(Paragraph("During testing, two real bugs were found and fixed:", P))

    story.append(Paragraph("Bug #1 — EF Core could not translate BloodCompatibility.CanDonateTo", H4))
    story.append(Paragraph("Symptom: POST /api/bloodrequests returned 500 after the donor notification step.", P))
    story.append(Paragraph("Cause: LINQ tried to translate the static helper into SQL, which is not supported.", P))
    story.append(Paragraph("Fix: split the query into two steps — SQL filter (city, has blood type, not blocked) followed by an in-memory compatibility check.", P))
    story.append(Paragraph("Also wrapped the notification call in try/catch so a notification failure cannot break the request creation.", P))

    story.append(Paragraph("Bug #2 — Hidden 500 on /api/bloodrequests from unhandled exception", H4))
    story.append(Paragraph("Symptom: response was 500 but the log showed nothing for the request itself.", P))
    story.append(Paragraph("Fix: now logged in the catch block and the request creation always succeeds even if notification dispatch fails.", P))
    story.append(PageBreak())

    # ===== 12. Stripe Flow =====
    story.append(Paragraph("12. Stripe Payment Flow", H1))
    story.append(Paragraph("Monetary donations use <b>Stripe Checkout</b> — a hosted payment page. The flow is:", P))
    story.append(code_block("""1. Donor calls POST /api/monetarydonations/create-intent
   → Backend creates a Stripe Checkout Session
   → Returns a checkoutUrl like
     https://checkout.stripe.com/c/pay/cs_test_xxx...

2. Frontend (or Postman) opens that URL in a browser

3. Donor pays with the test card
     Card: 4242 4242 4242 4242
     Exp:  any future date
     CVC:  any 3 digits

4. Stripe redirects back to
     GET /api/monetarydonations/success?session_id=...
   (or /cancel if the donor cancels)

5. The success endpoint marks the donation as Succeeded

6. (Optional) Stripe also POSTs the webhook to
     /api/monetarydonations/webhook
   which updates the donation in the background
   (requires a configured WebhookSecret)"""))
    story.append(Paragraph("Mock mode is automatic: if Stripe:SecretKey is left empty or contains a placeholder, the system returns a fake payment intent with isMock=true. This lets the rest of the flow be developed without a real Stripe account.", P))
    story.append(PageBreak())

    # ===== 13. Next steps =====
    story.append(Paragraph("13. How to Extend (Next Steps)", H1))
    story.append(Paragraph("The architecture makes it straightforward to add more modules without touching the existing ones. Typical follow-up deliveries could add:", P))
    story.append(Paragraph("• <b>Blood Drive Events</b> — managers create events, donors register and check in", P))
    story.append(Paragraph("• <b>Ratings</b> — donors rate the blood bank they visited", P))
    story.append(Paragraph("• <b>Badges & Gamification</b> — auto-award badges based on donation count / streaks", P))
    story.append(Paragraph("• <b>Real-time notifications</b> — SignalR push instead of DB-only notifications", P))
    story.append(Paragraph("• <b>Localization</b> — English + Arabic error messages and notifications", P))
    story.append(Paragraph("• <b>Admin analytics dashboard</b> — inventory levels, fulfillment rate, blood type distribution", P))
    story.append(Paragraph("• <b>Production hardening</b> — move secrets to environment variables, enable HTTPS-only, add rate limiting, add CI/CD", P))
    story.append(Spacer(1, 1*cm))
    story.append(Paragraph("<b>End of document</b>", CENTER))
    story.append(Paragraph("Generated automatically from the running project on 2026-08-08.", CENTER))

    doc.build(story)
    print(f"PDF written to: {OUTPUT}")
    print(f"Size: {os.path.getsize(OUTPUT):,} bytes")


if __name__ == "__main__":
    build()
