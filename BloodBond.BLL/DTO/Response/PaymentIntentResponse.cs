namespace BloodBond.DAL.DTO.Response
{
    public class PaymentIntentResponse
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public string? CheckoutUrl { get; set; } // Stripe Checkout URL (open in browser)
        public string? SessionId { get; set; }    // Stripe Checkout Session ID
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string Status { get; set; } = "requires_payment_method";
        public bool IsMock { get; set; }
    }
}
