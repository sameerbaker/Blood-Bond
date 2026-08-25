using System.Globalization;
using BloodBond.DAL.Enums;
using Microsoft.Extensions.Localization;

namespace BloodBond.BLL.Service
{
    
    public class LocalizedMessages
    {
        private readonly IStringLocalizer _localizer;

        public LocalizedMessages(IStringLocalizerFactory factory)
        {
            
            _localizer = factory.Create("SharedResource", "BloodBond");
        }

        public string this[string key] => _localizer[key];

        public string Get(string key) => _localizer[key];

        public string GetBloodTypeName(BloodType type) => type switch
        {
            BloodType.APpositive => _localizer["BloodTypeAPpositive"],
            BloodType.ANegative => _localizer["BloodTypeANegative"],
            BloodType.BPositive => _localizer["BloodTypeBPositive"],
            BloodType.BNegative => _localizer["BloodTypeBNegative"],
            BloodType.ABPositive => _localizer["BloodTypeABPositive"],
            BloodType.ABNegative => _localizer["BloodTypeABNegative"],
            BloodType.OPositive => _localizer["BloodTypeOPositive"],
            BloodType.ONegative => _localizer["BloodTypeONegative"],
            _ => type.ToString()
        };

        public string GetUrgencyName(UrgencyLevel level) => level switch
        {
            UrgencyLevel.Normal => _localizer["UrgencyLevelNormal"],
            UrgencyLevel.Urgent => _localizer["UrgencyLevelUrgent"],
            UrgencyLevel.Critical => _localizer["UrgencyLevelCritical"],
            _ => level.ToString()
        };

        // ---- Common messages ----
        public string Welcome => _localizer["Welcome"];
        public string UserRegistered => _localizer["UserRegistered"];
        public string UserLogin => _localizer["UserLogin"];
        public string InvalidCredentials => _localizer["InvalidCredentials"];
        public string AccountBlocked => _localizer["AccountBlocked"];
        public string EmailExists => _localizer["EmailExists"];
        public string PasswordMismatch => _localizer["PasswordMismatch"];
        public string BloodBankCreated => _localizer["BloodBankCreated"];
        public string BloodBankUpdated => _localizer["BloodBankUpdated"];
        public string BloodBankApproved => _localizer["BloodBankApproved"];
        public string BloodBankRejected => _localizer["BloodBankRejected"];
        public string NotFound => _localizer["NotFound"];
        public string Forbidden => _localizer["Forbidden"];
        public string ValidationError => _localizer["ValidationError"];
        public string ServerError => _localizer["ServerError"];
        public string LowStock => _localizer["LowStock"];
        public string RequestCreated => _localizer["RequestCreated"];
        public string DonationScheduled => _localizer["DonationScheduled"];
        public string DonationCompleted => _localizer["DonationCompleted"];
        public string EligibilityPassed => _localizer["EligibilityPassed"];
        public string EligibilityFailed => _localizer["EligibilityFailed"];
        public string RatingThanks => _localizer["RatingThanks"];
        public string MonetaryThanks => _localizer["MonetaryThanks"];
        public string EventRegistered => _localizer["EventRegistered"];
        public string EventCheckedIn => _localizer["EventCheckedIn"];
        public string BadgeEarned => _localizer["BadgeEarned"];
        public string RateLimitExceeded => _localizer["RateLimitExceeded"];
    }
}
