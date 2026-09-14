namespace BloodBond.BLL.Service
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;

        // Optional: where Stripe redirects after a successful / cancelled checkout.
        // Use {CHECKOUT_SESSION_ID} in SuccessUrl — Stripe replaces it.
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }

        // Default currency when the request doesn't specify one.
        public string Currency { get; set; } = "usd";
    }
}
