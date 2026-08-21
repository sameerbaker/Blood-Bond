namespace BloodBond.DAL.Enums
{
    public enum CheckInStatus
    {
        Registered = 0,   // Signed up, not yet at the event
        CheckedIn = 1,    // Showed up at the event
        NoShow = 2,       // Registered but didn't show up
        Cancelled = 3
    }
}
