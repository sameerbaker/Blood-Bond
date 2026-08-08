namespace BloodBond.DAL.Enums
{
    public enum BloodBankStatus
    {
        Pending = 0,    // Registered, awaiting admin verification
        Verified = 1,   // Admin approved — visible to users
        Rejected = 2    // Admin rejected
    }
}
