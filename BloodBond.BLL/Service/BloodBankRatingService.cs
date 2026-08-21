using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Models;
using BloodBond.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.BLL.Service
{
    public class BloodBankRatingService : IBloodBankRatingService
    {
        private readonly IBloodBankRatingRepository _ratingRepo;
        private readonly IBloodBankRepository _bankRepo;
        private readonly ApplicationDbContext _context;

        public BloodBankRatingService(
            IBloodBankRatingRepository ratingRepo,
            IBloodBankRepository bankRepo,
            ApplicationDbContext context)
        {
            _ratingRepo = ratingRepo;
            _bankRepo = bankRepo;
            _context = context;
        }

        public async Task<BloodBankRatingResponse> AddOrUpdateAsync(string userId, AddRatingRequest request)
        {
            // Validate bank
            var bank = await _bankRepo.GetByIdAsync(request.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            // Check if user already rated this bank
            var existing = await _ratingRepo.GetByUserAndBankAsync(userId, request.BloodBankId);

            if (existing != null)
            {
                // Update existing rating
                existing.Rating = request.Rating;
                existing.Comment = request.Comment;
                _ratingRepo.Update(existing);
            }
            else
            {
                // Create new
                existing = new BloodBankRating
                {
                    UserId = userId,
                    BloodBankId = request.BloodBankId,
                    Rating = request.Rating,
                    Comment = request.Comment
                };
                await _ratingRepo.AddAsync(existing);
            }

            await _context.SaveChangesAsync();
            return await MapToResponseAsync(existing);
        }

        public async Task<IEnumerable<BloodBankRatingResponse>> GetByBloodBankAsync(int bloodBankId)
        {
            var list = await _ratingRepo.GetByBloodBankAsync(bloodBankId);
            return list.Select(MapToResponseSimple);
        }

        public async Task<BloodBankRatingResponse?> GetByUserAndBankAsync(string userId, int bloodBankId)
        {
            var rating = await _ratingRepo.GetByUserAndBankAsync(userId, bloodBankId);
            return rating == null ? null : MapToResponseSimple(rating);
        }

        public async Task<BloodBankRatingStatsResponse> GetStatsAsync(int bloodBankId)
        {
            var bank = await _bankRepo.GetByIdAsync(bloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            var avg = await _ratingRepo.GetAverageRatingAsync(bloodBankId);
            var count = await _ratingRepo.GetCountAsync(bloodBankId);

            return new BloodBankRatingStatsResponse
            {
                BloodBankId = bank.Id,
                BloodBankName = bank.Name,
                AverageRating = Math.Round(avg, 2),
                TotalRatings = count
            };
        }

        private async Task<BloodBankRatingResponse> MapToResponseAsync(BloodBankRating r)
        {
            await Task.CompletedTask;
            return MapToResponseSimple(r);
        }

        private static BloodBankRatingResponse MapToResponseSimple(BloodBankRating r) => new()
        {
            Id = r.Id,
            BloodBankId = r.BloodBankId,
            UserId = r.UserId,
            UserName = r.User?.FullName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
