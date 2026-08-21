using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BloodBond.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Domain DbSets
        public DbSet<BloodBank> BloodBanks { get; set; }
        public DbSet<BloodInventory> BloodInventories { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<EligibilityAnswer> EligibilityAnswers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MonetaryDonation> MonetaryDonations { get; set; }
        public DbSet<BloodBankRating> BloodBankRatings { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<BloodDriveEvent> BloodDriveEvents { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity tables → PascalCase
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // ---- BloodBank ----
            builder.Entity<BloodBank>()
                   .HasOne(b => b.Manager)
                   .WithMany()
                   .HasForeignKey(b => b.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BloodBank>()
                   .HasOne(b => b.CreatedBy)
                   .WithMany()
                   .HasForeignKey(b => b.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BloodBank>()
                   .HasOne(b => b.UpdatedBy)
                   .WithMany()
                   .HasForeignKey(b => b.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- BloodInventory ----
            builder.Entity<BloodInventory>()
                   .HasOne(i => i.BloodBank)
                   .WithMany(b => b.Inventory)
                   .HasForeignKey(i => i.BloodBankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BloodInventory>()
                   .HasIndex(i => new { i.BloodBankId, i.BloodType })
                   .IsUnique();

            // ---- BloodRequest ----
            builder.Entity<BloodRequest>()
                   .HasOne(r => r.Requester)
                   .WithMany()
                   .HasForeignKey(r => r.RequesterId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- Donation ----
            builder.Entity<Donation>()
                   .HasOne(d => d.Donor)
                   .WithMany()
                   .HasForeignKey(d => d.DonorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Donation>()
                   .HasOne(d => d.BloodBank)
                   .WithMany()
                   .HasForeignKey(d => d.BloodBankId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Donation>()
                   .HasOne(d => d.Request)
                   .WithMany()
                   .HasForeignKey(d => d.RequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- EligibilityAnswer ----
            builder.Entity<EligibilityAnswer>()
                   .HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- Notification ----
            builder.Entity<Notification>()
                   .HasOne(n => n.User)
                   .WithMany()
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ---- MonetaryDonation ----
            builder.Entity<MonetaryDonation>()
                   .HasOne(m => m.Donor)
                   .WithMany()
                   .HasForeignKey(m => m.DonorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MonetaryDonation>()
                   .HasOne(m => m.BloodBank)
                   .WithMany()
                   .HasForeignKey(m => m.BloodBankId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- BloodBankRating ----
            builder.Entity<BloodBankRating>()
                   .HasOne(r => r.BloodBank)
                   .WithMany()
                   .HasForeignKey(r => r.BloodBankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BloodBankRating>()
                   .HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One rating per user per bank (prevent duplicate spam)
            builder.Entity<BloodBankRating>()
                   .HasIndex(r => new { r.UserId, r.BloodBankId })
                   .IsUnique();

            // ---- UserBadge (composite PK by convention: UserId + BadgeId) ----
            builder.Entity<UserBadge>()
                   .HasKey(ub => new { ub.UserId, ub.BadgeId });

            builder.Entity<UserBadge>()
                   .HasOne(ub => ub.User)
                   .WithMany()
                   .HasForeignKey(ub => ub.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserBadge>()
                   .HasOne(ub => ub.Badge)
                   .WithMany()
                   .HasForeignKey(ub => ub.BadgeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ---- BloodDriveEvent ----
            builder.Entity<BloodDriveEvent>()
                   .HasOne(e => e.BloodBank)
                   .WithMany()
                   .HasForeignKey(e => e.BloodBankId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ---- EventAttendance (composite PK: EventId + UserId) ----
            builder.Entity<EventAttendance>()
                   .HasKey(a => new { a.EventId, a.UserId });

            builder.Entity<EventAttendance>()
                   .HasOne(a => a.Event)
                   .WithMany(e => e.Attendances)
                   .HasForeignKey(a => a.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventAttendance>()
                   .HasOne(a => a.User)
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var entries = ChangeTracker.Entries<AuditableEntity>();
                var currentUserId = _httpContextAccessor.HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Property(nameof(AuditableEntity.CreatedAt))?.CurrentValue == null)
                            entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                        if (entry.Property("CreatedById") != null)
                            entry.Property("CreatedById").CurrentValue = currentUserId;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                        if (entry.Property("UpdatedById") != null)
                            entry.Property("UpdatedById").CurrentValue = currentUserId;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
