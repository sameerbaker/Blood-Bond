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
using Microsoft.Extensions.Options;
using Stripe;

namespace BloodBond.BLL.Service
{
    public class MonetaryDonationService : IMonetaryDonationService
    {
        private readonly IMonetaryDonationRepository _donationRepo;
        private readonly IBloodBankRepository _bankRepo;
        private readonly ApplicationDbContext _context;
        private readonly StripeSettings _stripe;

        public MonetaryDonationService(
            IMonetaryDonationRepository donationRepo,
            IBloodBankRepository bankRepo,
            ApplicationDbContext context,
            IOptions<StripeSettings> stripeOptions)
        {
            _donationRepo = donationRepo;
            _bankRepo = bankRepo;
            _context = context;
            _stripe = stripeOptions.Value;
        }

        public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(string donorId, MonetaryDonationRequest request)
        {
            if (request.BloodBankId.HasValue)
            {
                var bank = await _bankRepo.GetByIdAsync(request.BloodBankId.Value)
                    ?? throw new KeyNotFoundException("Blood bank not found.");
            }

            // Amount in cents
            long amountInCents = (long)(request.Amount * 100);
            string currency = (request.Currency ?? "usd").ToLower();

            if (string.IsNullOrWhiteSpace(_stripe.SecretKey)
                || _stripe.SecretKey.Contains("REPLACE", StringComparison.OrdinalIgnoreCase)
                || _stripe.SecretKey.Contains("your-stripe", StringComparison.OrdinalIgnoreCase))
            {
                var mockId = $"pi_mock_{Guid.NewGuid():N}";
                var donation = new MonetaryDonation
                {
                    DonorId = donorId,
                    BloodBankId = request.BloodBankId,
                    Amount = request.Amount,
                    Currency = currency,
                    StripePaymentIntentId = mockId,
                    Status = "Pending",
                    DonationDate = DateTime.UtcNow
                };
                await _donationRepo.AddAsync(donation);
                await _context.SaveChangesAsync();

                return new PaymentIntentResponse
                {
                    ClientSecret = $"{mockId}_secret_mock",
                    PaymentIntentId = mockId,
                    Amount = request.Amount,
                    Currency = currency,
                    Status = "requires_payment_method",
                    IsMock = true
                };
            }

            StripeConfiguration.ApiKey = _stripe.SecretKey;

            var donor = await _context.Users.FindAsync(donorId);
            var bankName = request.BloodBankId.HasValue
                ? (await _bankRepo.GetByIdAsync(request.BloodBankId.Value))?.Name
                : "BloodBond Foundation";

            var checkoutOptions = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = amountInCents,
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Donation to {bankName}",
                                Description = "Support our mission to save lives through blood donation"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7000/api/monetarydonations/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7000/api/monetarydonations/cancel",
                CustomerEmail = donor?.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "donorId", donorId },
                    { "bloodBankId", request.BloodBankId?.ToString() ?? "" }
                }
            };

            var sessionService = new Stripe.Checkout.SessionService();
            var session = await sessionService.CreateAsync(checkoutOptions);

            var realDonation = new MonetaryDonation
            {
                DonorId = donorId,
                BloodBankId = request.BloodBankId,
                Amount = request.Amount,
                Currency = currency,
                StripePaymentIntentId = session.Id,  // using session id as reference
                Status = "Pending",
                DonationDate = DateTime.UtcNow
            };
            await _donationRepo.AddAsync(realDonation);
            await _context.SaveChangesAsync();

            return new PaymentIntentResponse
            {
                ClientSecret = session.Id,
                PaymentIntentId = session.Id,
                CheckoutUrl = session.Url,
                SessionId = session.Id,
                Amount = request.Amount,
                Currency = currency,
                Status = "checkout_created",
                IsMock = false
            };
        }

        public async Task<MonetaryDonationResponse> ConfirmDonationAsync(string paymentIntentId, string status)
        {
            var donation = await _context.MonetaryDonations
                .Include(m => m.Donor)
                .Include(m => m.BloodBank)
                .FirstOrDefaultAsync(m => m.StripePaymentIntentId == paymentIntentId)
                ?? throw new KeyNotFoundException("Donation not found for this payment intent.");

            donation.Status = status;
            _context.MonetaryDonations.Update(donation);
            await _context.SaveChangesAsync();
            return MapToResponse(donation);
        }

        public async Task<IEnumerable<MonetaryDonationResponse>> GetMineAsync(string donorId)
        {
            var list = await _donationRepo.GetByDonorAsync(donorId);
            return list.Select(MapToResponse);
        }

        public async Task<IEnumerable<MonetaryDonationResponse>> GetByBloodBankAsync(int bloodBankId, string managerId)
        {
            var bank = await _bankRepo.GetByIdAsync(bloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            var list = await _donationRepo.GetByBloodBankAsync(bloodBankId);
            return list.Select(MapToResponse);
        }

        public async Task<decimal> GetTotalByDonorAsync(string donorId)
        {
            return await _donationRepo.GetTotalByDonorAsync(donorId);
        }

        private static MonetaryDonationResponse MapToResponse(MonetaryDonation m) => new()
        {
            Id = m.Id,
            DonorId = m.DonorId,
            DonorName = m.Donor?.FullName,
            BloodBankId = m.BloodBankId,
            BloodBankName = m.BloodBank?.Name,
            Amount = m.Amount,
            Currency = m.Currency,
            DonationDate = m.DonationDate,
            StripePaymentIntentId = m.StripePaymentIntentId,
            Status = m.Status
        };
    }
}
