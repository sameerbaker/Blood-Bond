using System;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Models;

namespace BloodBond.BLL.Service
{
    public class EligibilityService : IEligibilityService
    {
        private readonly ApplicationDbContext _context;

        public EligibilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EligibilityAnswerResponse> CheckAsync(string userId, EligibilityAnswerRequest request)
        {
            bool passed = true;
            string reason = "Eligible to donate.";

            if (request.Age < 18)
            {
                passed = false;
                reason = "You must be at least 18 years old.";
            }
            else if (request.Weight < 50)
            {
                passed = false;
                reason = "You must weigh at least 50 kg.";
            }
            else if (request.HasChronicDisease)
            {
                passed = false;
                reason = "Chronic disease disqualifies donation. Please consult your doctor.";
            }
            else if (request.LastSurgeryDate.HasValue
                     && (DateTime.UtcNow - request.LastSurgeryDate.Value).TotalDays < 180)
            {
                passed = false;
                reason = "You must wait at least 6 months after surgery before donating.";
            }

            var answer = new EligibilityAnswer
            {
                UserId = userId,
                Weight = request.Weight,
                Age = request.Age,
                HasChronicDisease = request.HasChronicDisease,
                LastSurgeryDate = request.LastSurgeryDate,
                Passed = passed
            };

            _context.EligibilityAnswers.Add(answer);
            await _context.SaveChangesAsync();

            return new EligibilityAnswerResponse
            {
                Id = answer.Id,
                UserId = answer.UserId,
                Weight = answer.Weight,
                Age = answer.Age,
                HasChronicDisease = answer.HasChronicDisease,
                LastSurgeryDate = answer.LastSurgeryDate,
                Passed = answer.Passed,
                Reason = reason,
                CreatedAt = answer.CreatedAt
            };
        }

        public async Task<EligibilityAnswerResponse?> GetLatestAsync(string userId)
        {
            var latest = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .FirstOrDefaultAsync(_context.EligibilityAnswers
                    .OrderByDescending(e => e.CreatedAt), e => e.UserId == userId);

            if (latest == null) return null;

            return new EligibilityAnswerResponse
            {
                Id = latest.Id,
                UserId = latest.UserId,
                Weight = latest.Weight,
                Age = latest.Age,
                HasChronicDisease = latest.HasChronicDisease,
                LastSurgeryDate = latest.LastSurgeryDate,
                Passed = latest.Passed,
                Reason = latest.Passed ? "Eligible" : "Not eligible",
                CreatedAt = latest.CreatedAt
            };
        }
    }
}
