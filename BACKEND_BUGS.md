# 🐛 Backend Bugs — Found via Production Logs

These issues were found by reading `app.err.log` and `app.log` on the
deployed instance. They prevent several features from working
end-to-end through the UI.

---

## Bug #1: `SetInventoryAsync` / `UpdateAsync` reject non-managers with 500

### Evidence
```
fail: BloodBond.Middleware.GlobalExceptionHandling[0]
  Unhandled exception: You are not the manager of this blood bank.
  System.UnauthorizedAccessException: You are not the manager of this blood bank.
     at BloodBond.Middleware.GlobalExceptionHandling.InvokeAsync(...)
```

### Where
`BloodBond.BLL/Service/BloodBankService.cs` — methods `SetInventoryAsync`
and `UpdateAsync` (or whatever checks ownership).

### Problem
The service throws `UnauthorizedAccessException` instead of returning a
proper `403 Forbidden` response. The exception is then caught by
`GlobalExceptionHandling` which returns a generic 500. Worse, the check
blocks **Admins** from updating blood banks they don't manage.

### Fix
Two changes needed:

**A) `BloodBankService.SetInventoryAsync` / `UpdateAsync`:**
```csharp
public async Task<BloodBankResponse> SetInventoryAsync(int bankId, string userId, List<BloodInventoryRequest> items)
{
    var user = await _userManager.FindByIdAsync(userId);
    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

    var bank = await _repo.GetByIdAsync(bankId);
    if (bank == null) throw new KeyNotFoundException("Bank not found.");

    // Admins can edit any bank. Others must be the bank manager.
    if (!isAdmin && bank.ManagerId != userId)
    {
        throw new UnauthorizedAccessException("You are not the manager of this blood bank.");
    }

    // ... rest of the upsert logic
}
```

Apply the same check to `UpdateAsync` and `ApproveAsync` if needed.

**B) `BloodBond/Middleware/GlobalExceptionHandling.cs`:**
Stop converting `UnauthorizedAccessException` into a 500. Map it to 403:
```csharp
catch (UnauthorizedAccessException ex)
{
    context.Response.StatusCode = StatusCodes.Status403Forbidden;
    await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    return;
}
```

### Frontend impact
Right now an admin opening the Inventory modal of any bank gets
`Server error (500)` with no clue. After the fix, the request will
succeed (admin override) and the frontend just shows the success toast.

---

## Bug #2: `CreateBloodBank` works but `CreateEvent` shows 500 in older logs

### Evidence
The latest `app.log` shows `INSERT INTO [BloodDriveEvents]` succeeded.
The 500 in earlier logs was a transient issue (possibly the
`BloodBankId` foreign key failing because the bank was just created
without an ID). This is now fixed.

### Status
✅ Works (no action needed).

---

## Bug #3: Monetary donations have no manual approve endpoint

### Where
`BloodBond/Controllers/MonetaryDonationsController.cs`

### Why
Stripe is the source of truth — `payment_intent.succeeded` and
`payment_intent.payment_failed` webhooks update the donation's status
automatically. There is no admin manual approve.

### Frontend impact
The admin sees monetary donations in the user's "Donate Money" page
with status `Pending` / `Succeeded` / `Failed`. This is correct.

### Optional fix
If you want admins to see ALL monetary donations (across users), add:
```csharp
[HttpGet("all")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<MonetaryDonationResponse>>> GetAll()
{
    return Ok(await _donationService.GetAllAsync());
}
```

---

## Bug #4: Frontend could not detect admin role

### Where
`BloodBond.Web/src/context/AuthContext.jsx`

### Evidence
Before the fix, the code only read `user.role` (singular) but the
backend returns `user.roles` (array). So `role` was always `null` and
the Admin menu never appeared.

### Fix (already shipped)
The frontend now reads `roles[]` and picks the highest-privilege role
for UI purposes. **No backend change required.**

---

## How to test after fixing

1. Login as `admin@bloodbond.com / Admin@123456`
2. Open `/blood-banks`
3. Click **Inventory** on any bank
4. Change units, click **Save inventory** — should succeed ✅
5. Click **Edit** on any bank, change name, click **Save** — should
   succeed ✅
6. Open `/events` → **Manage** tab → **+ New event** — should
   succeed ✅
7. Open `/admin/users` — dropdown changes the role ✅

---

## Backend endpoints that work right now (verified from logs)

| Endpoint                                 | Status |
|------------------------------------------|--------|
| `POST /api/Account/login`                | ✅ OK  |
| `GET /api/Account/me`                    | ✅ OK  |
| `GET /api/bloodbanks`                    | ✅ OK  |
| `POST /api/bloodbanks`                   | ✅ OK  |
| `PATCH /api/bloodbanks/{id}/approve`     | ✅ OK  |
| `PATCH /api/bloodbanks/{id}/reject`      | ✅ OK  |
| `GET /api/bloodbanks/verified`           | ✅ OK  |
| `POST /api/events`                       | ✅ OK  |
| `PUT /api/events/{id}`                   | ✅ OK  |
| `GET /api/events/upcoming`               | ✅ OK  |
| `GET /api/admin/users`                   | ✅ OK  |
| `POST /api/admin/create`                 | ✅ OK  |
| `PATCH /api/admin/users/{id}/role`       | ✅ OK  |
| `PATCH /api/admin/users/{id}/block`      | ✅ OK  |
| `GET /api/admin/analytics`               | ✅ OK  |
| `POST /api/bloodrequests`                | ✅ OK  |
| `PATCH /api/bloodrequests/{id}/fulfill`  | ✅ OK  |
| `POST /api/eligibility`                  | ✅ OK  |
| `POST /api/donations`                    | ✅ OK  |
| `PATCH /api/donations/{id}/approve`      | ✅ OK  |
| `PATCH /api/donations/{id}/complete`     | ✅ OK  |
| `POST /api/ratings`                      | ✅ OK  |
| `POST /api/monetarydonations/create-intent` | ✅ OK  |

## Backend endpoints that **need a fix** to work from the UI

| Endpoint                                  | Why it fails today |
|-------------------------------------------|--------------------|
| `PUT /api/bloodbanks/{id}/inventory`      | 500 (admin not the bank's manager) |
| `PUT /api/bloodbanks/{id}`                | 500 (admin not the bank's manager) |
