using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.utils
{
    /// <summary>
    /// Seeds the default badge set. Runs after role seeding.
    /// </summary>
    public class BadgeSeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;

        public BadgeSeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Badges.Any())
                return; // already seeded

            var badges = new[]
            {
                new Badge { Name = "First Drop",     Description = "Completed your first donation",       Icon = "🩸", PointsRequired = 10  },
                new Badge { Name = "Regular Donor",  Description = "Earned 50 points through donations",   Icon = "🏅", PointsRequired = 50  },
                new Badge { Name = "Hero Donor",     Description = "Earned 100 points through donations",  Icon = "🥇", PointsRequired = 100 },
                new Badge { Name = "Life Saver",     Description = "Earned 250 points through donations",  Icon = "🏆", PointsRequired = 250 },
                new Badge { Name = "Patron",         Description = "Made a monetary donation to a blood bank", Icon = "💝", PointsRequired = 0 }
            };

            await _context.Badges.AddRangeAsync(badges);
            await _context.SaveChangesAsync();
        }
    }
}
