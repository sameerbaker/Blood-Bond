using System.Collections.Generic;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.utils
{
    
    public static class BloodCompatibility
    {
        private static readonly Dictionary<BloodType, List<BloodType>> _recipientCompatibleDonors = new()
        {
            { BloodType.APpositive, new List<BloodType> { BloodType.APpositive, BloodType.ANegative, BloodType.OPositive, BloodType.ONegative } },
            { BloodType.ANegative, new List<BloodType> { BloodType.ANegative, BloodType.ONegative } },
            { BloodType.BPositive,  new List<BloodType> { BloodType.BPositive, BloodType.BNegative, BloodType.OPositive, BloodType.ONegative } },
            { BloodType.BNegative,  new List<BloodType> { BloodType.BNegative, BloodType.ONegative } },
            { BloodType.ABPositive, new List<BloodType> { BloodType.APpositive, BloodType.ANegative, BloodType.BPositive, BloodType.BNegative, BloodType.ABPositive, BloodType.ABNegative, BloodType.OPositive, BloodType.ONegative } },
            { BloodType.ABNegative, new List<BloodType> { BloodType.ANegative, BloodType.BNegative, BloodType.ABNegative, BloodType.ONegative } },
            { BloodType.OPositive,  new List<BloodType> { BloodType.OPositive, BloodType.ONegative } },
            { BloodType.ONegative,  new List<BloodType> { BloodType.ONegative } },
        };

        public static bool CanDonateTo(BloodType donor, BloodType recipient)
        {
            return _recipientCompatibleDonors.TryGetValue(recipient, out var list) && list.Contains(donor);
        }
    }
}
